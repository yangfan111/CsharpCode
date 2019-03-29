using System;
using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.SpatialPartition;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace Core.EntitasAdpater
{
    public interface IEntitasGameContext
    {
        Type[] ComponentTypes { get; }
        IContext EntitasContext { get; }
    }

    public abstract class EntitasGameContext<TEntity> : IGameContext, IEntitasGameContext where TEntity : Entity
    {
        private static LoggerAdapter _logger = new LoggerAdapter("EntitasGameContext");
        private Context<TEntity> _context;

        private ComponentIndexLookUp<TEntity> _indexLookUp;
        private int _entityKeyIndex;
        private int _ownerIdIndex;
        private int _positionIndex;
        private Bin2D<IGameEntity> _bin;
        private readonly EntityComponentChanged _onOnComponentAddedCache;
        private readonly EntityComponentChanged _onOnComponentRemovedCache;
        private readonly EntityComponentReplaced _onOnComponentReplacedCache;
        private readonly HashSet<int> _needChangeCacheIndexs = new HashSet<int>();
        protected EntitasGameContext(Context<TEntity> context, Type[] componentTypes, Bin2D<IGameEntity> bin)
        {
            _bin = bin;
            _context = context;
            _indexLookUp = new ComponentIndexLookUp<TEntity>(componentTypes);
            _entityKeyIndex = _indexLookUp.GetComponentIndex<EntityKeyComponent>();
            _ownerIdIndex = _indexLookUp.GetComponentIndex<OwnerIdComponent>();
            _positionIndex = _indexLookUp.GetComponentIndex<PositionComponent>();
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<PositionComponent>());
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<EntityKeyComponent>());
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<OwnerIdComponent>());
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<FlagCompensationComponent>());
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<FlagDestroyComponent>());
           
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<FlagSelfComponent>());
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<FlagSyncSelfComponent>());
            _needChangeCacheIndexs.Add(_indexLookUp.GetComponentIndex<FlagSyncNonSelfComponent>());
            
            _context.OnEntityCreated += ContextOnOnEntityCreated;
            _context.OnEntityWillBeDestroyed += ContextOnOnEntityDestroyed;
            _onOnComponentAddedCache = OnOnComponentAdded;
            _onOnComponentRemovedCache = OnOnComponentRemoved;
            _onOnComponentReplacedCache = OnOnComponentReplaced;
        }


        private void ContextOnOnEntityDestroyed(IContext context, IEntity entity1)
        {
            var entity = (TEntity) entity1;
            entity.RemoveOnComponentAdded(_onOnComponentAddedCache);
            entity.RemoveOnComponentRemoved(_onOnComponentRemovedCache);
            entity.RemoveOnComponentReplaced(_onOnComponentReplacedCache);
            if (EntityRemoved != null)
            {
                var entityKeyComp = EntitasGameEntity<TEntity>.DoGetComponent(entity,
                    _indexLookUp.GetComponentIndex<EntityKeyComponent>());
                if (entityKeyComp != null)
                    EntityRemoved(GetGameEntity(entity));
                var posComp = EntitasGameEntity<TEntity>.DoGetComponent(entity,
                    _indexLookUp.GetComponentIndex<PositionComponent>());
                if (posComp != null)
                    NotifyComponentChanged(entity1, _positionIndex, posComp, null);
            }
        }

        private void ContextOnOnEntityCreated(IContext context, IEntity entity1)
        {
            var entity = (TEntity) entity1;
            entity.AddOnComponentAdded(_onOnComponentAddedCache);
            entity.AddOnComponentRemoved(_onOnComponentRemovedCache);
            entity.AddOnComponentReplaced(_onOnComponentReplacedCache);
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
                _logger.ErrorFormat("error {0}", e);
            }
        }

        private void OnOnComponentAdded(IEntity entity, int index, IComponent component1)
        {
            try
            {
                if (index == _entityKeyIndex)
                {
                    AssertUtility.Assert(((EntityKeyComponent)component1).Value.EntityType == EntityType);
                    if (EntityAdded != null)
                    {
                        EntityAdded(GetGameEntity((TEntity) entity));
                    }
                }
                else
                {
                    if (index == _positionIndex)
                    {
                        AssertUtility.Assert(entity.HasComponent(_entityKeyIndex),
                            "EntityKeyComponent must be added before PositionComponent");
                    }

                    NotifyComponentChanged(entity, index, null, component1);
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("error {0},{1},{2}", entity.GetType(), index, e);
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
                _logger.ErrorFormat("error {0},{1},{2}", entity1.GetType(), index, e);
            }
        }

        private void NotifyComponentChanged(IEntity entity, int index, IComponent oldComp, IComponent newComp)
        {
            if (EntityComponentChanged != null && entity.HasComponent(_entityKeyIndex) )
            {
                if( _needChangeCacheIndexs.Contains(index))
                    EntityComponentChanged(GetGameEntity((TEntity) entity), index);
            }

            HandlePositionComponent(entity, index, oldComp, newComp);
        }

        void HandlePositionComponent(IEntity entity1, int index, IComponent oldComp, IComponent newComp)
        {
          
            if (index == _positionIndex)
            {
                var gameEntity = GetGameEntity((TEntity) entity1);
                var oldPos = oldComp as PositionComponent;
                if (oldPos != null)
                {
                    if (_bin != null)
                        _bin.Remove(gameEntity, oldPos.Value.To2D());
                    oldPos.RemovePositionListener(OnPositionChanged);
                    oldPos.CleanOwner();
                }

                var newPos = newComp as PositionComponent;
                if (newPos != null)
                {
                    newPos.AddPositionListener(OnPositionChanged);
                    newPos.SetOwner(gameEntity);
                    if (_bin != null)
                        _bin.Insert(gameEntity, newPos.Value.To2D());
                }
            }
        }

        public void OnPositionChanged(IGameEntity owner, Vector3 oldPos, Vector3 newPos)
        {
            if (_bin != null)
            {
                _bin.Update(owner, oldPos.To2D(), newPos.To2D());
            }
        }


        public int GetComponentIndex<TComponent>() where TComponent : IGameComponent
        {
            return _indexLookUp.GetComponentIndex<TComponent>();
        }

        public event EntityRemoved EntityAdded;
        public event EntityChanged EntityComponentChanged;
        public event EntityRemoved EntityRemoved;
        private List<IGameEntity> _listAllEntities = new List<IGameEntity>();

        public List<IGameEntity> GetEntities()
        {
            _listAllEntities.Clear();
            GetEntities(_context.GetEntities(), _listAllEntities);
            return _listAllEntities;
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


        private IGameEntity GetGameEntity(TEntity entity)
        {
            var comp = GetOrAddGameEntityComponent(entity);
            if (comp.SelfAdapter == null)
            {
                comp.SelfAdapter = EntitasGameEntity<TEntity>.Allocate(entity, _indexLookUp);
            }

            return comp.SelfAdapter;
        }

        private EntityAdapterComponent GetOrAddGameEntityComponent(TEntity entity)
        {
            int index = _indexLookUp.GetComponentIndex<EntityAdapterComponent>();
            EntityAdapterComponent comp =
                (EntityAdapterComponent) EntitasGameEntity<TEntity>.DoGetComponent(entity, index);
            if (comp == null)
            {
                comp = EntitasGameEntity<TEntity>.DoAddComponent<EntityAdapterComponent>(entity, index);
                comp.SelfAdapter = null;
            }

            return comp;
        }

        protected abstract TEntity GetEntityWithEntityKey(EntityKey entitykey);


        public abstract short EntityType { get; }

        public IGameEntity CreateAndGetEntity(EntityKey entitykey)
        {
            TEntity rc = GetEntityWithEntityKey(entitykey);
            if (rc == null)
            {
                rc = _context.CreateEntity();
                var index = _indexLookUp.GetComponentIndex<EntityKeyComponent>();
                var component = rc.CreateComponent<EntityKeyComponent>(index);
                component.Value = entitykey;
                rc.AddComponent(index, component);
            }

            IGameEntity rrc = GetWrapped(rc);
            if (rrc.IsDestroy)
            {
                rrc.RemoveComponent<FlagDestroyComponent>();
            }

            return rrc;
        }

        public IGameEntity GetEntity(EntityKey entityKey)
        {
            TEntity rc = GetEntityWithEntityKey(entityKey);
            return GetWrapped(rc);
        }

        public EntitasGameEntity<TEntity> GetWrapped(TEntity rc)
        {
            return (EntitasGameEntity<TEntity>) GetGameEntity(rc);
        }

        public bool CanContainComponent<TComponent>() where TComponent : IGameComponent
        {
            return _indexLookUp.GetComponentIndex<TComponent>() >= 0;
        }


        public IGameGroup CreateGameGroup<TComponent>() where TComponent : IGameComponent
        {
            var entities = _context.GetGroup(Matcher<TEntity>.AllOf(GetComponentIndex<TComponent>()));
            return new GameGroup<TEntity>(entities, this);
        }

        public Type[] ComponentTypes
        {
            get { return _indexLookUp.AllTypesByEntitasIndex; }
        }

        public IContext EntitasContext
        {
            get { return _context; }
        }
    }
}