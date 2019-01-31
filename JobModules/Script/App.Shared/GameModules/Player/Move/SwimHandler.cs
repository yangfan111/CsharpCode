
using Core.CameraControl;
using Core.CharacterController.ConcreteController;
using Core.Compare;
using Core.Configuration;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player
{

    public class SwimHandler
    {

        public static void Move(Contexts contexts, PlayerEntity player, float deltaTime)
        {
            var state = player.stateInterface.State;
            
            var postureInConfig = state.GetNextPostureState();

            var controller = player.characterContoller.Value;

            controller.SetCurrentControllerType(postureInConfig);
            
            controller.SetCharacterPosition(player.position.Value);
            
            var diveXAngle = CalcSwimXAngle(state.VerticalValue, state.HorizontalValue, state.UpDownValue);
            
            controller.SetCharacterRotation(new Vector3(diveXAngle,YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw), 0 ));
            
            var lastVel = Quaternion.Inverse(player.orientation.RotationYaw) * player.playerMove.Velocity.ToVector4();
            
            var velocity = player.stateInterface.State.GetSpeed(lastVel, deltaTime);
            
            velocity = player.orientation.RotationYaw * velocity.ToVector4();
            
            var velocityOffset = player.stateInterface.State.GetSpeedOffset();
            
            var dist = (velocity + velocityOffset) * deltaTime;

            if (DiveTest(player, velocityOffset, dist))
            {
                player.stateInterface.State.Dive();
                DiveHandler.Move(contexts,player, deltaTime);
                player.moveUpdate.BeginDive = true;
            }

            else
            {
                PlayerMoveUtility.Move(contexts, player, player.characterContoller.Value, dist, deltaTime);
                player.playerMove.Velocity = velocity;
                player.position.Value = controller.transform.position;
            }
        }

        private static bool DiveTest(PlayerEntity player, Vector3 velocityOffset, Vector3 dist)
        {
            return velocityOffset.magnitude > 0 ||
                   (player.stateInterface.State.IsForth &&
                    dist.sqrMagnitude > 0 &&
                    player.orientation
                        .Pitch >= //SingletonManager.Get<CameraConfigManager>().GetConfigByType(ECameraViewMode.ThirdPerson).PitchLimit.Max - 1);
                    SingletonManager.Get<CameraConfigManager>().Config.PoseConfigs[(int)ECameraPoseMode.Stand]
                        .PitchLimit.Max - 1);
        }

        private static float CalcSwimXAngle(float vertical, float horizontal, float updown)
        {
            float retVertical = 0.0f;
            if (vertical < 0)
            {
                retVertical = Mathf.Lerp(SwimCharacterController.SwimBack, 0, vertical + 1);
            }
            else
            {
                retVertical = Mathf.Lerp(0, SwimCharacterController.SwimFowrard, vertical);
            }
            return retVertical;
        }

    }
}