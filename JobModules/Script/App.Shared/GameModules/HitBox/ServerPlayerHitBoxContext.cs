using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Attack
{
    public class ServerPlayerHitBoxContext : PlayerHitBoxContext
    {
        public ServerPlayerHitBoxContext(PlayerContext playerContext) : base(playerContext)
        {
        }

        public override void UpdateHitBox(IGameEntity gameEntity, int renderTime, int cmdSeq)
        {
            var position = gameEntity.Position.Value;
            var rotation = gameEntity.GetComponent<OrientationComponent>().ModelView;
            var playerEntity = GetPlayerEntity(gameEntity);
            var hitBoxComponent = GetHitBoxComponent(playerEntity);

            if (hitBoxComponent != null)
            {
                playerEntity.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                PlayerEntityUtility.UpdateTransform(playerEntity, gameEntity.GetComponent<NetworkAnimatorComponent>(),
                    gameEntity.GetComponent<PredictedAppearanceComponent>(),
                    gameEntity.GetComponent<OrientationComponent>());

                // DebugUtil.AppendShootText(renderTime, "server animator {0}",
                //     gameEntity.GetComponent<NetworkAnimatorComponent>().ToStringExt());

                playerEntity.RootGo().transform.SetPositionAndRotation(position, rotation);
                playerEntity.hitBox.RenderTime = renderTime;

                // if (_logger.IsDebugEnabled)
                // {
                //     StringBuilder s = new StringBuilder();
                //     hitBoxComponent.HitBoxGameObject.transform.Recursively(t => s.Append("[n " + t.name + ", p " + t.position.ToStringExt() + ", r " + t.rotation.ToStringExt() + "]"));
                // //    _logger.DebugFormat("hitbox pos {0}, rot {1}, transforms {2}, ", position, rotation, s);
                // DebugUtil.AppendShootText(cmdSeq,"hitbox pos {0}, rot {1}, transforms {2}, ", position, rotation, s);
                // }
                base.UpdateHitBox(gameEntity, renderTime, cmdSeq);
            }
        }

        public override void RecoverHitBox(IGameEntity gameEntity, int renderTime)
        {
            var playerEntity = GetPlayerEntity(gameEntity);
            var hitBoxComponent = GetHitBoxComponent(playerEntity);
            if (hitBoxComponent != null)
            {
                if (playerEntity.hitBox.RenderTime != renderTime)
                {
                    playerEntity.RootGo().transform.SetPositionAndRotation(playerEntity.position.Value, playerEntity.orientation.ModelView);
                    
                    PlayerEntityUtility.UpdateTransform(playerEntity, playerEntity.networkAnimator,
                        playerEntity.predictedAppearance, playerEntity.orientation);
                    playerEntity.thirdPersonAnimator.UnityAnimator.cullingMode =
                        AnimatorCullingMode.CullUpdateTransforms;
                    playerEntity.hitBox.RenderTime = renderTime;
                    
                }
            }

            base.RecoverHitBox(gameEntity, renderTime);
        }
    }
}