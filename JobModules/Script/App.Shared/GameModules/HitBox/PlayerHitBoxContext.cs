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

namespace App.Shared.GameModules.Attack
{
    public class PlayerHitBoxContext :IHitBoxContext
    {
        protected static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerHitBoxContext));
        private PlayerContext _playerContext;
        private AnimatorPoseReplayer _poseReplayer;
      
        public PlayerHitBoxContext(PlayerContext playerContext)
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
            if (entity != null && entity.hasPosition && entity.hasHitBox && entity.gamePlay.IsAlive())
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

        StringBuilder _stringBuilder = new StringBuilder();
        public string GetCollidersDebugDatas(EntityKey entityKey)
        {
            var entity = _playerContext.GetEntityWithEntityKey(entityKey);
            if (entity != null && entity.hasPosition && entity.hasHitBox)
            {
                HitBoxTransformProvider provider = SingletonManager.Get<HitBoxTransformProviderCache>().GetProvider(entity.thirdPersonModel.Value);
                if (provider != null)
                {
                    _stringBuilder.Length = 0;
                     var colliders = provider.GetHitBoxColliders();
                     foreach (KeyValuePair<string, Collider> keyPair in colliders)
                     {
                         _stringBuilder.AppendFormat("<{0}=>trans:{1}>\n", keyPair.Key,
                             keyPair.Value.transform.position);
                         // _stringBuilder.AppendFormat("Key:{0},ctrans:{1},bounds:{2}", keyPair.Key,
                                                                                                                           // keyPair.Value.transform.position,keyPair.Value.bounds.ToString("f3"));
                     }

                     // var list = GetCollidersTransform(entityKey);
                     // foreach (var val in list)
                     // {
                     //     _stringBuilder.AppendFormat("trans:{0}", val.position);
                     // }
                     return _stringBuilder.ToString();
                }
                
            }
            return string.Empty;
        }
        public PlayerEntity GetPlayerEntity(IGameEntity gameEntity)
        {
            var playerEntity = _playerContext.GetEntityWithEntityKey(gameEntity.EntityKey);
            return playerEntity;
        }
        public HitBoxComponent GetHitBoxComponent(PlayerEntity entity)
        {
            if ( entity!= null && entity.hasPosition && entity.hasHitBox)
            {
                return entity.hitBox;
            }

            return null;
        }
        public virtual void UpdateHitBox(IGameEntity gameEntity, int renderTime, int cmdSeq)
        {
            DebugUtil.AppendShootText(cmdSeq,"[animator] {0}", gameEntity.GetComponent<NetworkAnimatorComponent>().ToStringExt());
            DebugUtil.AppendShootText(cmdSeq,"[collider]\n{0}",GetCollidersDebugDatas(gameEntity.EntityKey) );
            
        }

        public virtual void RecoverHitBox(IGameEntity gameEntity, int renderTime)
        {
           
        }
    }
}