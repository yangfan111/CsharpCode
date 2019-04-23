using App.Shared.Player;
using Core.CameraControl;
using Core.Components;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions.Move
{
    public static class GenericMove
    {
        private static readonly Vector3 ModelInitPosition = new Vector3(0,-PlayerEntityUtility.CcSkinWidth,0);
        public static void UpdateTransform(PlayerEntity player, float deltaTime)
        {
            var parent = player.RootGo().transform;
            var child = player.thirdPersonModel.Value.transform;
            var pos = child.localPosition - ModelInitPosition;
            var rotation = child.localRotation;

            parent.Translate(Vector3.right * pos.x);
            parent.Translate(Vector3.up * pos.y);
            parent.Translate(Vector3.forward * pos.z);

            parent.rotation *= rotation;

            child.localPosition = ModelInitPosition;
            child.localRotation = Quaternion.identity;

            player.playerMoveByAnimUpdate.NeedUpdate = true;
            player.position.Value = parent.position;
            player.playerMoveByAnimUpdate.Position =  parent.position.ShiftedToFixedVector3();
            
            player.playerMoveByAnimUpdate.ModelPitch = player.orientation.ModelPitch = YawPitchUtility.Normalize(parent.rotation.eulerAngles.x);
            player.playerMoveByAnimUpdate.ModelYaw = player.orientation.ModelYaw = YawPitchUtility.Normalize(parent.rotation.eulerAngles.y);
            
            if(player.hasPlayerMove)
                player.playerMove.Velocity = Vector3.zero;
            if(player.hasMoveUpdate)
                player.moveUpdate.Velocity = Vector3.zero;
        }
    }
}
