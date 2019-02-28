using Core.CameraControl.NewMotor;
using System.Collections.Generic;
using App.Shared.Components.Player;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public interface IPlayerStateAdapter
    {
        HashSet<EPlayerState> GetCurrentStates();
    }

    public class PlayerStateAdapter : IPlayerStateAdapter
    {
        private PlayerEntity _playerEntity;
        private HashSet<EPlayerState> _playerStates = new HashSet<EPlayerState>(EPlayerStateComparer.Instance);

        public PlayerStateAdapter(PlayerEntity player)
        {
            _playerEntity = player;
        }

        public HashSet<EPlayerState> GetCurrentStates()
        {
            _playerStates.Clear();
            if (!_playerEntity.hasStateInterface)
            {
                return _playerStates;
            }

            var gamePlay = _playerEntity.gamePlay;
            switch ((EPlayerLifeState)gamePlay.LifeState)
            {
                case EPlayerLifeState.Dead:
                    _playerStates.Add(EPlayerState.Dead);
                    break;
                case EPlayerLifeState.Dying:
                    _playerStates.Add(EPlayerState.Dying);
                    break;
            }
            
            _playerStates.Add(ActionToState(_playerEntity.stateInterface.State.GetActionState()));
            _playerStates.Add(ActionToState(_playerEntity.stateInterface.State.GetNextActionState()));
            _playerStates.Add(KeepActionToState(_playerEntity.stateInterface.State.GetActionKeepState()));
            _playerStates.Add(LeanToState(_playerEntity.stateInterface.State.GetCurrentLeanState()));
            _playerStates.Add(MoveToState(_playerEntity.stateInterface.State.GetCurrentMovementState()));
            _playerStates.Add(MoveToState(_playerEntity.stateInterface.State.GetNextMovementState()));

            var poseState = _playerEntity.stateInterface.State.GetCurrentPostureState();
            var nextPoseState = _playerEntity.stateInterface.State.GetNextPostureState();
            _playerStates.Add(PostureToState(poseState));
            if(poseState != nextPoseState && LegalTransition(poseState, nextPoseState))
            {
                _playerStates.Add(EPlayerState.PostureTrans);
            }
            if (_playerEntity.hasOxygenEnergyInterface && _playerEntity.oxygenEnergyInterface.Oxygen.InSightDebuffState)
            {
                _playerStates.Add(EPlayerState.RunDebuff);
            }

            if(_playerEntity.hasCameraStateNew && _playerEntity.cameraStateNew.FreeNowMode == (int)ECameraFreeMode.On)
            {
                _playerStates.Add(EPlayerState.CameraFree);
            }
            if(IsPlayerOnAir())
            {
                _playerStates.Add(EPlayerState.OnAir);
            }
            return _playerStates;
        }

        private EPlayerState PostureToState(PostureInConfig posture)
        {
            switch(posture)
            {
                case PostureInConfig.Crouch:
                    return EPlayerState.Crouch;
                case PostureInConfig.Dive:
                    return EPlayerState.Dive;
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
            switch(movement)
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
                    return EPlayerState.Dive;
                default:
                    return EPlayerState.None;
            }
        }

        private EPlayerState LeanToState(LeanInConfig lean)
        {
            switch(lean)
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
            switch(keepAction)
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
                default:
                    return EPlayerState.None;
            }
        }

        private bool IsPlayerOnAir()
        {
            return _playerEntity.gamePlay.GameState == (int)(Components.GameState.AirPlane);
        }

        private bool LegalTransition(PostureInConfig from, PostureInConfig to)
        {
            var exculde = from == PostureInConfig.Land
                || to == PostureInConfig.Land
                || from == PostureInConfig.Crouch && to == PostureInConfig.Stand;
                // 站到蹲算到切换防止趴下切枪动作异常
                //|| from == PostureInConfig.Stand && to == PostureInConfig.Crouch;
            return !exculde; 
        }
    }
}
