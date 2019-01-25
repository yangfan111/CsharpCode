using Core.CameraControl;
using Core.CharacterController;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Camera
{
    public class PlayerRotateLimitSystem: IBeforeUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerRotateLimitSystem));
        
        public void BeforeExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = owner.OwnerEntity as PlayerEntity;
            if (!playerEntity.hasCharacterContoller || !playerEntity.hasPlayerRotateLimit || !playerEntity.hasPosition || !playerEntity.hasOrientation)
            {
                return;
            }

            var controller = playerEntity.characterContoller.Value;
            var limitComponent = playerEntity.playerRotateLimit;

            if (controller.enabled && controller.GetCurrentControllerType() == CharacterControllerType.ProneKinematicCharacterController)
            {
                var bound = controller.GetRotateBound(playerEntity.orientation.ModelView, playerEntity.position.Value,cmd.FrameInterval);
                var yaw = YawPitchUtility.Normalize(playerEntity.orientation.ModelYaw);
                limitComponent.SetLimit(yaw + bound.Key, yaw + bound.Value);
                //Logger.InfoFormat("seq:{0}, lowerBound:{1}, upperBoound:{2}, cur Yaw:{3}", cmd.Seq ,bound.Key, bound.Value, yaw);
            }
            else
            {
                limitComponent.SetNoLimit();
            }
        }
    }
}