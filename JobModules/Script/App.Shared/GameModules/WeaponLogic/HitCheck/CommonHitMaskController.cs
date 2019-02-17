using Core.Attack;
using Core.BulletSimulation;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="CommonHitMaskController" />
    /// </summary>
    public class CommonHitMaskController : IHitMaskController
    {
        private PlayerEntity _playerEntity;

        private PlayerContext _playerContext;

        private List<int> _excludeList = new List<int>();

        public CommonHitMaskController(PlayerContext playerContext, PlayerEntity entity)
        {
            _playerEntity = entity;
            _playerContext = playerContext;
        }

        public List<int> BulletExcludeTargetList
        {
            get
            {
                _excludeList.Clear();
                var players = _playerContext.GetEntities();
                foreach (var otherPlayer in players)
                {
                    if ((otherPlayer.playerMask.SelfMask & _playerEntity.playerMask.TargetMask) == 0
                        || otherPlayer.playerMask.SelfMask == (byte)EPlayerMask.Invincible)
                    {
                        _excludeList.Add(otherPlayer.entityKey.Value.EntityId);
                    }
                }
                return _excludeList;
            }
        }

        public List<int> MeleeExcludeTargetList
        {
            get
            {
                _excludeList.Clear();
                var players = _playerContext.GetEntities();
                foreach (var otherPlayer in players)
                {
                    if ((otherPlayer.playerMask.SelfMask & _playerEntity.playerMask.TargetMask) == 0
                        || otherPlayer.playerMask.SelfMask == (byte)EPlayerMask.Invincible)
                    {
                        _excludeList.Add(otherPlayer.entityKey.Value.EntityId);
                    }
                }
                return _excludeList;
            }
        }

        public List<int> ThrowingExcludeTargetList
        {
            get
            {
                _excludeList.Clear();
                var players = _playerContext.GetEntities();
                foreach (var otherPlayer in players)
                {
                    if (otherPlayer.playerMask.SelfMask == (byte)EPlayerMask.Invincible)
                    {
                        _excludeList.Add(otherPlayer.entityKey.Value.EntityId);
                    }
                }
                return _excludeList;
            }
        }
    }
}
