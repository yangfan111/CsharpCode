using Core.Animation;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions.LadderPackage
{
    public class ClimbLadder
    {
        private PlayerEntity _player;
        private Animator _thirdPersonAnimator;

        private LadderLocation _toLadderLocation = LadderLocation.Null;
        private LadderKind _ladderKind = LadderKind.Null;

        private bool _isUsingLadder = false;
        private bool _triggerEnterOnce;
        private bool _triggerExitOnce;

        private float _ladderSpeed;

        public void SetPlayerData(PlayerEntity player)
        {
            _player = player;
            _thirdPersonAnimator = _player.thirdPersonAnimator.UnityAnimator;
        }

        public void Update(LadderLocation ladderLocation)
        {
            TestExitLadderImpl(ladderLocation);
        }

        public void UpdateLadderSpeed(float speed)
        {
            _ladderSpeed = speed;
            if (null == _player || !_isUsingLadder) return;
            _player.stateInterface.State.SetLadderSpeed(speed);
        }

        public void TestEnterLadder(LadderLocation location)
        {
            TestEnterLadderImpl(location);
        }

        public void ResetPlayerSetting()
        {
            Reset();
            if(null != _player)
                _player.stateInterface.State.InterruptLadder();
        }

        private void Reset()
        {
            _isUsingLadder = false;
            _triggerEnterOnce = false;
            _triggerExitOnce = false;
            if(null != _thirdPersonAnimator)
                _thirdPersonAnimator.applyRootMotion = false;
            _toLadderLocation = LadderLocation.Null;
            _ladderKind = LadderKind.Null;
        }
        
        private void TriggerEnterLadder()
        {
            _triggerEnterOnce = true;
            _isUsingLadder = true;
            _thirdPersonAnimator.applyRootMotion = true;

            _player.stateInterface.State.EnterLadder((float) _toLadderLocation, () =>
            {
                _triggerEnterOnce = false; 
                _ladderKind = LadderKind.Null;
            });
        }
        
        private void TriggerMiddleEnterLadder()
        {
            _triggerEnterOnce = false;
            _isUsingLadder = true;
            _thirdPersonAnimator.applyRootMotion = true;
            _ladderKind = LadderKind.Null;

            _player.stateInterface.State.MiddleEnterLadder();
        }

        private void TriggerExitLadder()
        {
            _triggerExitOnce = true;
            _player.stateInterface.State.ExitLadder((float) _toLadderLocation, () =>
            {
                _triggerExitOnce = false;
                _isUsingLadder = false;
                _thirdPersonAnimator.applyRootMotion = false;
                _ladderKind = LadderKind.Null;
            });
        }
        
        private void TestEnterLadderImpl(LadderLocation location)
        {
            _toLadderLocation = location;
            if (_isUsingLadder || _triggerEnterOnce || _triggerExitOnce) return;

            if (_ladderSpeed > 0.05f)
            {
                if (LadderLocation.Bottom == location ||
                    LadderLocation.Top == location)
                {
                    _ladderKind = LadderKind.Enter;
                    TriggerEnterLadder();
                }
                else if(LadderLocation.Middle == location)
                    TriggerMiddleEnterLadder();
            }
        }

        private void TestExitLadderImpl(LadderLocation ladderLocation)
        {
            _toLadderLocation = ladderLocation;
            if(!_isUsingLadder || _triggerEnterOnce || _triggerExitOnce) return;

            if (LadderLocation.Bottom == ladderLocation && _ladderSpeed < -0.05f ||
                (LadderLocation.Top == ladderLocation && _ladderSpeed > 0.05f))
            {
                _ladderKind = LadderKind.Exit;
                TriggerExitLadder();
            }
        }

        public bool IsUsingLadder
        {
            get
            {
                if (null == _player || null == _thirdPersonAnimator) return false;

                var playerMovement = _player.stateInterface.State.GetCurrentMovementState();
                var baseLayerInfo = _thirdPersonAnimator.GetCurrentAnimatorStateInfo(NetworkAnimatorLayer.LadderLayer);

                if (!_isUsingLadder && IsMovementLadder(playerMovement) &&
                    AnimatorStateIsLadder(baseLayerInfo))
                    _isUsingLadder = true;
                else if (_isUsingLadder && (!IsMovementLadder(playerMovement) ||
                         !AnimatorStateIsLadder(baseLayerInfo)))
                    _isUsingLadder = false;
                
                return _isUsingLadder;
            }
        }

        public bool NeedCheckLadderKind()
        {
            return _ladderKind == LadderKind.Null;
        }

        private bool IsMovementLadder(MovementInConfig movement)
        {
            return movement == MovementInConfig.Ladder ||
                   movement == MovementInConfig.EnterLadder;
        }

        private bool AnimatorStateIsLadder(AnimatorStateInfo info)
        {
            return info.IsName("EnterLadderTop") ||
                   info.IsName("EnterLadderBottom") ||
                   info.IsName("ExitLadderTop") ||
                   info.IsName("ExitLadderBottom") ||
                   info.IsName("ClimbLadder");
        }

        private enum LadderKind
        {
            Null,
            Enter,
            Ladder,
            Exit
        }
    }
}
