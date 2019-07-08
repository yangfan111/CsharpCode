using System.Collections.Generic;
using Core.Attack;
using Core.EntityComponent;
using Entitas;

namespace App.Shared.GameModules.Attack
{
    public class BulletEntityCollector
    {
        private bool _dirty = true;
        private IGroup<BulletEntity> _group;
        private List<IBulletEntityAgent> _list = new List<IBulletEntityAgent>();

        private EntityKey _ownerEntityKey;
        private PlayerContext _playerContext;

        public BulletEntityCollector(BulletContext bulletContext, PlayerContext playerContext)
        {
            _group =
                            bulletContext.GetGroup(BulletMatcher
                                                   .AllOf(BulletMatcher.OwnerId).NoneOf(BulletMatcher.FlagDestroy));
            _group.OnEntityAdded   += GroupOnOnEntityAdded;
            _group.OnEntityRemoved += GroupOnOnEntityRemoved;
            _group.OnEntityUpdated += GroupOnOnEntityUpdated;
            _playerContext         =  playerContext;
        }

        public EntityKey BulletOwner
        {
            set
            {
                _ownerEntityKey = value;
                _dirty          = true;
            }
        }

        private void GroupOnOnEntityUpdated(IGroup<BulletEntity> group, BulletEntity bulletEntity, int index,
                                            IComponent previousComponent, IComponent newComponent)
        {
            _dirty = true;
        }

        private void GroupOnOnEntityRemoved(IGroup<BulletEntity> group, BulletEntity bulletEntity, int index,
                                            IComponent component)
        {
            _dirty = true;
        }

        private void GroupOnOnEntityAdded(IGroup<BulletEntity> group, BulletEntity bulletEntity, int index,
                                          IComponent component)
        {
            _dirty = true;
        }

        public List<IBulletEntityAgent> GetAllPlayerBulletAgents()
        {
            if (_dirty)
            {
                _dirty = false;
                _list.Clear();
                var entites = _group.GetEntities();
                for (int i = 0; i < entites.Length; i++)
                {
                    var entity = entites[i];
                    if (entity.ownerId.Value == _ownerEntityKey)
                    {
                        var bulletAgent = entity.bulletRuntime.BulletAgent as BulletEntityAgent;
                        bulletAgent.SetPlayerContext(_playerContext);
                        _list.Add(entity.bulletRuntime.BulletAgent);
                    }
                }
            }

            return _list;
        }
    }
}