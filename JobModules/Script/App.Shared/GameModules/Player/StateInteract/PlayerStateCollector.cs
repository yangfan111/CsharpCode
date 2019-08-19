using App.Shared.Components.Player;
using Core;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerStateCollector : IPlayerStateColltector
    {
        private PlayerEntity _playerEntity;
        private HashSet<EPlayerState> playerStates = new HashSet<EPlayerState>(EPlayerStateComparer.Instance);

        public PlayerStateCollector(PlayerEntity player)
        {
            _playerEntity = player;
        }

        public HashSet<EPlayerState> GetCurrStates(EPlayerStateCollectType collectType)
        {
            switch (collectType)
            {
                case EPlayerStateCollectType.UseCacheAddation:
                    if (_playerEntity.playerClientUpdate.OpenUIFrameSync)
                    {
                        _playerEntity.playerClientUpdate.OpenUIFrameSync = false;
                        playerStates.Add(EPlayerState.OpenUI);
                        
                    }
                   
                    var filteredInput = _playerEntity.StateInteractController().UserInput;
                    if (filteredInput.IsInput(EPlayerInput.IsPullboltInterrupt))
                        playerStates.Add(EPlayerState.PullBoltInterrupt);
                    break;
                case EPlayerStateCollectType.CurrentMoment:
                    Update();
                    break;
            }
           
            return playerStates;
        }

        public void Update()
        {
            playerStates.Clear();
            if (!_playerEntity.hasStateInterface)
            {
                return;
            }

            var gamePlay = _playerEntity.gamePlay;
            switch ((EPlayerLifeState) gamePlay.LifeState)
            {
                case EPlayerLifeState.Dead:
                    playerStates.Add(EPlayerState.Dead);
                    break;
                case EPlayerLifeState.Dying:
                    playerStates.Add(EPlayerState.Dying);
                    break;
            }

            if (_playerEntity.hasPlayerGameState)
            {
                switch (_playerEntity.playerGameState.CurrentPlayerLifeState)
                {
                    case PlayerLifeStateEnum.Reborn:
                    case PlayerLifeStateEnum.Dead:
                        playerStates.Add(EPlayerState.Dead);
                        break;
                }
            }
          
            var playerStateSource = _playerEntity.stateInterface.State;
            playerStates.Add(ActionToState(playerStateSource.GetActionState()));
            playerStates.Add(ActionToState(playerStateSource.GetNextActionState()));
            playerStates.Add(KeepActionToState(playerStateSource.GetActionKeepState()));
            playerStates.Add(LeanToState(playerStateSource.GetCurrentLeanState()));
            playerStates.Add(MoveToState(playerStateSource.GetCurrentMovementState()));
            playerStates.Add(MoveToState(playerStateSource.GetNextMovementState()));

            var poseState = playerStateSource.GetCurrentPostureState();
            var nextPoseState = playerStateSource.GetNextPostureState();
            playerStates.Add(PostureToState(poseState));
            if (poseState != nextPoseState && LegalTransition(poseState, nextPoseState))
                playerStates.Add(EPlayerState.PostureTrans);
          
            if (_playerEntity.hasOxygenEnergyInterface && _playerEntity.oxygenEnergyInterface.Oxygen.InSightDebuffState)
                playerStates.Add(EPlayerState.RunDebuff);

            if (_playerEntity.hasCameraStateNew && _playerEntity.cameraStateNew.FreeNowMode == (int) ECameraFreeMode.On)
                playerStates.Add(EPlayerState.CameraFree);

            if (IsPlayerOnAir()) playerStates.Add(EPlayerState.OnAir);

            if (gamePlay.IsSave) playerStates.Add(EPlayerState.Rescue);

            var move = playerStates.Contains(EPlayerState.Run) || playerStates.Contains(EPlayerState.Sprint) || playerStates.Contains(EPlayerState.Walk);
            if (move) playerStates.Add(EPlayerState.Move);
            if (playerStates.Contains(EPlayerState.Prone) && move)
                playerStates.Add(EPlayerState.ProneMove);

            if(_playerEntity.characterBone.IsWeaponRotState)
                playerStates.Add(EPlayerState.WeaponRotState);

            if (PlayerStateUtil.HasUIState(EPlayerUIState.PaintOpen, gamePlay))
                playerStates.Add(EPlayerState.PaintDisc);

            var throwingData = _playerEntity.throwingAction.ActionData;
            throwingData.IsThrowing = throwingData.IsThrowing && (playerStates.Contains(EPlayerState.Grenade));
            if(throwingData.IsThrowing) playerStates.Add(EPlayerState.GrenadeThrow);
            if (PlayerStateUtil.HasPlayerState(EPlayerGameState.TurnOver, _playerEntity.gamePlay))
                playerStates.Add(EPlayerState.FinalPosing);
            playerStates.Remove(EPlayerState.None);
        }

        public HashSet<EPlayerState> GetCurrStates(bool rightNow = false)
        {
            if (rightNow)
                Update();
            return playerStates;
        }

        private EPlayerState PostureToState(PostureInConfig posture)
        {
            switch (posture)
            {
                case PostureInConfig.Crouch:
                    return EPlayerState.Crouch;
                case PostureInConfig.Climb:
                    return EPlayerState.Climb;
                case PostureInConfig.Dive:
                    return EPlayerState.Swim;
                case PostureInConfig.Jump:
                    return EPlayerState.Jump;
                case PostureInConfig.Prone:
                    return EPlayerState.Prone;
                case PostureInConfig.Sight:
                    return EPlayerState.Sight;
                case PostureInConfig.Stand:
                case PostureInConfig.Land:
                    return EPlayerState.Stand;
                case PostureInConfig.Swim:
                    return EPlayerState.Swim;
                case PostureInConfig.ProneToCrouch:
                case PostureInConfig.ProneToStand:
                case PostureInConfig.ProneTransit:
                    return EPlayerState.PostureTrans;
                default:
                    return EPlayerState.None;
            }
        }

        private EPlayerState MoveToState(MovementInConfig movement)
        {
            switch (movement)
            {
                case MovementInConfig.Walk:
                    return EPlayerState.Walk;
                case MovementInConfig.Sprint:
                    return EPlayerState.Sprint;
                case MovementInConfig.Run:
                    return EPlayerState.Run;
                case MovementInConfig.Injured:
                    return EPlayerState.Injured;
                case MovementInConfig.Swim:
                    return EPlayerState.Swim;
                case MovementInConfig.Dive:
                    return EPlayerState.Swim;
                case MovementInConfig.Ladder:
                case MovementInConfig.EnterLadder:
                    return EPlayerState.Ladder;
                default:
                    return EPlayerState.None;
            }
        }

        private EPlayerState LeanToState(LeanInConfig lean)
        {
            switch (lean)
            {
                case LeanInConfig.PeekLeft:
                    return EPlayerState.PeekLeft;
                case LeanInConfig.PeekRight:
                    return EPlayerState.PeekRight;
                default:
                    return EPlayerState.None;
            }
        }

        private EPlayerState KeepActionToState(ActionKeepInConfig keepAction)
        {
            switch (keepAction)
            {
                case ActionKeepInConfig.Drive:
                    return EPlayerState.Drive;
                case ActionKeepInConfig.Sight:
                    return EPlayerState.Sight;
                default:
                    return EPlayerState.None;
            }
        }

        private EPlayerState ActionToState(ActionInConfig action)
        {
            switch (action)
            {
                case ActionInConfig.Reload:
                    return EPlayerState.Reload;
                case ActionInConfig.SpecialFireHold:
                case ActionInConfig.Fire:
                    return EPlayerState.Firing;
                case ActionInConfig.SpecialReload:
                    return EPlayerState.SpecialReload;
                case ActionInConfig.SpecialFireEnd:
                    return EPlayerState.PullBolt;
                case ActionInConfig.Gliding:
                    return EPlayerState.Gliding;
                case ActionInConfig.Parachuting:
                    return EPlayerState.Parachuting;
                case ActionInConfig.PickUp:
                    return EPlayerState.Pickup;
                case ActionInConfig.SwitchWeapon:
                    return EPlayerState.SwitchWeapon;
                case ActionInConfig.MeleeAttack:
                    return EPlayerState.MeleeAttacking;
                case ActionInConfig.Grenade:
                    return EPlayerState.Grenade;
                case ActionInConfig.Props:
                    return EPlayerState.Props;
                case ActionInConfig.OpenDoor:
                    return EPlayerState.OpenDoor;

                default:
                    return EPlayerState.None;
            }
        }

        private bool IsPlayerOnAir()
        {
            return _playerEntity.gamePlay.GameState == (int) (Components.GameState.AirPlane);
        }

        private bool LegalTransition(PostureInConfig from, PostureInConfig to)
        {
            // 站到蹲算到切换防止趴下切枪动作异常
            var exculde = (from == PostureInConfig.Land || to == PostureInConfig.Land || from == PostureInConfig.Crouch && to == PostureInConfig.Stand)
                        ||(from == PostureInConfig.Stand && to == PostureInConfig.Crouch);
            return !exculde;
        }
    }
}