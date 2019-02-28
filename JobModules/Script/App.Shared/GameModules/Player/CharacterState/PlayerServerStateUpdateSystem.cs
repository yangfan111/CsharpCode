using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Player.Animation;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.Player;
using Core.Animation;
using Core.CharacterState;
using Core.CharacterState.Posture;
using Core.Common;
using Core.Fsm;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.WeaponAnimation;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterState
{
    public class PlayerServerStateUpdateSystem: IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerServerStateUpdateSystem));

        private Contexts _contexts;
        
        private readonly FsmOutputBaseSystem _fsmOutputs = new FsmOutputBaseSystem();

        public PlayerServerStateUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;

            CheckPlayerLifeState(playerEntity);

            var stateManager = playerEntity.stateInterface.State;
            
            ComponentSynchronizer.SyncFromStateInterVarComponent(playerEntity.stateInterVar, stateManager );
            ComponentSynchronizer.SyncFromStateComponent(playerEntity.state, stateManager );
            stateManager.ServerUpdate();
            _fsmOutputs.ResetOutput();
            ConverntToFsmOutCommand(playerEntity.stateInterVar.StateInterCommands);
            _fsmOutputs.SetOutput(playerEntity);
            stateManager.HandleAnimationCallback(playerEntity.stateInterVar.AnimationCallbackCommands.Commands);
            
            //动画回调
            ProcessAnimationEvent(playerEntity);

            SyncThirdPersonAppearance(playerEntity);
        }

        private static void ProcessAnimationEvent(PlayerEntity playerEntity)
        {
            var firstPersonEvent = playerEntity.firstPersonModel.Value.GetComponent<AnimationClipEvent>();
            if (firstPersonEvent != null)
            {
                foreach (KeyValuePair<short, AnimationEventParam> keyValuePair in playerEntity.stateInterVar.FirstPersonAnimationEventCallBack
                    .Commands)
                {
                    firstPersonEvent.ServerFunc(keyValuePair.Key, keyValuePair.Value);
                }
            }

            var thirdPersonEvent = playerEntity.thirdPersonModel.Value.GetComponent<AnimationClipEvent>();
            if (thirdPersonEvent != null)
            {
                foreach (KeyValuePair<short, AnimationEventParam> keyValuePair in playerEntity.stateInterVar.ThirdPersonAnimationEventCallBack
                    .Commands)
                {
                    thirdPersonEvent.ServerFunc(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }


        private void ConverntToFsmOutCommand(StateInterCommands commands)
        {
            foreach (KeyValuePair<short,float> pair in commands.Commands)
            {
                _fsmOutputs.AddOutput(new FsmOutput
                {
                    Type = (FsmOutputType)pair.Key,
                    FloatValue = pair.Value
                });
            }
        }
        
        private void SyncThirdPersonAppearance(PlayerEntity player)
        {
            player.thirdPersonAppearance.Posture = ThirdPersonAppearanceUtils.GetPosture(player.stateInterface.State.GetCurrentPostureState());
            player.thirdPersonAppearance.NextPosture = ThirdPersonAppearanceUtils.GetPosture(player.stateInterface.State.GetNextPostureState());
            player.thirdPersonAppearance.Action = ThirdPersonAppearanceUtils.GetAction(player.stateInterface.State.GetActionState());
            player.thirdPersonAppearance.Movement = ThirdPersonAppearanceUtils.GetMovement(player.stateInterface.State.GetCurrentMovementState());
            player.thirdPersonAppearance.PeekDegree = player.characterBoneInterface.CharacterBone.PeekDegree;
            player.thirdPersonAppearance.NeedUpdateController = true;
            player.thirdPersonAppearance.CharacterHeight = player.characterControllerInterface.CharacterController.GetCharacterControllerHeight;
            player.thirdPersonAppearance.CharacterCenter = player.characterControllerInterface.CharacterController.GetCharacterControllerCenter;
            player.thirdPersonAppearance.CharacterRadius = player.characterControllerInterface.CharacterController.GetCharacterControllerRadius;
        }
        
        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.playerGameState) return;
            var gameState = player.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Revive:
                    Revive(player);
                    break;
                case PlayerLifeStateEnum.Dying:
                    Dying(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }

        private void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.PlayerReborn();
        }
        
        private void Revive(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.Revive();
        }

        private void Dying(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.Dying();
        }

        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.PlayerReborn();
            _logger.InfoFormat("ServerPlayerStateUpdateDead");
        }

        #endregion
    }
}