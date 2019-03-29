using System.Collections.Generic;
using Core.EntityComponent;
using Entitas;

namespace Core.EntitasAdpater
{
    public class GameGroup<TEntity> :IGameGroup where TEntity : Entity
    {
        public GameGroup(IGroup<TEntity> entitasGroup, EntitasGameContext<TEntity> context)
        {
            _entitasGroup = entitasGroup;
            _context = context;
        }

        private IGroup<TEntity> _entitasGroup;
        private EntitasGameContext<TEntity> _context;
        private List<IGameEntity> _gameEntitiesCache = new List<IGameEntity>();
        private TEntity[] _entitiesCache;
        public List<IGameEntity> GetGameEntities()
        {
            if (_entitiesCache != _entitasGroup.GetEntities())
            {
                _entitiesCache = _entitasGroup.GetEntities();
                _gameEntitiesCache.Clear();
                _context.GetEntities(_entitiesCache, _gameEntitiesCache);
            }
            return _gameEntitiesCache;
        }
    }
}