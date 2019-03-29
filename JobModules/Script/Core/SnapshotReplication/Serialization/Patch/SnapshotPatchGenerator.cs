using System.Collections;
using Core.Components;
using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public class SnapshotPatchGenerator : IEntityMapCompareHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SnapshotPatchGenerator));
        private SnapshotPatch _patch;
        private AbstractEntityPatch _currentEntityPatch;
        private INetworkObjectSerializerManager _serializerManager;
        public SnapshotPatchGenerator(INetworkObjectSerializerManager serializerManager)
        {
            _patch = SnapshotPatch.Allocate();
            _serializerManager = serializerManager;
        }
        public bool IsBreak()
        {
            return false;
        }

        public bool IsExcludeComponent(IGameComponent component)
        {
            return false;
        }

        /// <summary>
        /// 生成单个Component的Patch
        /// </summary>
        /// <param name="leftEntity"></param>
        /// <param name="leftComponent"></param>
        /// <param name="rightEntity"></param>
        /// <param name="rightComponent"></param>
        public void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            var serializer = _serializerManager.GetSerializer(leftComponent.GetComponentId());
            var bitMask = serializer.DiffNetworkObject(leftComponent as INetworkObject, rightComponent as INetworkObject);
          
            if (bitMask.HasValue())
            {
                var componentPatch = ModifyComponentPatch.Allocate(leftComponent, rightComponent, bitMask);
                _currentEntityPatch.AddComponentPatch(componentPatch);
                componentPatch.ReleaseReference();
            }        
            
            
        }

       

        /// <summary>
        /// 生成新加Component的Patch
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <param name="leftEntity"></param>
        /// <param name="rightComponent"></param>
        public void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("AddComponentPatch :{0}, {1}",leftEntity.EntityKey, rightComponent.GetComponentId());
            var componentPatch = AddComponentPatch.Allocate(rightComponent);
            _currentEntityPatch.AddComponentPatch(componentPatch);
            componentPatch.ReleaseReference();
        }

        /// <summary>
        /// 在Patch中生成删除Entity的Patch
        /// </summary>
        /// <param name="leftEntity"></param>
        /// <param name="rightEntity"></param>
        /// <param name="leftComponent"></param>
        public void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.DebugFormat("DeleteComponentPatch ::{0}, {1}",leftEntity.EntityKey,  leftComponent.GetComponentId());
            var componentPatch = DeleteComponentPatch.Allocate(leftComponent);
            _currentEntityPatch.AddComponentPatch(componentPatch);
            componentPatch.ReleaseReference();
        }

        /// <summary>
        /// 在Patch中生成新加Entity的信息
        /// </summary>
        /// <param name="rightEntity"></param>
        public void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            
            var addEntityPath = AddEntityPatch.Allocate();
            addEntityPath.Key = rightEntity.EntityKey;
            _logger.DebugFormat("AddEntityPatch ::{0},",rightEntity.EntityKey);
            foreach (var comp in rightEntity.ComponentList)
            {
                AddComponentPatch patch = AddComponentPatch.Allocate(comp);
                addEntityPath.AddComponentPatch(patch);
	            patch.ReleaseReference();

			}
            _patch.AddEntityPatch(addEntityPath);
            addEntityPath.ReleaseReference();
        }

        /// <summary>
        /// 生成删除Entity的Patch
        /// </summary>
        /// <param name="leftEntity"></param>
        public void OnRightEntityMissing(IGameEntity leftEntity)
        {
            _logger.DebugFormat("DeleteEntityPatch ::{0},",leftEntity.EntityKey);
            EntityKey entityKey = leftEntity.EntityKey;
            var patch = DeleteEntityPatch.Allocate(entityKey);
            _patch.AddEntityPatch(patch);
            patch.ReleaseReference();
        }

       
        public void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            var patch = ModifyEntityPatch.Allocate(rightEntity);
            _currentEntityPatch = patch;
        }

        public void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
           if (_currentEntityPatch.GetComponentPatchCount() > 0)
           {
                _patch.AddEntityPatch(_currentEntityPatch);
               
           }
            //fix bug memory leak
            _currentEntityPatch.ReleaseReference();
            _currentEntityPatch = null;
        }

        public SnapshotPatch Detach()
        {
#pragma warning disable RefCounter001
            var rc =  _patch;
#pragma warning restore RefCounter001
            _patch = null;
            return rc;
        }

    }
}
