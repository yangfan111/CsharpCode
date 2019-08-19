using System;
using System.Collections.Generic;
using App.Shared;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Prediction;
using Core.Replicaton;
using Core.SpatialPartition;
using UnityEngine;

namespace Core.EntityComponent
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
            List<IGameEntity> preEntitys,
            bool isAccountStage,
            bool isPrepareStage,
            Action<IGameEntity, bool> OnInsertFun = null
            )
        {
            _preEntitys.Clear();
            _seq = seq;
            _filter.Init(self,position);
            _snapshot = Snapshot.Allocate();

            _accountStage = isAccountStage;
            _prepareStage = isPrepareStage;

            _OnInsertFun = OnInsertFun;

            forPre = true;

            foreach (var gameEntity in preEntitys)
            {
                DoInsert(gameEntity);
                _preEntitys.Add(gameEntity.EntityKey);
            }

            forPre = false;

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
        private bool _accountStage;        //只发送同队数据,用于结算场景
        private bool _prepareStage;        //只发送玩家数据，用于准备场景

        private bool forPre = false; 	   // 判断是否在向快照加入PreEntity
      
        private Action<IGameEntity, bool> _OnInsertFun = null;    //插入快照时的响应函数


        public void DoInsert(IGameEntity localEntity)
        {
            var entityKey = localEntity.EntityKey;
            var sendFilter = _filter;
            bool hasAdd = false;

            if (_preEntitys.Contains(entityKey))
            {
                hasAdd = true;
            }
            else if (sendFilter.IsIncludeEntity(localEntity))
            {
                if (sendFilter.IsEntitySelf(localEntity))
                {
                    _snapshot.AddEntity(localEntity.GetSelfEntityCopy(_seq));
                    hasAdd = true;
                }
                else if(!_accountStage)
                {
                    if (_prepareStage && localEntity.EntityType != (int) EEntityType.Player)
                        return;
                    if (localEntity.HasPositionFilter)
                    {
                        var positionFilter = localEntity.GetComponent<PositionFilterComponent>();
                        var position = localEntity.Position;
                        if (!positionFilter.Filter(position.Value, _filter.Position))
                        {
                            _snapshot.AddEntity(localEntity.GetNonSelfEntityCopy(_seq));
                            hasAdd = true;
                        }
                    }
                    else
                    {
                        _snapshot.AddEntity(localEntity.GetNonSelfEntityCopy(_seq));
                        hasAdd = true;
                    }
                }
            }

            if (hasAdd && _OnInsertFun != null)
            {
                _OnInsertFun(localEntity, forPre);
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
            _OnInsertFun = null;
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

            EntityMapDeepCloner.Clone(snapshot.EntityMap, _gameContexts.MyEntityMap,
                new PreSendSnapshotFilter(self, position));
            return snapshot;
        }


        public ISnapshot GeneratePerPlayerSnapshot(int seq, EntityKey self, Vector3 position, Bin2DConfig config,
            IBin2DManager bin, List<IGameEntity> preEntitys, bool isAccountStage, bool isPrepareStage)
        {
            var factory = ObjectAllocatorHolder<PartialSnapshotFactory>.Allocate();
            var ret = factory.GeneratePartialSnapshot(seq, self, position, config, bin, preEntitys, isAccountStage, isPrepareStage);
            factory.ReleaseReference();
            return ret;
        }

        /// <summary>
        /// 创建属于每个角色的可感知对象快照
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="self"></param>
        /// <param name="position"></param>
        /// <param name="config"></param>
        /// <param name="bin"></param>
        /// <param name="preEntitys"></param>
        /// <param name="isAccountStage"></param>
        /// <param name="isPrepareStage"></param>
        /// <param name="watchDict">上次残留对象的字典</param>
        /// <param name="OnInsertFun">插入到快照时的响应函数</param>
        /// <returns></returns>
        public ISnapshot GeneratePerPlayerSnapshot(int seq, EntityKey self, Vector3 position, Bin2DConfig config,
            IBin2DManager bin, List<IGameEntity> preEntitys, bool isAccountStage, bool isPrepareStage,
             Action<IGameEntity, bool> OnInsertFun)
        {
            var factory = ObjectAllocatorHolder<PartialSnapshotFactory>.Allocate();
            var ret = factory.GeneratePartialSnapshot(seq, self, position, config, bin, preEntitys, isAccountStage, isPrepareStage, OnInsertFun);
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