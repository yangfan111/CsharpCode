using App.Shared.Components.Player;
using App.Shared.Player;
using Core.CameraControl;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions
{
    public class PlayerMoveByRootMotionSystem : IUserCmdExecuteSystem
    {
        public PlayerMoveByRootMotionSystem()
        {
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity)owner.OwnerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                // gamePlay有对应的处理，这里不需要
                return;
            }

            UpdateTransform(player);
        }

        private void UpdateTransform(PlayerEntity player)
        {
            //rootMotion控制人物移动
            if (!player.thirdPersonAnimator.UnityAnimator.applyRootMotion)
            {
                player.playerMoveByAnimUpdate.NeedUpdate = false;
                return;
            }

            var parent = player.RootGo().transform;
            var child = player.thirdPersonModel.Value.transform;

            parent.Translate(Vector3.right * child.localPosition.x);
            parent.Translate(Vector3.up * child.localPosition.y);
            parent.Translate(Vector3.forward * child.localPosition.z);

            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;

            player.playerMoveByAnimUpdate.NeedUpdate = true;
            player.playerMoveByAnimUpdate.Position = player.position.Value = parent.position;
            player.playerMoveByAnimUpdate.ModelPitch = player.orientation.ModelPitch = YawPitchUtility.Normalize(parent.rotation.eulerAngles.x);
            player.playerMoveByAnimUpdate.ModelYaw = player.orientation.ModelYaw = YawPitchUtility.Normalize(parent.rotation.eulerAngles.y);
            
            if(player.hasPlayerMove)
                player.playerMove.Velocity = Vector3.zero;
            if(player.hasMoveUpdate)
                player.moveUpdate.Velocity = Vector3.zero;
        }
    }
}
