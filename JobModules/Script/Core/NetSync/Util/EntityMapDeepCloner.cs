using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.Replicaton;
using Core.Utils;
// ReSharper disable once PossibleNullReferenceException
namespace Core.Prediction
{


    public class EntityMapDeepCloner
    {
        public struct CloneAction
        {
            public CloneAction(EntityMap dstEntityMap, IEntityMapFilter filter)
            {
                this.dstEntityMap = dstEntityMap;
                this.filter = filter;
            }

            public readonly EntityMap dstEntityMap;
            public readonly IEntityMapFilter filter;

            public void Clone(IGameEntity localEntity)
            {

                var entityKey = localEntity.EntityKey;
                if (filter.IsIncludeEntity(localEntity))
                {
                    var dstEntity = CompensationGameEntity.Allocate(entityKey);
                    dstEntityMap.Add(entityKey, dstEntity);
                    dstEntity.ReleaseReference();
                    foreach (var component in localEntity.SortedComponentList)
                    {
                        if (filter.IsIncludeComponent(localEntity, component))
                        {
                            var remoteComponent = dstEntity.AddComponent(component.GetComponentId());
                            var comp = remoteComponent as ICloneableComponent;
                            comp.CopyFrom(component);
                        }
                    }
                }
            }
        }

        public static void Clone(EntityMap dstEntityMap, EntityMap srcEntityMap, IEntityMapFilter filter)
        {
            CloneAction action = new CloneAction(dstEntityMap, filter);
            foreach (var entity in srcEntityMap.Values)
            {
                action.Clone(entity);
            }
        }

        public static void Clone(ISnapshot dst, ISnapshot src, IEntityMapFilter filter)
        {
            CloneAction action = new CloneAction(dst.EntityMap, filter);
            src.ForeachGameEntity(action.Clone);
        }
    }
}