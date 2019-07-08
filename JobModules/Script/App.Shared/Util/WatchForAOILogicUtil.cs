using Core.EntitasAdpater;
using Core.EntityComponent;
using Entitas;
using System;
using System.Collections.Generic;


namespace App.Shared.Util
{
    public class WatchDict : IWatchDict
    {
        private Dictionary<EntityKey, WatchInfo> _dict = new Dictionary<EntityKey, WatchInfo>(EntityKeyComparer.Instance);
        private List<EntityKey> _removeList = new List<EntityKey>();

        /// <summary>
        /// 寄存了 原始PreEntity的对象 （对应的PreEntities列表则会追加上次的感知残留对象）
        /// </summary>
        public HashSet<EntityKey> preEntitys_HashSet
        {
            get
            {
                return _preEntitys_HashSet;
            }
        }

        public List<EntityKey> removeList
        {
            get {
                return _removeList;
            }
        }

        public object dict
        {
            get
            {
                return _dict;
            }
        }

        private HashSet<EntityKey> _preEntitys_HashSet = new HashSet<EntityKey>(EntityKeyComparer.Instance);

        public void Add(IGameEntity localEntity, IContexts _contexts)
        {
            Contexts contexts = _contexts as Contexts;
            int serverTime = contexts.session.currentTimeObject.CurrentTime;

            WatchInfo watchInfo = new WatchInfo();
            watchInfo.info = serverTime;

            _dict[localEntity.EntityKey] = watchInfo;  //进行信息更新
        }

        public void BuildEntitiesHashSet(List<IGameEntity> entities)
        {
            preEntitys_HashSet.Clear();
            foreach (var entity in entities)
            {
                preEntitys_HashSet.Add(entity.EntityKey);
            }
        }

        public void ClearDictByRemoveList()
        {
            foreach (var entityKey in removeList)
            {
                _dict.Remove(entityKey);
            }
        }
    }

    /// <summary>
    /// 监视信息
    /// </summary>
    public struct WatchInfo
    {
        public int info;
        /// <summary>
        /// 不符合 (newInfo - info) < threshold 则返回false
        /// </summary>
        /// <param name="newInfo"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CanStay(int newInfo, int threshold = 10000)
        {
            return (newInfo - info) < threshold;
        }

        public bool CanStay(IContexts _contexts)
        {
            int serverTime = (_contexts as Contexts).session.currentTimeObject.CurrentTime;
            return CanStay(serverTime);
        }
    }


    /// <summary>
    /// 用于进行感知范围内残留的处理
    /// </summary>
    public class WatchForAOILogicUtil
    {
        static IContexts _contexts;

        /// <summary>
        /// 通过记录的 <被检测对象, 检测时间> 字典 watchMap，来进行 preEntitys 的添加
        /// 这个函数是用来防止 角色可视对象 频繁修改造成的问题
        /// </summary>
        /// <param name="watchMap"></param>
        /// <param name="preEntitys">预存在于快照的Entity列表，用于将上次残留的entity存入其中</param>
        /// <param name="preEntitys_HashSet">preEntitys对应的EntityKeyHashSet</param>
        /// <param name="serverTime">当前的服务器时间</param>
        /// <param name="MaxDuration">允许残留对象存在的最长时间</param>
        /// <param name="_gameContexts"></param>
        /// UpdateWatchMap
        public static void UpdateWatchMap(IWatchDict watchMap,
            List<IGameEntity> preEntitys,
            IContexts _newContexts)
        {
            _contexts = _newContexts;

            watchMap.removeList.Clear();

            watchMap.BuildEntitiesHashSet(preEntitys);

            var _gameContexts = (_contexts as Contexts).session.commonSession.GameContexts;
            
            var dict = watchMap.dict as Dictionary<EntityKey, WatchInfo>;
            foreach (var keyValue in dict)
            {

                var entityKey = keyValue.Key;

                IGameEntity entity = null;
                if (! _gameContexts.TryGetGameEntity(entityKey, out entity))
                {
                    watchMap.removeList.Add(entityKey);
                    continue;
                }

                //---------------------------------------------------
                // 不符合条件的就标记去除
                if (!keyValue.Value.CanStay(_contexts))
                {
                    watchMap.removeList.Add(entityKey);
                    continue;
                }
                //---------------------------------------------------

                if (!watchMap.preEntitys_HashSet.Contains(entityKey))
                {
                    preEntitys.Add(entity);
                }
            }

            watchMap.ClearDictByRemoveList();

        }


        /// <summary>
        /// 根据entities 创建 entities_HashSet
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="entities_HashSet"></param>
        private void BuildEntitiesHashSet(List<IGameEntity> entities, HashSet<EntityKey> entities_HashSet)
        {
            entities_HashSet.Clear();
            foreach (var entity in entities)
            {
                entities_HashSet.Add(entity.EntityKey);
            }
        }


        /// <summary>
        /// 函数代理
        /// </summary>
        public static Action<IGameEntity, bool, IWatchDict> OnInsertFun
        {
            get
            {
                if (_OnInsertFun == null)
                {
                    _OnInsertFun = new Action<IGameEntity, bool, IWatchDict>(OnInsert);
                }
                return _OnInsertFun;
            }
        }

        private static Action<IGameEntity, bool, IWatchDict> _OnInsertFun;

        public static void OnInsert(IGameEntity localEntity, bool forPre, IWatchDict watchMap)
        {
            if (forPre && !watchMap.preEntitys_HashSet.Contains(localEntity.EntityKey))
            {
                // 若在处理PreEntity，却又没有包含在 preEntitys_HashSet 中，则说明是上次的残留对象，不进行 _watchMap.Add处理，直接返回。
                return;
            }

            watchMap.Add(localEntity, _contexts);
        }


    }
}
