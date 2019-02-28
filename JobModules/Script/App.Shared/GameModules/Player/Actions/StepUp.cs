using App.Shared.Player;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Player.Actions
{
    public class StepUp : IAction
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(StepUp));
        private PlayerEntity _player;
        private Animator _thirdPersonAnimator;

        private bool _isPlayingAnimation;
        private bool _triggerActionOnce;

        public void Update()
        {
        }

        public void ActionInput(PlayerEntity player)
        {
            _player = player;
            _thirdPersonAnimator = player.thirdPersonAnimator.UnityAnimator;

            if (CanTriggerAction && ActionConditions && !_triggerActionOnce)
            {
                TriggerAnimation();
            }
        }

        public void TriggerAnimation()
        {
            _logger.InfoFormat("TriggerAnimation");

            _isPlayingAnimation = true;
            _thirdPersonAnimator.applyRootMotion = true;

            _player.stateInterface.State.StartClimb((float)GenericActionKind.Step, () => {
                _logger.InfoFormat("Finish Step");
                ResetConcretAction();
                _thirdPersonAnimator.applyRootMotion = false;
            });

            _triggerActionOnce = true;
        }

        public void AnimationBehaviour()
        {
            if (null == _player) return;
            if (PlayingAnimation)
            {
                if (!_thirdPersonAnimator.isMatchingTarget && _thirdPersonAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClimbStart") &&
                    _thirdPersonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f)
                    _thirdPersonAnimator.MatchTarget(MatchTarget, Quaternion.identity, AvatarTarget.LeftHand,
                        new MatchTargetWeightMask(Vector3.one, 0), 0, 0.3f);
            }
            else
            {
                _thirdPersonAnimator.applyRootMotion = false;
            }
        }

        public bool PlayingAnimation
        {
            get
            {
                if (null == _player) return false;

                var playerPosture = _player.stateInterface.State.GetCurrentPostureState();

                var baseLayerInfo = _thirdPersonAnimator.GetCurrentAnimatorStateInfo(0);
                
                if (!_isPlayingAnimation && playerPosture == PostureInConfig.Climb &&
                    (baseLayerInfo.IsName("ClimbStart") || baseLayerInfo.IsName("ClimbEnd")))
                {
                    _isPlayingAnimation = true;
                }
                else if (_isPlayingAnimation && playerPosture != PostureInConfig.Climb &&
                         (!baseLayerInfo.IsName("ClimbState") || !baseLayerInfo.IsName("ClimbEnd")))
                    _isPlayingAnimation = false;
                    

                return _isPlayingAnimation;
            }
        }

        public Vector3 MatchTarget { set; get; }
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
