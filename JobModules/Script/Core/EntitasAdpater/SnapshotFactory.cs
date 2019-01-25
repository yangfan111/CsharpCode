using System;
using System.Collections.Generic;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Prediction;
using Core.Replicaton;
using Core.SpatialPartition;
using UnityEngine;

namespace Core.EntitasAdpater
{
    public class PartialSnapshotFactory : BaseRefCounter
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(PartialSnapshotFactory))
            {
            }

            public override object MakeObject()
            {
                return new PartialSnapshotFactory();
            }
        }

        public ISnapshot GeneratePartialSnapshot(int seq,
            EntityKey self,
            Vector3 position,
            Bin2DConfig config,
            IBin2DManager bin,
            List<IGameEntity> preEntitys)
        {
            _preEntitys.Clear();
            _seq = seq;
            _filter.Init(self,position);
            _snapshot = Snapshot.Allocate();

            foreach (var gameEntity in preEntitys)
            {
                DoInsert(gameEntity);
                _preEntitys.Add(gameEntity.EntityKey);
            }


            foreach (var bin2DState in bin.GetBin2Ds())
            {
                Rect rect = new Rect(position.x - bin2DState.VisibleRadius, position.z - bin2DState.VisibleRadius,
                    bin2DState.VisibleRadius * 2, bin2DState.VisibleRadius * 2);
                bin2DState.Retrieve(position, rect, _doInsert);
            }


            return _snapshot;
        }

        private int _seq;
        private ISnapshot _snapshot;
        private PreSendSnapshotFilter _filter;
        private HashSet<EntityKey> _preEntitys = new HashSet<EntityKey>(EntityKeyComparer.Instance);
        private Action<IGameEntity> _doInsert;

        public void DoInsert(IGameEntity localEntity)
        {
            var entityKey = localEntity.EntityKey;
            var sendFilter = _filter;
            if (_preEntitys.Contains(entityKey)) return;
            if (sendFilter.IsIncludeEntity(localEntity))
            {
                if (sendFilter.IsEntitySelf(localEntity))
                {
                    _snapshot.AddEntity(localEntity.GetSelfEntityCopy(_seq));
                }
                else
                {
                    if (localEntity.HasPositionFilter)
                    {
                        var positionFilter = localEntity.GetComponent<PositionFilterComponent>();
                        var position = localEntity.Position;
                        if (!positionFilter.Filter(position.Value, _filter.Position))
                        {
                            _snapshot.AddEntity(localEntity.GetNonSelfEntityCopy(_seq));
                        }
                    }
                    else
                    {
                        _snapshot.AddEntity(localEntity.GetNonSelfEntityCopy(_seq));
                    }
                }
            }
        }

        public PartialSnapshotFactory()
        {
            _doInsert = DoInsert;
            _filter = new PreSendSnapshotFilter(EntityKey.Default, Vector3.zero);
        }

        protected override void OnCleanUp()
        {
            _seq = -1;
            _snapshot = null;
            _filter.Init(EntityKey.Default, Vector3.zero);
            _preEntitys.Clear();
            ObjectAllocatorHolder<PartialSnapshotFactory>.Free(this);
        }
    }


    public class SnapshotFactory
    {
        private IGameContexts _gameContexts;
        CompensationFilter _compensationFilter = new CompensationFilter();

        public SnapshotFactory(IGameContexts gameContexts)
        {
            _gameContexts = gameContexts;
        }

        public ISnapshot GenerateSnapshot(EntityKey self, Vector3 position)
        {
            Snapshot snapshot = Snapshot.Allocate();

            EntityMapDeepCloner.Clone(snapshot.EntityMap, _gameContexts.EntityMap,
                new PreSendSnapshotFilter(self, position));
            return snapshot;
        }


        public ISnapshot GeneratePerPlayerSnapshot(int seq, EntityKey self, Vector3 position, Bin2DConfig config,
            IBin2DManager bin, List<IGameEntity> preEntitys)
        {
            var factory = ObjectAllocatorHolder<PartialSnapshotFactory>.Allocate();
            var ret = factory.GeneratePartialSnapshot(seq, self, position, config, bin, preEntitys);
            factory.ReleaseReference();
            return ret;
        }

        public ISnapshot GenerateCompensationSnapshot()
        {
            Snapshot snapshot = Snapshot.Allocate();

            EntityMapDeepCloner.Clone(snapshot.EntityMap, _gameContexts.CompensationEntityMap, _compensationFilter);
            return snapshot;
        }

        public static ISnapshot CloneSnapshot(ISnapshot src)
        {
            Snapshot snapshot = Replicaton.CloneSnapshot.Allocate();
            snapshot.Header = src.Header;
            EntityMapDeepCloner.Clone(snapshot.EntityMap, src.EntityMap, EntityComponent.DummyEntityMapFilter.Instance);
            return snapshot;
        }

        class CompensationFilter : IEntityMapFilter
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
    }
}