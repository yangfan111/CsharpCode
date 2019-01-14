using System;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SyncLatest;
using UnityEngine;

namespace Core.Replicaton
{
    public class DummyEntityMapFilter : IEntityMapFilter
    {
        public DummyEntityMapFilter()
        {
        }

        public bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType)
        {
            return true;
        }

        public bool IsIncludeEntity(IGameEntity entity)
        {
            return true;
        }
    }

    public class PreSendSnapshotFilter : ISnapshotSerializeFilter
    {
        private EntityKey _self;
        private ReplicationFilter _filter;
        private Vector3 _position;

        public PreSendSnapshotFilter(EntityKey self, Vector3 position)
        {
            _self = self;
            _filter = new ReplicationFilter();
            _position = position;
        }

        public void Init(EntityKey self, Vector3 position)
        {
            _self = self;
            
            _position = position;
        }

        public bool IsIncludeEntity(IGameEntity entity)
        {
            bool isSync = (entity.IsSyncNonSelf ||
                           entity.IsSyncSelf) &&
                          !entity.IsDestroy;

            return isSync && _filter.IsSyncSelfOrThird(entity, _self);
        }

        public EntityKey Self
        {
            get { return _self; }
        }

        public Vector3 Position
        {
            get { return _position; }
        }

        public bool IsEntitySelf(IGameEntity entity)
        {
            return _filter.IsSelf;
        }


        public bool IsIncludeComponent(IGameEntity entity, IGameComponent component)
        {
            if (_filter.IsSelf)
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