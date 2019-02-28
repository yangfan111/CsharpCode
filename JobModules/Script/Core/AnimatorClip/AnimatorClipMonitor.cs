using Assets.Utils.Configuration;
using Core.CharacterState;
using Core.Fsm;
using System;
using System.Collections.Generic;
using Core.Animation;
using UnityEngine;
using Utils.CharacterState;
using Utils.Singleton;
using WeaponConfigNs;

namespace Core.AnimatorClip
{
    class AnimatorClipMonitor : AnimatorClipBehavior
    {
        private List<AnimatorStateItem> _animatorClips = new List<AnimatorStateItem>();

        public void SetAnimationCleanEventCallback(Action<AnimationEvent> action)
        {
            _animationCleanEventCallback = action;
        }

        public void SetAnimatorClipsTime(int? weaponId)
        {
            if (weaponId.HasValue)   //有枪
            {
                var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weaponId.Value);
                if(null != config)
                    _animatorClips = config.AniStates;
            }
            else
            {
                _animatorClips = null;
            }
        }

        public override void Update(Action<FsmOutput> addOutput, CharacterView view, float stateSpeedBuff)
        {
            base.Update(addOutput, view, stateSpeedBuff);
            for (int i = 0; i < _currentCommandIndex; i++)
            {
                _outerCommand[i].Execute(addOutput, view, stateSpeedBuff);
            }
            _currentCommandIndex = 0;
        }

        public override void OnClipEnter(Animator animator, AnimatorClipInfo clipInfo, int layerIndex)
        {
            base.OnClipEnter(animator, clipInfo, layerIndex);

            CalcSpeedMultiplier(clipInfo);
        }

        public override void OnClipExit(Animator animator, AnimatorClipInfo clipInfo, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnClipExit(animator, clipInfo, stateInfo, layerIndex);

            ResetSpeedMultiplier(clipInfo);
        }

        public override void ChangeSpeedMultiplier(float speed, bool reset = false)
        {
            var cmd = GetAvailableCommand();
            cmd.SetCommand(SetCommand, speed, reset);
        }

        private void CalcSpeedMultiplier(AnimatorClipInfo clipInfo)
        {
            var clip = clipInfo.clip;
            if (null == clip || null == _animatorClips) return;
            var cilpName = _matcher.Match(clip.name);
            foreach (var item in _animatorClips)
            {
                if (item.StateName.Equals(cilpName))
                {
                    var animationSpeed = clip.length / item.StateTime;
                    ChangeSpeedMultiplier(animationSpeed);
                    break;
                }
            }
        }

        private void ResetSpeedMultiplier(AnimatorClipInfo clipInfo)
        {
            var clip = clipInfo.clip;
            if (null == clip || null == _animatorClips) return;
            var cilpName = _matcher.Match(clip.name);
            foreach (var item in _animatorClips)
            {
                if (item.StateName.Equals(cilpName))
                {
                    ChangeSpeedMultiplier(AnimatorParametersHash.DefaultAnimationSpeed, true);
                    break;
                }
            }
        }

        private void SetCommand(Action<FsmOutput> addOutput, float additionalValue = float.NaN, CharacterView view = CharacterView.EndOfTheWorld)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UpperBodySpeedRatioHash,
                                          AnimatorParametersHash.Instance.UpperBodySpeedRatioName,
                                          additionalValue,
                                          view);
            addOutput(FsmOutput.Cache);
        }

        #region command
        private readonly List<Command> _outerCommand = new List<Command>();
        private int _currentCommandIndex;
        private Command GetAvailableCommand()
        {
            if (_currentCommandIndex >= _outerCommand.Count)
            {
                _outerCommand.Add(new Command());
            }

            return _outerCommand[_currentCommandIndex++];
        }

        class Command
        {
            private float _additionalValue;
            private bool _reset;
            private Action<Action<FsmOutput>, float, CharacterView> _action;

            public void SetCommand(Action<Action<FsmOutput>, float, CharacterView> action, float additionValue = float.NaN, bool reset = false)
            {
                _action = action;
                _additionalValue = additionValue;
                _reset = reset;
            }

            public void Execute(Action<FsmOutput> addOutput, CharacterView view, float stateSpeedBuff)
            {
                if (null != _action)
                {
                    var speed = _reset ? _additionalValue : _additionalValue * stateSpeedBuff;
                    _action.Invoke(addOutput, speed, view);
                    _action = null;
                }
            }
        }
        #endregion
    }
}
