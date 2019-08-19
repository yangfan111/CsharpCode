using System;
using System.Collections.Generic;
using System.Linq;
using Core.EntityComponent;
using Core.Replicaton;
using Core.Utils;

namespace Core.EntityComponent
{
    public class GameContexts : IGameContexts
    {
        private static LoggerAdapter logger = new LoggerAdapter(typeof(GameContexts));
        private EntityMapCacheGroup cacheGroup;
        //改成Array
        private Dictionary<int, IGameContext> infos = new Dictionary<int, IGameContext>();

        public GameContexts()
        {
            MyEntityMap = EntityMap.Allocate();
            cacheGroup          = EntityMapCacheGroup.Allocate(MyEntityMap);
        }

        public EntityMap MyEntityMap { get; private set; }


        public IGameEntity CreateAndGetGameEntity(EntityKey entityKey)
        {
            return infos[entityKey.EntityType].CreateAndGetEntity(entityKey);
        }

        public IGameEntity GetGameEntity(EntityKey entityKey)
        {
            var info = infos[entityKey.EntityType];
            if (info == null)
            {
                AssertUtility.Assert(false, "infos[" + entityKey.EntityType + "] == null");
                return null;
            }

            return info.GetEntity(entityKey);
        }
#pragma warning disable RefCounter002
        public bool TryGetGameEntity(EntityKey entityKey, out IGameEntity entity)
#pragma warning restore RefCounter002
        {
            entity = null;

            var info = infos[entityKey.EntityType];
            if (info == null)
            {
                AssertUtility.Assert(false, "infos[" + entityKey.EntityType + "] == null");
                return false;
            }

            return info.TryGetEntity(entityKey, out entity);
        }

        public IGameContext[] AllContexts
        {
            get { return infos.Values.ToArray(); }
        }


        public EntityMap LatestEntityMap
        {
            get { return cacheGroup.LatestEntityMap; }
        }

        public EntityMap SelfEntityMap
        {
            get { return cacheGroup.SelfEntityMap; }
        }

        public EntityMap NonSelfEntityMap
        {
            get { return cacheGroup.NonSelfEntityMap; }
        }

        public EntityMap CompensationEntityMap
        {
            get { return cacheGroup.CompensationEntityMap; }
        }

        public EntityKey Self
        {
            get { return cacheGroup.MyKey; }
            set { cacheGroup.MyKey = value; }
        }

        public void AddContextEle(IGameContext basicInfo)
        {
            AddEntities(basicInfo);
            AssertUtility.Assert(!infos.ContainsKey(basicInfo.EntityType));
            IEntitasGameContext nec = (IEntitasGameContext) basicInfo;
            foreach (var context in infos.Values)
            {
                IEntitasGameContext ec = (IEntitasGameContext) context;
                AssertUtility.Assert(ec.ComponentTypes != nec.ComponentTypes);
                AssertUtility.Assert(ec.EntitasContext != nec.EntitasContext);
            }

            infos[basicInfo.EntityType] = basicInfo;
        }

        private void AddEntities(IGameContext basicInfo)
        {
            var entities = basicInfo.GetEntities();
            foreach (var entity in entities)
                MyEntityMap.Add(entity.EntityKey, entity);

            basicInfo.EntityAdded            += EntityAdded;
            basicInfo.EntityRemoved          += EntityDestroy;
            basicInfo.EntityComponentChanged += EntityComponentChanged;
        }

        private void EntityAdded(IGameEntity entity)
        {
            try
            {
                MyEntityMap.Add(entity.EntityKey, entity);
                cacheGroup.OnEntityAdded(entity);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("error {0}", e);
            }
        }

        private void EntityDestroy(IGameEntity entity)
        {
            try
            {
                MyEntityMap.Remove(entity.EntityKey);
                cacheGroup.OnEntityRemoved(entity.EntityKey);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("error {0}", e);
            }
        }

        private void EntityComponentChanged(IGameEntity entity, int index)
        {
            cacheGroup.OnEntityComponentChanged(entity, index);
        }
    }
}