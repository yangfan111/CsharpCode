using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Shared.Components.Serializer;
using Core.EntityComponent;
using Core.Network;
using Core.ObjectPool;
using Core.SnapshotReplication.Serialization.Patch;

using Core.UpdateLatest;
using Core.Utils;
using log4net.Core;
using Sharpen;

namespace App.Shared.Network.SerializeInfo
{
    public class ReplicatedUpdateEntitySerializeInfo : ISerializeInfo
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ReplicatedUpdateEntitySerializeInfo));
        private ComponentSerializerManager _componentSerializerManager;

      //  private Dictionary<int, UpdateLatestPacakge> _updateLatestHistory = new Dictionary<int, UpdateLatestPacakge>();
        public IUpdateMessagePool MessagePool { get; private set; }

        public ReplicatedUpdateEntitySerializeInfo(ComponentSerializerManager instance, IUpdateMessagePool updateMessagePool)
        {
            _componentSerializerManager = instance;
            MessagePool = updateMessagePool;
            Statistics = new SerializationStatistics("UpdateEntity");
        }

        public void Dispose()
        {
            MessagePool.Dispose();
           
        }

        private List<IUpdateComponent> _emptyUpdateComponents = new List<IUpdateComponent>();
        private List<MemoryStream> _sendHistoryStreams = new List<MemoryStream>();
        private List<int> _sendHistorySeqs = new List<int>();
        public const int SendCount = 5;

        public void Serialize(Stream outStream, object message)
        {
            var msg = message as UpdateLatestPacakge;
            var binaryWriter = MyBinaryWriter.Allocate(outStream);
            if (MessagePool.GetPackageBySeq(msg.Head.UserCmdSeq)!=null)
            {
                _logger.ErrorFormat("repetition  msg.Head.UserCmdSeq{0}", msg.Head.UserCmdSeq);
                binaryWriter.Write((byte)0);
                return;
            }
            MessagePool.AddMessage(msg);
            if (_sendHistoryStreams.Count > SendCount)
            {
                RemoveHistoryFirst();
            }

            for (int i = 0; i < SendCount; i++)
            {
                if (_sendHistorySeqs.Count >0 &&msg.Head.BaseUserCmdSeq >0 && _sendHistorySeqs.First() <= msg.Head.BaseUserCmdSeq)
                {
                    RemoveHistoryFirst();
                }
                else
                {
                    break;
                }
            }
       
            var stream = SerializeSinaglePackage(msg);
            _sendHistoryStreams.AddLast(stream);
            _sendHistorySeqs.AddLast(msg.Head.UserCmdSeq);
           

            binaryWriter.Write((byte) _sendHistoryStreams.Count);
            
            foreach (var sendHistroyStream in _sendHistoryStreams)
            {
                binaryWriter.Write(sendHistroyStream.GetBuffer(),
                    (int) (sendHistroyStream.Position - sendHistroyStream.Length), (int) sendHistroyStream.Length);
            }
            _logger.DebugFormat("send package{0}",binaryWriter.Position);
            binaryWriter.ReleaseReference();
        }

        private void RemoveHistoryFirst()
        {
            _sendHistorySeqs.RemoveFirst();
            var old = _sendHistoryStreams.RemoveFirst();
            ObjectAllocatorHolder<MemoryStream>.Free(old);
        }

        private long SerializeComponents(MyBinaryWriter binaryWriter, List<IUpdateComponent> oldComponents,
            List<IUpdateComponent> currentComponents)
        {
            var startPostion = binaryWriter.Position;
            foreach (var currentComponent in currentComponents)
            {
                var serializer = _componentSerializerManager.GetSerializer(currentComponent.GetComponentId());
                bool isModife = false;
                foreach (var oldComponent in oldComponents)
                {
                    if (currentComponent.GetComponentId() == oldComponent.GetComponentId())
                    {
                        var bitMask = serializer.DiffNetworkObject(oldComponent ,
                            currentComponent );                       
                        var modifyPatch = ModifyComponentPatch.Allocate(oldComponent, currentComponent, bitMask);

                        modifyPatch.Serialize(binaryWriter, _componentSerializerManager);
                        modifyPatch.ReleaseReference();
                        
                        isModife = true;

                        break;
                    }
                }

                if (!isModife)
                {
                    var addPatch = AddComponentPatch.Allocate(currentComponent);
                    addPatch.Serialize(binaryWriter, _componentSerializerManager);
                    addPatch.ReleaseReference();
                }
            }

            var endPosition = binaryWriter.Position;
            return endPosition - startPostion;
        }

        public MemoryStream SerializeSinaglePackage(UpdateLatestPacakge msg)
        {
            MemoryStream stream = ObjectAllocatorHolder<MemoryStream>.Allocate();
            stream.Seek(0, SeekOrigin.Begin);
            var binaryWriter = MyBinaryWriter.Allocate(stream);
            long bodyLength;
            var old = MessagePool.GetPackageBySeq(msg.Head.BaseUserCmdSeq);
            if (old != null)
            {
              
                msg.Head.Serialize(binaryWriter);
                bodyLength = SerializeComponents(binaryWriter, old.UpdateComponents, msg.UpdateComponents);
            }
            else
            {
                msg.Head.BaseUserCmdSeq = -1;
                msg.Head.Serialize(binaryWriter);
                bodyLength = SerializeComponents(binaryWriter, _emptyUpdateComponents, msg.UpdateComponents);
            }

            AssertUtility.Assert(bodyLength < 65535);
            msg.Head.ReWriteBodyLength(binaryWriter, (short) bodyLength);
            binaryWriter.ReleaseReference();
            return stream;
        }

        public object Deserialize(Stream inStream)
        {
            ReusableList<UpdateLatestPacakge> list = ReusableList<UpdateLatestPacakge>.Allocate();
            BinaryReader binaryReader = new BinaryReader(inStream);
            int count = binaryReader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                UpdateLatestPacakge pacakge = UpdateLatestPacakge.Allocate();

                pacakge.Head.Deserialize(binaryReader);
                var seq = pacakge.Head.UserCmdSeq;
                var bodyLenght = pacakge.Head.BodyLength;
                if (MessagePool.GetPackageBySeq(seq) == null)
                {
                    var baseSeq = pacakge.Head.BaseUserCmdSeq;
                    var old = MessagePool.GetPackageBySeq(baseSeq);
                    if (old != null)
                    {
                        pacakge.CopyUpdateComponentsFrom(old.UpdateComponents);
                    }


                  
                  
                    for (int c = 0; c < pacakge.Head.ComponentCount; c++)
                    {
                        var opType = (ComponentReplicateOperationType) binaryReader.ReadByte();

                        var patch = CreateEmptyComponentPatch(opType);
                        patch.DeSerialize(binaryReader, _componentSerializerManager);
                        ApplyPatchTo(patch, pacakge.UpdateComponents);

                        patch.ReleaseReference();
                    }

                    if (pacakge.Head.ComponentCount == pacakge.UpdateComponents.Count)
                    {
                        MessagePool.AddMessage(pacakge);
                        pacakge.AcquireReference();
                        list.Value.Add(pacakge);
                        
                    }
                    else
                    {
                        _logger.WarnFormat("Skip package {0} with length {1} baseSeq;{2}",pacakge.Head.UserCmdSeq, pacakge.Head.BodyLength,pacakge.Head.BaseUserCmdSeq);
                    }
                }
                else
                {
                    
                    _logger.DebugFormat("Skip package {0} with length {1}",pacakge.Head.UserCmdSeq, pacakge.Head.BodyLength);
                    binaryReader.BaseStream.Seek(bodyLenght, SeekOrigin.Current);
                }

            
                pacakge.ReleaseReference();
            }

            return list;
        }

        private void ApplyPatchTo(AbstractComponentPatch patch, List<IUpdateComponent> pacakgeUpdateComponents)
        {
            if (patch is ModifyComponentPatch)
            {
                foreach (var component in pacakgeUpdateComponents)
                {
                    if (component.GetComponentId() == patch.Component.GetComponentId())
                    {
                        patch.ApplyPatchTo(component, _componentSerializerManager);
                        return;
                    }
                }
            }
            else if (patch is AddComponentPatch)
            {
                foreach (var component in pacakgeUpdateComponents)
                {
                    if (component.GetComponentId() == patch.Component.GetComponentId())
                    {
                       throw new ArgumentException(string.Format("repetition component;{0}",
                            component.GetComponentId()));
                    }
                }
                IUpdateComponent add =
                    (IUpdateComponent) GameComponentInfo.Instance.Allocate(patch.Component.GetComponentId());
                patch.ApplyPatchTo(add, _componentSerializerManager);
                pacakgeUpdateComponents.Add(add);
            }
            else
            {
                throw new ArgumentException(string.Format("error;{0}",
                    patch));
            }
        }

        private AbstractComponentPatch CreateEmptyComponentPatch(ComponentReplicateOperationType opType)
        {
            switch (opType)
            {
                case ComponentReplicateOperationType.Add:
                    return AddComponentPatch.Allocate();
                case ComponentReplicateOperationType.Del:
                    return DeleteComponentPatch.Allocate();
                case ComponentReplicateOperationType.Mod:
                    return ModifyComponentPatch.Allocate();
                default:
                    throw new Exception(string.Format("invalid ComponentReplicateOperationType {0}", opType));
            }
        }

        public SerializationStatistics Statistics { get; private set; }
    }
}