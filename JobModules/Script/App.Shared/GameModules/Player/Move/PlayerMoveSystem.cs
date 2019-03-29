using System.Collections;
using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using Core.CharacterState.Movement;
using Core.Compare;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.HitBox;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using XmlConfig;
using App.Shared.Player;
using Core.CameraControl;
using Core.CharacterController;
using App.Shared.GameModules.Player.Appearance;
using Core.CharacterController.ConcreteController;
using Utils.Singleton;
using Utils.Utils;

namespace App.Shared.GameModules.Player
{
    public enum MoveType
    {
        Land,
        Swim,
        Dive,
        Fly,
        EnumEnd
    }
   
    public class PlayerMoveSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerMoveSystem));
        private readonly Contexts _contexts;
        
        public PlayerMoveSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;

            CorrectPositionByServer(cmd, player);
            
            CheckPlayerLifeState(player);

            var updateComponent = player.moveUpdate;
            
            updateComponent.LastPosition = player.position.Value;

            updateComponent.Reset();
            
            if (!CanPlayerMove(player))
            {
                return;
            }
            
            player.playerMove.ClearState();

            MoveType moveType = GetPlayerMoveType(player);
            switch (moveType)
            {
                case MoveType.Land:
                    LandHandler.Move(_contexts, player, cmd.FrameInterval * 0.001f);
                    break;
                case MoveType.Swim:
                    SwimHandler.Move(_contexts, player, cmd.FrameInterval * 0.001f);
                    break;
                case MoveType.Dive:
                    DiveHandler.Move(_contexts, player, cmd.FrameInterval * 0.001f);
                    break;
                case MoveType.Fly:
                    FlyHandler.Move(player, cmd, cmd.FrameInterval * 0.001f);
                    break;
                default:
                    break;
            }
            
            SyncPlayerMove(player, moveType);
        }

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.playerGameState) return;
            var gameState = player.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }

        private void Dead(PlayerEntity player)
        {
        }

        private void Reborn(PlayerEntity player)
        {
            player.playerMove.Reset();
        }

        private void CorrectPositionByServer(IUserCmd cmd, PlayerEntity player)
        {
            var adjustPos = player.latestAdjustCmd.GetPos(cmd.Seq);
            if (adjustPos != null) player.position.Value = adjustPos.Value;
        }

        private MoveType GetPlayerMoveType(PlayerEntity player)
        {
            var postureInConfig = player.stateInterface.State.GetNextPostureState();
            if (postureInConfig == PostureInConfig.Swim)
                return MoveType.Swim;
            if (postureInConfig == PostureInConfig.Dive)
                return MoveType.Dive;
            if (player.playerSkyMove.IsMoveEnabled)
                return MoveType.Fly;
            return MoveType.Land;
        }
        
        private void SyncPlayerMove(PlayerEntity player, MoveType moveType)
        {
            if (moveType == MoveType.Fly)
                SyncSkyMoveComponent(player);
            var uploadComponent = player.moveUpdate;
            uploadComponent.NeedUpdate = false;
            uploadComponent.NeedUpdate = true;  
            uploadComponent.MoveType = (int)moveType;
            uploadComponent.SpeedRatio = player.playerMove.SpeedRatio;
            uploadComponent.IsGround = player.playerMove.IsGround;
            uploadComponent.IsCollided = player.playerMove.IsCollided;
            uploadComponent.MoveInWater = player.stateInterface.State.IsMoveInWater();
            uploadComponent.ExceedSteepLimit = player.stateInterface.State.IsSteepSlope();
            uploadComponent.Velocity = player.playerMove.Velocity;
            uploadComponent.Rotation = player.RootGo().transform.eulerAngles;
            uploadComponent.ModelPitch = player.orientation.ModelPitch;
            uploadComponent.ModelYaw = player.orientation.ModelYaw;
            uploadComponent.MoveSpeedRatio = player.playerMove.MoveSpeedRatio;
            uploadComponent.NeedUpdate = true;
            SyncUpdateComponentPos(player, player.position.Value);
        }

        private void SyncSkyMoveComponent(PlayerEntity player)
        {
            player.skyMoveUpdate.SkyMoveStage = player.playerSkyMove.MoveStage;
            player.skyMoveUpdate.SkyPosition = player.playerSkyMove.Position;
            player.skyMoveUpdate.SkyRotation = player.playerSkyMove.Rotation;
            player.skyMoveUpdate.SkyLocalPlayerPosition = player.playerSkyMove.LocalPlayerPosition;
            player.skyMoveUpdate.SkyLocalPlayerRotation = player.playerSkyMove.LocalPlayerRotation;
            player.skyMoveUpdate.Pitch = player.orientation.Pitch;
            player.skyMoveUpdate.Yaw = player.orientation.Yaw;
            player.skyMoveUpdate.Roll = player.orientation.Roll;
            player.skyMoveUpdate.GameState = player.gamePlay.GameState;
            player.skyMoveUpdate.IsMoveEnabled = player.playerSkyMove.IsMoveEnabled;
        }

        public static void SyncUpdateComponentPos(PlayerEntity player, Vector3 pos)
        {
            MoveUpdateComponent updateComponent = player.moveUpdate;
            player.position.Value = pos;
            updateComponent.Dist = player.position.Value - updateComponent.LastPosition;
        }

        /// <summary>
        /// 角色是否可以移动和旋转
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool CanPlayerMove(PlayerEntity player)
        {
            if ((player.hasCharacterContoller && !player.characterContoller.Value.enabled) ||
                (player.hasThirdPersonAnimator && player.thirdPersonAnimator.UnityAnimator.applyRootMotion))
            {
                return false;
            }

            if (player.IsOnVehicle() || PlayerStateUtil.HasPlayerState(EPlayerGameState.NotMove, player.gamePlay))
            {
                return false;
            }

            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return false;
            }

            return true;
        }

       
    }
}