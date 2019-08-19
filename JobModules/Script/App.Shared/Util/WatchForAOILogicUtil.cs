using Core.EntityComponent;
using Core.EntityComponent;
using Entitas;
using System;
using System.Collections.Generic;

namespace App.Shared.Util
{
    public class WatchDict : IWatchDict
    {
        public WatchDict(EntityKey playerEntityKey)
        {
            this.playerEntityKey = playerEntityKey;
        }

        /// <summary>
        /// 用于记录所属的角色
        /// </summary>
        public EntityKey playerEntityKey;

        private static readonly Core.Utils.LoggerAdapter _logger = new Core.Utils.LoggerAdapter(typeof(WatchDict));

        private Dictionary<EntityKey, WatchInfo> _dict = new Dictionary<EntityKey, WatchInfo>(EntityKeyComparer.Instance);
        private List<EntityKey> _removeList = new List<EntityKey>(40);

        private IContexts _contexts;
        private IGameContexts _gameContexts = null;

        /// <summary>
        /// 用于在 Add 函数中判断 是否加入的entity是 上次残留的对象
        /// </summary>
        private HashSet<EntityKey> _preEntitys_HashSet = new HashSet<EntityKey>(EntityKeyComparer.Instance);


        public void Update(List<IGameEntity> preEntitys, IContexts _newContexts)
        {
            try
            {
                _contexts = _newContexts;

                _removeList.Clear();

                // _preEntitys_HashSet 进行填充，用于在 Add 函数中判断 是否加入的entity是 上次残留的对象
                _preEntitys_HashSet.Clear();
                foreach (var entity in preEntitys)
                {
                    _preEntitys_HashSet.Add(entity.EntityKey);
                }

                // 获取 _gameContexts 
                if (_gameContexts == null)
                {
                    _gameContexts = (_newContexts as Contexts).session.commonSession.GameContexts;
                }

                // 遍历残留字典
                foreach (var keyValue in _dict)
                {
                    var entityKey = keyValue.Key;

                    IGameEntity entity = null;
                    //---------------------------------------------------
                    // 找不到entityKey对应的entity，就标记去除
                    if (!_gameContexts.TryGetGameEntity(entityKey, out entity))
                    {
                        _removeList.Add(entityKey);
                        continue;
                    }

                    //---------------------------------------------------
                    // 不符合条件的就标记去除
                    if (!keyValue.Value.CanStay(_contexts))
                    {
                        _removeList.Add(entityKey);
                        continue;
                    }
                    //---------------------------------------------------

                    preEntitys.Add(entity);
                }

                // -----清除被标记的key-------------------------------
                foreach (var entityKey in _removeList)
                {
                    _dict.Remove(entityKey);
                }
            }
            catch(Exception e)
            {
                _logger.ErrorFormat("当前线程为 {0},异常为：{1}", System.Threading.Thread.CurrentThread.Name, e.Message+"\n"+e.StackTrace);
            }
        }

        public void Add(IGameEntity localEntity)
        {
            Contexts contexts = _contexts as Contexts;
            int serverTime = contexts.session.currentTimeObject.CurrentTime;

            WatchInfo watchInfo = new WatchInfo();
            watchInfo.info = serverTime;

            _dict[localEntity.EntityKey] = watchInfo;  //进行信息更新
        }


        public void OnInsert(IGameEntity localEntity, bool forPre)
        {
            if (forPre && !_preEntitys_HashSet.Contains(localEntity.EntityKey))
            {
                // 若在处理PreEntity，且又没有包含在 preEntitys_HashSet 中，则说明是上次的残留对象，不进行 _watchMap.Add处理，直接返回。
                return;
            }

            Add(localEntity);
        }


        /// <summary>
        /// 函数代理
        /// </summary>
        public Action<IGameEntity, bool> OnInsertFun
        {
            get
            {
                if (_OnInsertFun == null)
                {
                    _OnInsertFun = new Action<IGameEntity, bool>(OnInsert);
                }
                return _OnInsertFun;
            }
        }

        private Action<IGameEntity, bool> _OnInsertFun;
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

}
