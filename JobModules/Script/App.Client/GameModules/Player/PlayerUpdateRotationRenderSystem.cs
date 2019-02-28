using App.Shared.GameModules.Player;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Player
{
    public class PlayerUpdateRotationRenderSystem : AbstractSelfPlayerRenderSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerUpdateRotationRenderSystem));


        public PlayerUpdateRotationRenderSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return PlayerMoveSystem.CanPlayerMove(playerEntity) && playerEntity.hasPlayerSkyMove &&
                   playerEntity.hasOrientation && playerEntity.hasCharacterContoller &&
                   !playerEntity.playerSkyMove.IsMoveEnabled;
        }

        public override void OnRender(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            controller.SetCharacterRotation(Quaternion.Euler(player.orientation.ModelPitch,
                player.orientation.Yaw + player.orientation.PunchYaw, 0));
            //_logger.InfoFormat("---------------------------------------------------------------------");
        }
    }
}