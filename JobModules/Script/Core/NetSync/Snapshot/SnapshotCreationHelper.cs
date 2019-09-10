using System.Collections.Generic;
using App.Shared;
using Core.Components;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SpatialPartition;
using UnityEngine;

namespace Core.EntityComponent
{
    public delegate void SnapshotEntityInsert(IGameEntity gameEntity, bool forPre);

    /// <summary>
    ///     创建属于每个角色的可感知对象快照
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
    public class SnapshotCreationHelper : BaseRefCounter
    {
        private bool forPre;
        private bool isAccountStage;
        private bool isPrepareStage;
        private SnapshotEntityInsert onInsert;
        private HashSet<EntityKey> preEntityKeys = new HashSet<EntityKey>(EntityKeyComparer.Instance);
        private SendSnapshotFilter sendSnapshotFilter = new SendSnapshotFilter(EntityKey.Default, Vector3.zero);
        private int seq;
        private Snapshot snapshot;

        protected override void OnCleanUp()
        {
            sendSnapshotFilter.Init(EntityKey.Default, Vector3.zero);
            preEntityKeys.Clear();
            ObjectAllocatorHolder<SnapshotCreationHelper>.Free(this);
        }

        public ISnapshot GeneratePerPlayerSnapshot(int seq, EntityKey self, Vector3 position, Bin2DConfig config,
                                                   IBin2DManager bin, List<IGameEntity> preEntitys, bool isAccountStage,
                                                   bool isPrepareStage, SnapshotEntityInsert onInsert)
        {
            preEntityKeys.Clear();

            sendSnapshotFilter.Init(self, position);
            snapshot            = Snapshot.Allocate();
            forPre              = true;
            this.isPrepareStage = isPrepareStage;
            this.isAccountStage = isAccountStage;
            this.seq            = seq;
            this.onInsert = onInsert;
            foreach (var gameEntity in preEntitys)
            {
                DoInsert(gameEntity);
                preEntityKeys.Add(gameEntity.EntityKey);
            }

            forPre = false;

            foreach (var bin2DState in bin.GetBin2Ds())
            {
                Rect rect = new Rect(position.x - bin2DState.VisibleRadius, position.z - bin2DState.VisibleRadius,
                    bin2DState.VisibleRadius * 2, bin2DState.VisibleRadius * 2);
                bin2DState.Retrieve(position, rect, gameEntity => DoInsert(gameEntity));
            }

            return snapshot;
        }

        private void DoInsert(IGameEntity gameEntity)
        {
            var  entityKey = gameEntity.EntityKey;
            bool hasAdd    = false;

            if (preEntityKeys.Contains(entityKey))
            {
                hasAdd = true;
            }
            else if (sendSnapshotFilter.IsIncludeEntity(gameEntity))
            {
                if (sendSnapshotFilter.IsEntitySelf(gameEntity))
                {//获取自己的备份
                    snapshot.AddEntity(gameEntity.GetSelfEntityCopy(seq));
                    hasAdd = true;
                }
                else if (!isAccountStage)
                {
                    if (isPrepareStage && gameEntity.EntityType != (int) EEntityType.Player)
                        return;
                    //获取别人的备份
                    if (gameEntity.HasPositionFilter)
                    {
                        var positionFilter = gameEntity.GetComponent<PositionFilterComponent>();
                        var position       = gameEntity.Position;
                        if (!positionFilter.Filter(position.Value, sendSnapshotFilter.Position))
                        {
                            snapshot.AddEntity(gameEntity.GetNonSelfEntityCopy(seq));
                            hasAdd = true;
                        }
                    }
                    else
                    {
                        snapshot.AddEntity(gameEntity.GetNonSelfEntityCopy(seq));
                        hasAdd = true;
                    }
                }
            }

            if (hasAdd && onInsert != null)
            {
                onInsert(gameEntity, forPre);
            }
        }
    }
}