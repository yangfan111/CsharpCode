using System;
using App.Client.GameModules.Player;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Bullet;
using Core.Animation;
using Core.Compensation;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.HitBox;
using Core.Utils;
using Entitas;
using UnityEngine;
using Random = System.Random;

namespace App.Shared.GameModules.Player
{
    public class PlayerDebugDrawSystem : AbstractGamePlaySystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAttackSystem));
        private IHitBoxEntityManager _hitBoxEntityManager;

        public PlayerDebugDrawSystem(Contexts contexts) : base(contexts)
        {
            _hitBoxEntityManager = new HitBoxEntityManager(contexts,false);
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.EntityAdapter, PlayerMatcher.HitBox));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return DebugConfig.DrawHitBoxOnFrame;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
            _hitBoxEntityManager.UpdateHitBox(entity.entityAdapter.SelfAdapter);
            _hitBoxEntityManager.DrawHitBoxOnFrame(entity.entityAdapter.SelfAdapter);
        }
    }
}