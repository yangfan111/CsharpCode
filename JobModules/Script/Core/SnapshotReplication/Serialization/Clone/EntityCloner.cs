using Core.Components;
using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.Utils;
using UnityEngine;

namespace Core.SnapshotReplication.Serialization.Clone
{
    public class EntityCloner
    {
      
        public static IGameEntity Clone(IGameEntity entity)
        {
            var entityCopy = GameEntity.Allocate(entity.EntityKey);
            foreach (var comp in entity.ComponentList)
            {
                
                var compCopy = entityCopy.AddComponent(comp.GetComponentId());
       
                //if (compCopy.GetComponentId() == (int)EComponentIds.WeaponBagSet)
                //{
                //  //  DebugUtil.LogInUnity("Create new:"+comp.ToString());
                //}
                (compCopy as INetworkObject).CopyFrom(comp);
            }
            return entityCopy;
        }
    }
}
