    using System;
    using System.Collections.Generic;
using System.Linq;
using Core.EntitasAdpater;
using Core.Replicaton;
using Core.Utils;

namespace Core.EntityComponent
{
    public class GameContexts : IGameContexts
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(GameContexts));
        private Dictionary<int, IGameContext> _infos = new Dictionary<int, IGameContext>();
        private EntityMapCache _cache;
        private EntityMap _entityMap;
        public GameContexts()
        {
            _entityMap = EntityMap.Allocate();
            _cache = EntityMapCache.Allocate(_entityMap);
        }

        public void AddContextInfo(IGameContext basicInfo)
        {
            AddEntities(basicInfo);
            AssertUtility.Assert(!_infos.ContainsKey(basicInfo.EntityType));
            IEntitasGameContext nec = (IEntitasGameContext)basicInfo;
            foreach (var context in _infos.Values)
            {
                IEntitasGameContext ec = (IEntitasGameContext) context;
                AssertUtility.Assert(ec.ComponentTypes != nec.ComponentTypes);
                AssertUtility.Assert(ec.EntitasContext != nec.EntitasContext);
            }
            _infos[basicInfo.EntityType] = basicInfo;
        }

        private void AddEntities(IGameContext basicInfo)
        {
            var entities = basicInfo.GetEntities();
            foreach (var entity in entities)
            {
                _entityMap.Add(entity.EntityKey, entity);
            }

            basicInfo.EntityAdded += EntityAdded;
            basicInfo.EntityRemoved += EntityRemoved;
            basicInfo.EntityComponentChanged += EntityComponentChanged;

        }

        private void EntityAdded(IGameEntity entity)
        {
            try
            {
                _entityMap.Add(entity.EntityKey, entity);
                _cache.OnEntityAdded(entity);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("error {0}", e);
            }
        }
        private void EntityRemoved(IGameEntity entity)
        {
            try
            {
                _entityMap.Remove(entity.EntityKey);
                _cache.OnEntityRemoved(entity.EntityKey);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("error {0}", e);
            }
        }
        private void EntityComponentChanged(IGameEntity entity, int index)
        {
            _cache.OnEntityComponentChanged(entity,index);
        }


        public IGameEntity CreateAndGetGameEntity(EntityKey entityKey)
        {
            var info = _infos[entityKey.EntityType];
            
            return info.CreateAndGetEntity(entityKey);
        }

        public IGameEntity GetGameEntity(EntityKey entityKey)
        {
            var info = _infos[entityKey.EntityType];

            return info.GetEntity(entityKey);
        }


        public IGameContext[] AllContexts { get { return _infos.Values.ToArray(); } }

        
        public EntityMap EntityMap{get{return _entityMap;}}
        public EntityMap LatestEntityMap { get { return _cache.LatestEntityMap; } }
        public EntityMap SelfEntityMap { get { return _cache.SelfEntityMap; } }
        public EntityMap NonSelfEntityMap { get { return _cache.NonSelfEntityMap; } }
        public EntityMap CompensationEntityMap { get { return _cache.CompensationEntityMap; } }

        public EntityKey Self
        {
            get { return _cache.Self; }
            set
            {
                _cache.Self = value;
            }
        }
    }
}