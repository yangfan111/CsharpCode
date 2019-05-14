using App.Shared.Player;
using Core.CameraControl;
using Core.Components;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions.Move
{
    public static class LadderMove
    {
        private static readonly Vector3 ModelInitPosition = new Vector3(0,0,0);
        public static void UpdateTransform(PlayerEntity player, float deltaTime)
        {
            var parent = player.RootGo().transform;
            var child = player.thirdPersonModel.Value.transform;
            var pos = child.localPosition - ModelInitPosition;
            var rotation = child.localRotation;

            parent.Translate(Vector3.right * pos.x);
            parent.Translate(Vector3.up * pos.y);
            parent.Translate(Vector3.forward * pos.z);

            parent.rotation = player.orientation.RotationYaw;
            parent.rotation *= rotation;
            
            var lastVel = Quaternion.Inverse(player.orientation.RotationYaw) * player.playerMove.Velocity.ToVector4();
            var currentVelocity = player.orientation.RotationYaw * player.stateInterface.State.GetSpeed(lastVel, deltaTime);
            currentVelocity.y = 0;

            player.characterContoller.Value.Move(currentVelocity * deltaTime);

            child.localPosition = ModelInitPosition;
            child.localRotation = Quaternion.identity;

            player.playerMoveByAnimUpdate.NeedUpdate = true;
            player.position.Value = parent.position;
            player.playerMoveByAnimUpdate.Position =  parent.position.ShiftedToFixedVector3();
            
            player.playerMoveByAnimUpdate.ModelPitch = player.orientation.ModelPitch = YawPitchUtility.Normalize(parent.rotation.eulerAngles.x);
            player.playerMoveByAnimUpdate.ModelYaw = player.orientation.ModelYaw = YawPitchUtility.Normalize(parent.rotation.eulerAngles.y);
        }
    }
}
