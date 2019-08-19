using System;
using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.SpatialPartition;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace Core.EntityComponent
{
    public interface IEntitasGameContext
    {
        Type[]   ComponentTypes { get; }
        IContext EntitasContext { get; }
    }

    /// <summary>
    ///     持有GameComponentIndexLookUp,持有Bin2D
    ///     构造GameEntity
    ///     为GameContexts更新GameComponents提供事件订阅（通过订阅Context的Entity相关事件）
    ///     监听PositionComponent，同步更新Bin2D<IGameEntity> 
    ///     EntityKeyComponent,EntityAdapterComponent,FlagDestroyComponent这三个component必定包含
    ///     IGameEntity是EntitySelfAdapter.Value
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntitasGameContext<TEntity> : IGameContext, IEntitasGameContext where TEntity : Entity
    {
        private static LoggerAdapter logger = new LoggerAdapter("EntitasGameContext");

        //   private readonly HashSet<int> needChangeCacheIndexs = new HashSet<int>();
        private readonly EntityComponentChanged onOnComponentAddedCache;
        private readonly EntityComponentChanged onOnComponentRemovedCache;
        private readonly EntityComponentReplaced onOnComponentReplacedCache;
        private Bin2D<IGameEntity> bin;

        private Context<TEntity> entitasContext;
        //  private int entityKeyIndex;

        private GameComponentIndexLookUp<TEntity> indexLookUp;

        private List<IGameEntity> listAllEntities = new List<IGameEntity>();
        // private int ownerIdIndex;
        // private int positionIndex;

        protected EntitasGameContext(Context<TEntity> context, Type[] componentTypes, Bin2D<IGameEntity> bin)
        {
            this.bin                        =  bin;
            entitasContext                  =  context;
            indexLookUp                     =  new GameComponentIndexLookUp<TEntity>(componentTypes);
            context.OnEntityCreated         += ContextOnOnEntityCreated;
            context.OnEntityWillBeDestroyed += ContextOnOnEntityDestroyed;
            onOnComponentAddedCache         =  OnOnComponentAdded;
            onOnComponentRemovedCache       =  OnOnComponentRemoved;
            onOnComponentReplacedCache      =  OnOnComponentReplaced;
        }

        public IContext EntitasContext
        {
            get { return entitasContext; }
        }

        public Type[] ComponentTypes
        {
            get { return indexLookUp.AllTypesByEntitasIndex; }
        }


        //从Contexts订阅此消息，返回IGameEntity（只有通用组件添加之后才有）
        public event EntityRemoved EntityAdded;
        public event EntityChanged EntityComponentChanged;
        public event EntityRemoved EntityRemoved;

        public List<IGameEntity> GetEntities()
        {
            listAllEntities.Clear();
            GetEntities(entitasContext.GetEntities(), listAllEntities);
            return listAllEntities;
        }


        public abstract short EntityType { get; }


        public IGameEntity CreateAndGetEntity(EntityKey entitykey)
        {
            TEntity rc = GetEntityWithEntityKey(entitykey);
            if (rc == null)
            {
                rc = entitasContext.CreateEntity();
                var index     = indexLookUp.EntityKeyComponentIndex;
                var component = rc.CreateComponent<EntityKeyComponent>(index);
                component.Value = entitykey;
                rc.AddComponent(index, component);
            }

            IGameEntity gameEntity = GetWrapped(rc);
            if (gameEntity.IsDestroy)
            {
                gameEntity.RemoveComponent<FlagDestroyComponent>();
            }

            return gameEntity;
        }

#pragma warning disable RefCounter002
        public bool TryGetEntity(EntityKey entityKey, out IGameEntity entity)
#pragma warning restore RefCounter002
        {
            entity = null;

            TEntity rc = GetEntityWithEntityKey(entityKey);
            if (rc == null)
            {
                // 就目前而言 GetEntityWithEntityKey 找不到 entityKey 对应的entity，则返回null
                return false;
            }

            entity = GetWrapped(rc);

            return entity != null;
        }

        public IGameEntity GetEntity(EntityKey entityKey)
        {
            TEntity rc = GetEntityWithEntityKey(entityKey);
            return GetWrapped(rc);
        }

        public bool CanContainComponent<TComponent>() where TComponent : IGameComponent
        {
            return indexLookUp.GetComponentIndex<TComponent>() >= 0;
        }

        /// <summary>
        ///     创建一个Group，并赋值给GameGroup
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public IGameGroup CreateGameGroup<TComponent>() where TComponent : IGameComponent
        {
            IGroup<TEntity> entities = entitasContext.GetGroup(Matcher<TEntity>.AllOf(GetComponentIndex<TComponent>()));
            return new GameGroup<TEntity>(entities, this);
        }

        public int GetComponentIndex<TComponent>() where TComponent : IGameComponent
        {
            return indexLookUp.GetComponentIndex<TComponent>();
        }

        public void GetEntities(TEntity[] entities, List<IGameEntity> results)
        {
            foreach (var entity in entities)
            {
#pragma warning disable RefCounter001
                var comp = GetGameEntity(entity);
#pragma warning restore RefCounter001
                results.Add(comp);
            }
        }

        //单例获取IGameEntity =>comp.SelfAdapter
        private IGameEntity GetGameEntity(TEntity entity)
        {
            var comp = GetOrAddGameEntityComponent(entity);

            if (comp.SelfAdapter == null)
            {
                comp.SelfAdapter = EntitasGameEntity<TEntity>.Allocate(entity, indexLookUp);
            }

            return comp.SelfAdapter;
        }

        private EntityAdapterComponent GetOrAddGameEntityComponent(TEntity entity)
        {
            int index = indexLookUp.EntityAdapterComponentIndex;
            EntityAdapterComponent comp =
                            (EntityAdapterComponent) EntitasGameEntity<TEntity>.DoGetComponent(entity, index);

            if (comp == null)
            {
                comp             = EntitasGameEntity<TEntity>.DoAddComponent<EntityAdapterComponent>(entity, index);
                comp.SelfAdapter = null;
            }

            return comp;
        }

        protected abstract TEntity GetEntityWithEntityKey(EntityKey entitykey);

        public EntitasGameEntity<TEntity> GetWrapped(TEntity rc)
        {
            return (EntitasGameEntity<TEntity>) GetGameEntity(rc);
        }

        #region unfinished

        private void ContextOnOnEntityDestroyed(IContext context, IEntity entity1)
        {
            var entity = (TEntity) entity1;
            entity.RemoveOnComponentAdded(onOnComponentAddedCache);
            entity.RemoveOnComponentRemoved(onOnComponentRemovedCache);
            entity.RemoveOnComponentReplaced(onOnComponentReplacedCache);
            if (EntityRemoved != null)
            {
                var entityKeyComp =
                                EntitasGameEntity<TEntity>.DoGetComponent(entity, indexLookUp.EntityKeyComponentIndex);
                if (entityKeyComp != null)
                    EntityRemoved(GetGameEntity(entity));
                var posComp = EntitasGameEntity<TEntity>.DoGetComponent(entity, indexLookUp.PositionComponentIndex);
                if (posComp != null)
                    NotifyComponentChanged(entity1, indexLookUp.PositionComponentIndex, posComp, null);
            }
        }

        private void ContextOnOnEntityCreated(IContext context, IEntity entity1)
        {
            var entity = (TEntity) entity1;
            entity.AddOnComponentAdded(onOnComponentAddedCache);
            entity.AddOnComponentRemoved(onOnComponentRemovedCache);
            entity.AddOnComponentReplaced(onOnComponentReplacedCache);
        }


        private void OnOnComponentReplaced(IEntity entity1, int index, IComponent previousComponent,
                                           IComponent newComponent)
        {
            try
            {
                NotifyComponentChanged(entity1, index, previousComponent, newComponent);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("error {0}", e);
            }
        }

        //监听AddComponent，每生产一个EntityKey就执行一个GetOrAddGameEntityComponent
        private void OnOnComponentAdded(IEntity entity, int index, IComponent component1)
        {
            try
            {
                if (index == indexLookUp.EntityKeyComponentIndex)
                {
                    AssertUtility.Assert(((EntityKeyComponent) component1).Value.EntityType == EntityType);
                    if (EntityAdded != null)
                    {
                        EntityAdded(GetGameEntity((TEntity) entity));
                    }
                }
                else
                {
                    if (index == indexLookUp.PositionComponentIndex)
                    {
                        AssertUtility.Assert(entity.HasComponent(indexLookUp.EntityKeyComponentIndex),
                            "EntityKeyComponent must be added before PositionComponent");
                    }

                    NotifyComponentChanged(entity, index, null, component1);
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("error {0},{1},{2}", entity.GetType(), index, e);
            }
        }

        private void OnOnComponentRemoved(IEntity entity1, int index, IComponent component1)
        {
            try
            {
                NotifyComponentChanged(entity1, index, component1, null);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("error {0},{1},{2}", entity1.GetType(), index, e);
            }
        }

        private void NotifyComponentChanged(IEntity entity, int index, IComponent oldComp, IComponent newComp)
        {
            if (EntityComponentChanged != null && entity.HasComponent(indexLookUp.EntityKeyComponentIndex))
            {
                if (indexLookUp.NeedNotifyIndexs.Contains(index))
                    EntityComponentChanged(GetGameEntity((TEntity) entity), index);
            }

            HandlePositionComponent(entity, index, oldComp, newComp);
        }

        //GameContext订阅位置信息，同步更新Bin2D<IEntity>
        void HandlePositionComponent(IEntity entity1, int index, IComponent oldComp, IComponent newComp)
        {
            if (index == indexLookUp.PositionComponentIndex)
                return;
            IGameEntity gameEntity = GetGameEntity((TEntity) entity1);
            var         oldPos     = oldComp as PositionComponent;
            if (oldPos != null)
            {
                if (bin != null)
                    bin.Remove(gameEntity, oldPos.Value.To2D());
                oldPos.RemovePositionListener(OnPositionChanged);
                oldPos.CleanOwner();
            }

            var newPos = newComp as PositionComponent;
            if (newPos != null)
            {
                newPos.AddPositionListener(OnPositionChanged);
                newPos.SetOwner(gameEntity);
                if (bin != null)
                    bin.Insert(gameEntity, newPos.Value.To2D());
            }
        }

        //同步更新Bin2D<IEntity>
        public void OnPositionChanged(IGameEntity owner, Vector3 oldPos, Vector3 newPos)
        {
            if (bin != null)
            {
                bin.Update(owner, oldPos.To2D(), newPos.To2D());
            }
        }

        #endregion
    }
}