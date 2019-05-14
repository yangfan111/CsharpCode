using System.Collections.Generic;
using System.Linq;
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


        public Vector3 GetPosition(EntityKey entityKey)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if (entity == null) return Vector3.zero;
            return entity.hitBox.HitPreliminaryGeo.position;
        }

        public float GetRadius(EntityKey entityKey)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if (entity == null) return -1;
            return entity.hitBox.HitPreliminaryGeo.radius;
        }

        public void EnableHitBox(EntityKey entityKey, bool enable)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if (entity != null && entity.hasPosition && entity.hasHitBox)
            {
                var provider = SingletonManager.Get<HitBoxTransformProviderCache>().GetProvider(entity.thirdPersonModel.Value);
                if (provider != null)
                    provider.SetActive(enable);
            }
        }

        public List<Transform> GetCollidersTransform(EntityKey entityKey)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if (entity != null && entity.hasPosition && entity.hasHitBox)
            {
                var provider = SingletonManager.Get<HitBoxTransformProviderCache>().GetProvider(entity.thirdPersonModel.Value);
                if (provider != null) return provider.GetHitBoxTransforms().Values.ToList();
            }
            return null;
        }
        
        public void UpdateHitBox(IGameEntity gameEntity)
        {
        }
    }
}