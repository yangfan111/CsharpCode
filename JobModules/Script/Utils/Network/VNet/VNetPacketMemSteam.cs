using System;
using System.IO;
using Core.Network;
using Core.ObjectPool;
using Core.Utils;
using UnityEngine;

namespace VNet
{
    internal class VNetPacketMemSteam : BaseRefCounter, IPacket
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VNetPacket));
        private MemoryStream _stream = new MemoryStream(1024*100);
        public MemoryStream Stream
        {
            get { return _stream; }
            
        }

        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(VNetPacketMemSteam)){}
            public override object MakeObject()
            {
                return new VNetPacketMemSteam();
            }

        }
        public static VNetPacketMemSteam Allocate()
        {
            VNetPacketMemSteam rc = ObjectAllocatorHolder<VNetPacketMemSteam>.Allocate();

            return rc;
        }

        public int Length
        {
            get
            {
                return (int)_stream.Length;}
        }

        public int Capacity
        {
            get { return _stream.Capacity; }
        }

        public byte[] Data
        {
            get { return _stream.GetBuffer(); }
        }

      

        public void CopyTo(byte[] array, int arrayIndex, int count, int sourceIndex)
        {
            
            Buffer.BlockCopy(_stream.GetBuffer(),arrayIndex,array,arrayIndex,count);
        }

        public void CopyFrom(byte[] array, int arrayIndex, int count)
        {
            _stream.Write(array,arrayIndex,count);
        }

        public void Dispose()
        {
            ReleaseReference();
        }

        protected override void OnCleanUp()
        {
            
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.SetLength(0);
            ObjectAllocatorHolder<VNetPacketMemSteam>.Free(this);
        }
    }
}
