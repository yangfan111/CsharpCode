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
            
            var actionState = _playerEntity.stateInterface.State.GetActionState();
            switch (actionState)
            {
                case ActionInConfig.Reload:
                    _playerStates.Add(EPlayerState.Reload);
                    break;
                case ActionInConfig.SpecialFireHold:
                case ActionInConfig.Fire:
                    _playerStates.Add(EPlayerState.Firing);
                    break;
                case ActionInConfig.SpecialReload:
                    _playerStates.Add(EPlayerState.SpecialReload);
                    break;
                case ActionInConfig.SpecialFireEnd:
                    _playerStates.Add(EPlayerState.PullBolt);
                    break;
                case ActionInConfig.Gliding:
                    _playerStates.Add(EPlayerState.Gliding);
                    break;
                case ActionInConfig.Parachuting:
                    _playerStates.Add(EPlayerState.Parachuting);
                    break;
                case ActionInConfig.PickUp:
                    _playerStates.Add(EPlayerState.Pickup);
                    break;
                case ActionInConfig.SwitchWeapon:
                    _playerStates.Add(EPlayerState.SwitchWeapon);
                    break;
                case ActionInConfig.MeleeAttack:
                    _playerStates.Add(EPlayerState.MeleeAttacking);
                    break;
                case ActionInConfig.Grenade:
                    _playerStates.Add(EPlayerState.Grenade);
                    break;
                case ActionInConfig.Props:
                    _playerStates.Add(EPlayerState.Props);
                    break;
            }
            var actionKeepState = _playerEntity.stateInterface.State.GetActionKeepState();
            switch (actionKeepState)
            {
                case ActionKeepInConfig.Drive:
                    _playerStates.Add(EPlayerState.Drive);
                    break;
                case ActionKeepInConfig.Sight:
                    _playerStates.Add(EPlayerState.Sight);
                    break;
            }
            var leanState = _playerEntity.stateInterface.State.GetCurrentLeanState();
            switch (leanState)
            {
                case LeanInConfig.PeekLeft:
                    _playerStates.Add(EPlayerState.PeekLeft);
                    break;
                case LeanInConfig.PeekRight:
                    _playerStates.Add(EPlayerState.PeekRight);
                    break;
                case LeanInConfig.NoPeek:
                    break;
            }
            var poseState = _playerEntity.stateInterface.State.GetCurrentPostureState();
            var nextPoseState = _playerEntity.stateInterface.State.GetNextPostureState();
            switch (poseState)
            {
                case PostureInConfig.Crouch:
                    _playerStates.Add(EPlayerState.Crouch);
                    break;
                case PostureInConfig.Dive:
                    _playerStates.Add(EPlayerState.Dive);
                    break;
                case PostureInConfig.Jump:
                    _playerStates.Add(EPlayerState.Jump);
                    break;
                case PostureInConfig.Land:
                    CountAsStand();
                    break;
                case PostureInConfig.Prone:
                    _playerStates.Add(EPlayerState.Prone);
                    break;
                case PostureInConfig.Sight:
                    _playerStates.Add(EPlayerState.Sight);
                    break;
                case PostureInConfig.Stand:
                    _playerStates.Add(EPlayerState.Stand);
                    break;
                case PostureInConfig.Swim:
                    _playerStates.Add(EPlayerState.Swim);
                    break;
                case PostureInConfig.ProneToCrouch:
                case PostureInConfig.ProneToStand:
                case PostureInConfig.ProneTransit:
                    _playerStates.Add(EPlayerState.PostureTrans);
                    break;

            }
            if(poseState != nextPoseState && LegalTransition(poseState, nextPoseState))
            {
                _playerStates.Add(EPlayerState.PostureTrans);
            }
            var moveState = _playerEntity.stateInterface.State.GetCurrentMovementState();
            switch (moveState)
            {
                case MovementInConfig.Walk:
                    _playerStates.Add(EPlayerState.Walk);
                    break;
                case MovementInConfig.Sprint:
                    _playerStates.Add(EPlayerState.Sprint);
                    break;
                case MovementInConfig.Run:
                    _playerStates.Add(EPlayerState.Run);
                    break;
                case MovementInConfig.Injured:
                    _playerStates.Add(EPlayerState.Injured);
                    break;
                case MovementInConfig.Swim:
                    _playerStates.Add(EPlayerState.Swim);
                    break;
                case MovementInConfig.Dive:
                    _playerStates.Add(EPlayerState.Dive);
                    break;
            }
            var nextMoveState = _playerEntity.stateInterface.State.GetNextMovementState();
            //移动相关的状态开始切换的时候就算作下一个状态
            switch(nextMoveState)
            {
                case MovementInConfig.Sprint:
                    _playerStates.Add(EPlayerState.Sprint);
                    break;
                case MovementInConfig.Run:
                    _playerStates.Add(EPlayerState.Run);
                    break;
                case MovementInConfig.Walk:
                    _playerStates.Add(EPlayerState.Walk);
                    break;
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

        private bool IsPlayerOnAir()
        {
            return _playerEntity.gamePlay.GameState == (int)(Components.GameState.AirPlane);
        }

        private void CountAsStand()
        {
            _playerStates.Add(EPlayerState.Stand);
        }

        private bool LegalTransition(PostureInConfig from, PostureInConfig to)
        {
            var exculde = from == PostureInConfig.Land
                || to == PostureInConfig.Land
                || from == PostureInConfig.Crouch && to == PostureInConfig.Stand
                || from == PostureInConfig.Stand && to == PostureInConfig.Crouch;
            return !exculde; 
        }
    }
}
