using Core;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions.VaultPackage
{
    public class VaultUp : IAction
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VaultUp));

        protected const float FixedNormalizeTime = 0.32f;
        protected float LastNormalizeTime;
        
        protected PlayerEntity Player;
        protected Animator ThirdPersonAnimator;
        protected GameObject ThirdPersonModel;

        private readonly GenericActionKind _kind;

        private bool _isPlayingAnimation;
        private bool _triggerActionOnce;

        public VaultUp(GenericActionKind kind)
        {
            _kind = kind;
        }

        public virtual void Update()
        {
        }

        public void ActionInput(PlayerEntity player)
        {
            Player = player;
            ThirdPersonAnimator = player.thirdPersonAnimator.UnityAnimator;
            ThirdPersonModel = player.thirdPersonModel.Value;

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
            ThirdPersonAnimator.applyRootMotion = true;
            LastNormalizeTime = 0.0f;
            
            Player.stateInterface.State.StartClimb((float)_kind, () => {
                Logger.InfoFormat("Finish Vault");
                ResetConcretAction();
                ThirdPersonAnimator.applyRootMotion = false;
            });

            _triggerActionOnce = true;
        }

        public virtual void AnimationBehaviour()
        {
            if (null == Player) return;
            if (PlayingAnimation)
            {
                var normalizedTime = ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (!ThirdPersonAnimator.isMatchingTarget && ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClimbStart") &&
                    normalizedTime <= FixedNormalizeTime)
                {
                    var yTranslateSpeed = ModelYTranslateOffset / FixedNormalizeTime;
                    ThirdPersonModel.transform.Translate((normalizedTime - LastNormalizeTime) * yTranslateSpeed * Vector3.up);

                    var yRotationSpeed = ModelYRotationOffset / FixedNormalizeTime;
                    ThirdPersonModel.transform.Rotate((normalizedTime - LastNormalizeTime) * yRotationSpeed * Vector3.up);
                    
                    LastNormalizeTime = normalizedTime;
                }
            }
            else
            {
                ThirdPersonAnimator.applyRootMotion = false;
            }
        }

        public bool PlayingAnimation
        {
            get
            {
                if (null == Player) return false;

                var playerPosture = Player.stateInterface.State.GetCurrentPostureState();

                var baseLayerInfo = ThirdPersonAnimator.GetCurrentAnimatorStateInfo(0);

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
                var playerPosture = Player.stateInterface.State.GetCurrentPostureState();
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
