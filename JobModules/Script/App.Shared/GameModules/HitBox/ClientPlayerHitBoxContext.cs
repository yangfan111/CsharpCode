using System.Collections.Generic;
using App.Shared.Components.Common;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core.Animation;
using Core.Components;
using Core.EntityComponent;
using Core.HitBox;
using Core.Utils;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.Bullet
{
    public class ClientPlayerHitBoxContext : IHitBoxContext
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientPlayerHitBoxContext));
        private PlayerContext _playerContext;
        private AnimatorPoseReplayer _poseReplayer;
      
        public ClientPlayerHitBoxContext(PlayerContext playerContext)
        {
            _playerContext = playerContext;
            _poseReplayer = new AnimatorPoseReplayer();
        }

        public HitBoxComponent GetHitBoxComponent(EntityKey entityKey)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if ( entity!= null && entity.hasPosition && entity.hasHitBox)
            {
                return entity.hitBox;
            }
            return null;
        }

        public HitBoxTransformProvider GetHitBoxProvider(EntityKey entityKey)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if (entity != null && entity.hasPosition && entity.hasHitBox)
            {
                var provider = SingletonManager.Get<HitBoxTransformProviderCache>().GetProvider(entity.thirdPersonModel.Value);
                if (provider != null) return provider;
            }
            return null;
        }

        public void UpdateHitBox(IGameEntity gameEntity)
        {
        }
      
        private PlayerEntity GetPlayerEntity(IGameEntity gameEntity)
        {
            var playerEntity = _playerContext.GetEntityWithEntityKey(gameEntity.EntityKey);
            return playerEntity;
        }
    }
}