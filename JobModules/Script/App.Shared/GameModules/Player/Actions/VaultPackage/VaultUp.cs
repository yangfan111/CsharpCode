using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions.VaultPackage
{
    public class VaultUp : IAction
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VaultUp));

        private const float FixedNormalizeTime = 0.32f;
        private float _lastNormalizeTime;
        
        private PlayerEntity _player;
        private Animator _thirdPersonAnimator;
        private Animator _firstPersonAnimator;
        private GameObject _thirdPersonModel;

        private readonly GenericActionKind _kind;

        private bool _isPlayingAnimation;
        private bool _triggerActionOnce;

        public VaultUp(GenericActionKind kind)
        {
            _kind = kind;
        }

        public void Update()
        {
        }

        public void ActionInput(PlayerEntity player)
        {
            _player = player;
            _thirdPersonAnimator = player.thirdPersonAnimator.UnityAnimator;
            _thirdPersonModel = player.thirdPersonModel.Value;

            _firstPersonAnimator = player.firstPersonAnimator.UnityAnimator;

            if (CanTriggerAction && ActionConditions && !_triggerActionOnce)
            {
              //  Player.ModeController().CallBeforeAction(player.WeaponController(), EPlayerActionType.Climp);
                TriggerAnimation();
            }
        }

        private void TriggerAnimation()
        {
            Logger.InfoFormat("TriggerAnimation");

            _isPlayingAnimation = true;
            _thirdPersonAnimator.applyRootMotion = true;
            _firstPersonAnimator.applyRootMotion = true;
            _lastNormalizeTime = 0.0f;
            
            _player.stateInterface.State.StartClimb((float)_kind, () => {
                Logger.InfoFormat("Finish Vault");
                ResetConcretAction();
                _thirdPersonAnimator.applyRootMotion = false;
                _firstPersonAnimator.applyRootMotion = false;
            });

            _triggerActionOnce = true;
        }

        public void AnimationBehaviour()
        {
            if (null == _player) return;
            if (PlayingAnimation)
            {
                var normalizedTime = _thirdPersonAnimator.GetCurrentAnimatorStateInfo(GenericAction.ClimbLayerIndex).normalizedTime;
                if (!_thirdPersonAnimator.isMatchingTarget && _thirdPersonAnimator.GetCurrentAnimatorStateInfo(GenericAction.ClimbLayerIndex).IsName("ClimbStart") &&
                    normalizedTime <= FixedNormalizeTime)
                {
                    var yTranslateSpeed = ModelYTranslateOffset / FixedNormalizeTime;
                    _thirdPersonModel.transform.Translate((normalizedTime - _lastNormalizeTime) * yTranslateSpeed * Vector3.up);

                    var yRotationSpeed = ModelYRotationOffset / FixedNormalizeTime;
                    _thirdPersonModel.transform.Rotate((normalizedTime - _lastNormalizeTime) * yRotationSpeed * Vector3.up);
                    
                    _lastNormalizeTime = normalizedTime;
                }
            }
            else
            {
                _thirdPersonAnimator.applyRootMotion = false;
                _firstPersonAnimator.applyRootMotion = false;
            }
        }

        public bool PlayingAnimation
        {
            get
            {
                if (null == _player) return false;

                var playerPosture = _player.stateInterface.State.GetCurrentPostureState();

                var baseLayerInfo = _thirdPersonAnimator.GetCurrentAnimatorStateInfo(GenericAction.ClimbLayerIndex);

                if (!_isPlayingAnimation && playerPosture == PostureInConfig.Climb &&
                    (baseLayerInfo.IsName("ClimbStart") || baseLayerInfo.IsName("ClimbEnd")))
                {
                    _isPlayingAnimation = true;
                }
                else if (_isPlayingAnimation && playerPosture != PostureInConfig.Climb &&
                         (!baseLayerInfo.IsName("ClimbStart") || !baseLayerInfo.IsName("ClimbEnd")))
                    _isPlayingAnimation = false;

                return _isPlayingAnimation;
            }
        }

        public float ModelYTranslateOffset { set; get; }
        public float ModelYRotationOffset { set; get; }

        public Vector3 MatchTarget { set; get; }
        public Quaternion MatchQuaternion { set; get; }
        public bool CanTriggerAction { set; get; }

        private bool ActionConditions
        {
            get
            {
                var playerPosture = _player.stateInterface.State.GetCurrentPostureState();
                return playerPosture == PostureInConfig.Stand;
            }
        }

        public void ResetConcretAction()
        {
            CanTriggerAction = false;
            _triggerActionOnce = false;
        }
    }
}
