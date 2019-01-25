using System;
using System.Collections.Generic;

using Core.BulletSimulation;
using Core.EntityComponent;
using Entitas;



namespace App.Shared.GameModules.Bullet
{
    public class BulletEntityCollector : IBulletEntityCollector
    {
        private IGroup<BulletEntity> _group;
        private bool _dirty = true;
        private PlayerContext _playerContext;
        public BulletEntityCollector(BulletContext bulletContext, PlayerContext playerContext)
        {
            _group = bulletContext.GetGroup(BulletMatcher.AllOf(BulletMatcher.OwnerId).NoneOf(BulletMatcher.FlagDestroy));
            _group.OnEntityAdded += GroupOnOnEntityAdded;
            _group.OnEntityRemoved += GroupOnOnEntityRemoved;
            _group.OnEntityUpdated += GroupOnOnEntityUpdated;
            _playerContext = playerContext;
        }

        private void GroupOnOnEntityUpdated(IGroup<BulletEntity> @group, BulletEntity bulletEntity, int index, IComponent previousComponent, IComponent newComponent)
        {
            _dirty = true;
        }

        private void GroupOnOnEntityRemoved(IGroup<BulletEntity> @group, BulletEntity bulletEntity, int index, IComponent component)
        {
            _dirty = true;
        }

        private void GroupOnOnEntityAdded(IGroup<BulletEntity> @group, BulletEntity bulletEntity, int index, IComponent component)
        {
            _dirty = true;
        }

        private EntityKey _ownerEntityKey = new EntityKey();
        public EntityKey OwnerEntityKey { set { _ownerEntityKey = value;
            _dirty = true;
        } }
        private List<IBulletEntity> _list = new List<IBulletEntity>();
        public List<IBulletEntity> GetAllBulletEntities()
        {
            if (_dirty)
            {
                _dirty = false;
                _list.Clear();
                var entites = _group.GetEntities();
                for(int i = 0; i < entites.Length; i++)
                {
                    var entity = entites[i];
                    if (entity.ownerId.Value == _ownerEntityKey)
                    {
                        if (!entity.hasBulletEntityAdapter)
                        {
                            entity.AddBulletEntityAdapter(new BulletEntityAdapter(entity, _playerContext));
                        }
                        _list.Add(entity.bulletEntityAdapter.Adapter);
                    }
                }
            }
            return _list;
        }
    }
}