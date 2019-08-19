using System.Collections;
using Core.Components;
using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Patch
{
    public class SnapshotPatchGenerator : EntityMapDiffHandlerAdapter
    {
        private static LoggerAdapter logger = new LoggerAdapter(typeof(SnapshotPatchGenerator));
        private SnapshotPatch snapshotPatch;
        private AbstractEntityPatch currentEntityPatch;
        private INetworkObjectSerializerManager serializerManager;
        public SnapshotPatchGenerator(INetworkObjectSerializerManager serializerManager)
        {
            snapshotPatch = SnapshotPatch.Allocate();
            this.serializerManager = serializerManager;
        }

        public override bool IsExcludeComponent(IGameComponent component)
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
        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            var serializer = serializerManager.GetSerializer(leftComponent.GetComponentId());
            var bitMask = serializer.DiffNetworkObject(leftComponent as INetworkObject, rightComponent as INetworkObject);
          
            if (bitMask.HasValue())
            {
                var componentPatch = ModifyComponentPatch.Allocate(leftComponent, rightComponent, bitMask);
                currentEntityPatch.AddComponentPatch(componentPatch);
                componentPatch.ReleaseReference();
            }        
            
            
        }

        /// <summary>
        /// 生成新加Component的Patch
        /// </summary>
        /// <param name="rightEntity"></param>
        /// <param name="leftEntity"></param>
        /// <param name="rightComponent"></param>
        public override void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            logger.DebugFormat("AddComponentPatch :{0}, {1}",leftEntity.EntityKey, rightComponent.GetComponentId());
            var componentPatch = AddComponentPatch.Allocate(rightComponent);
            currentEntityPatch.AddComponentPatch(componentPatch);
            componentPatch.ReleaseReference();
        }

        /// <summary>
        /// 在Patch中生成删除Entity的Patch
        /// </summary>
        /// <param name="leftEntity"></param>
        /// <param name="rightEntity"></param>
        /// <param name="leftComponent"></param>
        public override void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            logger.DebugFormat("DeleteComponentPatch ::{0}, {1}",leftEntity.EntityKey,  leftComponent.GetComponentId());
            var componentPatch = DeleteComponentPatch.Allocate(leftComponent);
            currentEntityPatch.AddComponentPatch(componentPatch);
            componentPatch.ReleaseReference();
        }

        /// <summary>
        /// 在Patch中生成新加Entity的信息
        /// </summary>
        /// <param name="rightEntity"></param>
        public override void OnLeftEntityMissing(IGameEntity rightEntity)
        {
            
            var addEntityPath = AddEntityPatch.Allocate();
            addEntityPath.Key = rightEntity.EntityKey;
            logger.DebugFormat("AddEntityPatch ::{0},",rightEntity.EntityKey);
            foreach (var comp in rightEntity.ComponentList)
            {
                AddComponentPatch patch = AddComponentPatch.Allocate(comp);
                addEntityPath.AddComponentPatch(patch);
	            patch.ReleaseReference();

			}
            snapshotPatch.AddEntityPatch(addEntityPath);
            addEntityPath.ReleaseReference();
        }

        /// <summary>
        /// 生成删除Entity的Patch
        /// </summary>
        /// <param name="leftEntity"></param>
        public override void OnRightEntityMissing(IGameEntity leftEntity)
        {
            logger.DebugFormat("DeleteEntityPatch ::{0},",leftEntity.EntityKey);
            EntityKey entityKey = leftEntity.EntityKey;
            var patch = DeleteEntityPatch.Allocate(entityKey);
            snapshotPatch.AddEntityPatch(patch);
            patch.ReleaseReference();
        }

       
        public override void DoDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            var patch = ModifyEntityPatch.Allocate(rightEntity);
            currentEntityPatch = patch;
        }

        public override void DoDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
           if (currentEntityPatch.GetComponentPatchCount() > 0)
           {
                snapshotPatch.AddEntityPatch(currentEntityPatch);
               
           }
            //fix bug memory leak
            currentEntityPatch.ReleaseReference();
            currentEntityPatch = null;
        }

        public SnapshotPatch Detach()
        {
#pragma warning disable RefCounter001
            var rc =  snapshotPatch;
#pragma warning restore RefCounter001
            snapshotPatch = null;
            return rc;
        }

    }
}
