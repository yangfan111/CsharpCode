using System;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SyncLatest;
using UnityEngine;

namespace Core.Replicaton
{
    public class CompensationFilter : IEntityMapFilter
    {
        public bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType)
        {
            return componentType is ICompensationComponent;
        }

        public bool IsIncludeEntity(IGameEntity entity)
        {
            return (entity.IsCompensation && !entity.IsDestroy);
        }
    }
    public struct DummyEntityMapFilter : IEntityMapFilter
    {
       

        public bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType)
        {
            return true;
        }

        public bool IsIncludeEntity(IGameEntity entity)
        {
            return true;
        }
    }

    public struct SendSnapshotFilter : ISnapshotSerializeFilter
    {
        private EntityKey self;
        private ReplicationFilter filter;
        private Vector3 position;

        public SendSnapshotFilter(EntityKey self, Vector3 position)
        {
            this.self = self;
            filter = new ReplicationFilter();
            this.position = position;
        }

        public void Init(EntityKey self, Vector3 position)
        {
            this.self = self;
            
            this.position = position;
        }

        public bool IsIncludeEntity(IGameEntity entity)
        {
            bool isSync = (entity.IsSyncNonSelf ||
                           entity.IsSyncSelf) &&
                          !entity.IsDestroy;

            return isSync && filter.IsSyncSelfOrThird(entity, self);
        }

        public EntityKey Self
        {
            get { return self; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public bool IsEntitySelf(IGameEntity entity)
        {
            return filter.IsSelf;
        }


        public bool IsIncludeComponent(IGameEntity entity, IGameComponent component)
        {
            if (filter.IsSelf)
            {
                return component is IPredictionComponent || component is ISelfLatestComponent;
            }
            else
            {
                return component is IPlaybackComponent || component is INonSelfLatestComponent;
            }
        }
    }
}