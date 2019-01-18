using Core.CameraControl;
using Core.CharacterController.ConcreteController;
using Core.Compare;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class DiveHandler 
    {

        public static void Move(Contexts contexts, PlayerEntity player, float deltaTime)
        {
            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();

            var controller = player.characterContoller.Value;

            controller.SetCurrentControllerType(postureInConfig);

            var diveXAngle = CalcDiveXAngle(state.VerticalValue, state.HorizontalValue, state.UpDownValue);

            controller.SetCharacterPosition(player.position.Value);
            //_logger.InfoFormat("my epxected rot:x:{0},y:{1}, new:{2}", diveXAngle, YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw),YawPitchUtility.Normalize(Quaternion.Euler(diveXAngle,YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw), 0 )).ToStringExt() );
            controller.SetCharacterRotation(new Vector3(diveXAngle,YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw), 0));

            var velocity = player.stateInterface.State.GetSpeed(Vector3.zero, deltaTime);
            velocity = player.orientation.RotationView * velocity.ToVector4();
            var velocityOffset = player.stateInterface.State.GetSpeedOffset();

            var dist = (velocity + velocityOffset) * deltaTime;

            PlayerMoveUtility.Move(contexts, player, player.characterContoller.Value, dist, deltaTime);

            //_logger.InfoFormat("dive move dist:{0}, prev pos:{1}, after pos:{2}", dist.ToStringExt(), player.position.Value.ToStringExt(), controller.transform.position.ToStringExt());
            player.playerMove.Velocity = velocity;
            player.position.Value = controller.transform.position;
        }
        
        private static float CalcDiveXAngle(float vertical, float horizontal, float updown)
        {
            float retVertical = 0.0f;
            float retUpDown = 0.0f;
            // process vertical
            if (vertical < 0)
            {
                retVertical = Mathf.Lerp(DiveCharacterController.DiveBack, 0, vertical + 1);
            }
            else
            {
                retVertical = Mathf.Lerp(0, DiveCharacterController.DiveFowrard, vertical);
            }

            if (updown < 0)
            {
                retUpDown = Mathf.Lerp(DiveCharacterController.DiveDown, 0, updown + 1);
            }
            else
            {
                retUpDown = Mathf.Lerp(0, DiveCharacterController.DiveUp, updown);
            }

            float absVertical = Mathf.Abs(vertical);
            float absUpdown = Mathf.Abs(updown);
            float total = absVertical + absUpdown;
            if (CompareUtility.IsApproximatelyEqual(total, 0))
            {
                return retVertical;
            }

            return retVertical * absVertical / total + retUpDown * absUpdown / total;


        }
    }
}