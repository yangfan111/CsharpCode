using System;
using System.Collections.Generic;
using Core.CharacterState.Action.CommandLimit;
using Core.Fsm;
using UnityEngine;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.Animation
{
    public class AnimationMonitor
    {
        private readonly AnimationClipNameMatcher _matcher = new AnimationClipNameMatcher();
        
        // 人物移动在人物状态更新之前，因此某些状态的触发要在Update之前
        private readonly Dictionary<string, FsmInput> _animationProgressBeforeUpdate;
        private readonly Dictionary<string, FsmInput> _p3AnimationProgressAfterUpdate;
        private readonly Dictionary<string, FsmInput> _p1AnimationProgressAfterUpdate;

        private readonly Dictionary<string, LoopCount> _animationLoopCount;

        private readonly Dictionary<int, string> _animationClipNameCache = new Dictionary<int, string>();
        
        public AnimationMonitor()
        {
            _animationProgressBeforeUpdate = new Dictionary<string, FsmInput>
            {
                { "JumpStart",    FsmInput.Freefall },
                { "JumpLoop",     FsmInput.Freefall },
            };

            _p3AnimationProgressAfterUpdate = new Dictionary<string, FsmInput>
            {
                { "JumpEnd",            FsmInput.LandProgressP3 },
                { "Fire",               FsmInput.FireProgressP3 },
                { "FireEnd",            FsmInput.FireEndProgressP3 },
                { "SightsFire",         FsmInput.SightsFireProgressP3 },
                { "Reload",             FsmInput.ReloadProgressP3 },
                { "ReloadEmpty",        FsmInput.ReloadEmptyProgressP3 },
                { "ReloadStart",        FsmInput.ReloadStartProgressP3 },
                { "ReloadLoop",         FsmInput.ReloadLoopProgressP3 },
                { "ReloadEnd",          FsmInput.ReloadEndProgressP3 },
                { "Select",             FsmInput.SelectProgressP3},
                { "HolsterEnd",         FsmInput.HolsterProgressP3},
                { "Melee",              FsmInput.MeleeAttackProgressP3},
                { "ThrowEnd",           FsmInput.ThrowEndProgressP3 },
                { "PickUp",             FsmInput.PickUpProgressP3 },
                { "OpenDoor",           FsmInput.OpenDoorProgressP3},
                { "Props",              FsmInput.PropsProgressP3},
                { "Use",                FsmInput.BuriedBombProgressP3}
            };

            _p1AnimationProgressAfterUpdate = new Dictionary<string, FsmInput>
            {
                { "JumpEnd",            FsmInput.LandProgressP1 },
                { "Fire",               FsmInput.FireProgressP1 },
                { "FireEnd",            FsmInput.FireEndProgressP1 },
                { "SightsFire",         FsmInput.SightsFireProgressP1 },
                { "Reload",             FsmInput.ReloadProgressP1 },
                { "ReloadEmpty",        FsmInput.ReloadEmptyProgressP1 },
                { "ReloadStart",        FsmInput.ReloadStartProgressP1 },
                { "ReloadLoop",         FsmInput.ReloadLoopProgressP1 },
                { "ReloadEnd",          FsmInput.ReloadEndProgressP1 },
                { "Select",             FsmInput.SelectProgressP1},
                { "Holster",            FsmInput.HolsterProgressP1},
                { "Melee",              FsmInput.MeleeAttackProgressP1},
                { "ThrowEnd",           FsmInput.ThrowEndProgressP1 },
                { "PickUp",             FsmInput.PickUpProgressP1 },
                { "OpenDoor",           FsmInput.OpenDoorProgressP1},
                { "Props",              FsmInput.PropsProgressP1},
                { "Use",                FsmInput.BuriedBombProgressP1}
            };

            _animationLoopCount = new Dictionary<string, LoopCount>
            {
                { "ReloadLoop",   new LoopCount { Value = -1, Command = FsmInput.SpecialReloadTrigger } }
            };
        }
        List<AnimatorClipInfo> _animatorClipInfos = new List<AnimatorClipInfo>();
        public void MonitorBeforeFsmUpdate(IAdaptiveContainer<IFsmInputCommand> commands, Animator animator, bool land)
        {
            var layerCount = animator.layerCount;
            for (int i = 0; i <layerCount; i++)
            {
                _animatorClipInfos.Clear();
                animator.GetCurrentAnimatorClipInfo(i,_animatorClipInfos);
                if (_animatorClipInfos.Count > 0)
                {
                    var animState = animator.GetCurrentAnimatorStateInfo(i);
                    var inTransition = animator.IsInTransition(i);
                    
                    var type = _matcher.Match(GetAnimationClipName(animState,_animatorClipInfos[0].clip));
                    if (FsmInputRelate.AnimationFinished.ContainsKey(type) && !inTransition)
                    {
                        FsmInputRelate.AnimationFinished[type].Value = true;
                    }
                    // 不在在transition的时候，才进行Land检测，因为在transiton的时候可能是下一个状态
                    if (_animationProgressBeforeUpdate.ContainsKey(type) && !inTransition)
                    {
                        SetCommand(commands, _animationProgressBeforeUpdate[type]);
                    }
                    if (_animationLoopCount.ContainsKey(type))
                    {
                        _animationLoopCount[type].Value = (int)Math.Ceiling(animState.normalizedTime);
                    }
                }
            }

            PostProcess(commands, land);
        }

        private void MonitorAnimationProgressAfterUpdate(IAdaptiveContainer<IFsmInputCommand> commands, Animator animator, 
            Dictionary<string, FsmInput> animationProgress)
        {
            var layerCount = animator.layerCount;
            for (int i = 0; i < layerCount; i++)
            {
                _animatorClipInfos.Clear();
                animator.GetCurrentAnimatorClipInfo(i,_animatorClipInfos);
                if (_animatorClipInfos.Count> 0)
                {
                    var animState = animator.GetCurrentAnimatorStateInfo(i);
                    var type = _matcher.Match(GetAnimationClipName(animState,_animatorClipInfos[0].clip));
                    if (animationProgress.ContainsKey(type))
                    {
                        SetCommand(commands,
                                   animationProgress[type],
                                   animState.normalizedTime,
                                   animState.length * (float.IsNaN(animState.speedMultiplier) ? 1.0f:animState.speedMultiplier) * 1000);
                    }
                }
            }
        }

        public void MonitorAfterFsmUpdate(IAdaptiveContainer<IFsmInputCommand> commands, Animator animatorP3, Animator animatorP1)
        {
            MonitorAnimationProgressAfterUpdate(commands, animatorP3, _p3AnimationProgressAfterUpdate);
            MonitorAnimationProgressAfterUpdate(commands, animatorP1, _p1AnimationProgressAfterUpdate);

            var layerCount = animatorP3.layerCount;
            for (int i = 0; i < layerCount; i++)
            {
                _animatorClipInfos.Clear();
                animatorP3.GetCurrentAnimatorClipInfo(i,_animatorClipInfos);
                if (_animatorClipInfos.Count > 0)
                {
                    var animState = animatorP3.GetCurrentAnimatorStateInfo(i);
                    var type = _matcher.Match(GetAnimationClipName(animState,_animatorClipInfos[0].clip));
                    if (FsmInputRelate.AnimationFinished.ContainsKey(type) && !animatorP3.IsInTransition(i) && FsmInputRelate.AnimationFinished[type].Value)
                    {
                        FsmInputRelate.AnimationFinished[type].Value = false;
                    }

                    if (_animationLoopCount.ContainsKey(type))
                    {
                        if ((int)Math.Floor(animState.normalizedTime) == _animationLoopCount[type].Value)
                        {
                            SetCommand(commands, _animationLoopCount[type].Command);
                        }
                    }
                }
            }

            foreach (var value in FsmInputRelate.AnimationFinished)
            {
                var item = value.Value;
                if (!item.Value) continue;
                SetCommand(commands, item.Command);
                item.Value = false;
            }
        }

        private void SetCommand(IAdaptiveContainer<IFsmInputCommand> commands,
                                FsmInput type,
                                float additionalValue = float.NaN,
                                float alterAdditionalValue = float.NaN)
        {
            var item = commands.GetAvailableItem(command => (command.Type == FsmInput.None || command.Type == type));
            SetCommandParam(item, type, additionalValue, alterAdditionalValue);
        }

        private void SetCommandParam(IFsmInputCommand command, FsmInput type, float additionalValue = float.NaN,
            float alterAdditionalValue = float.NaN)
        {
            command.Type = type;

            if (!float.IsNaN(additionalValue))
            {
                command.AdditioanlValue = additionalValue;
            }
            if (!float.IsNaN(alterAdditionalValue))
            {
                command.AlternativeAdditionalValue = alterAdditionalValue;
            }
        }

        private void PostProcess(IAdaptiveContainer<IFsmInputCommand> commands, bool land)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                var command = commands[i];
                switch (command.Type)
                {
                    case FsmInput.Freefall:
                        if (land)
                        {
                            command.Type = FsmInput.Land;
                        }
                        else
                        {
                            command.Type = FsmInput.None;
                        }
                        break;
                }
            }
        }

        private string GetAnimationClipName(AnimatorStateInfo state, AnimationClip clip)
        {
            if (!_animationClipNameCache.ContainsKey(state.fullPathHash))
            {
                _animationClipNameCache[state.fullPathHash] = clip.name;
            }

            return _animationClipNameCache[state.fullPathHash];
        }

        private class LoopCount
        {
            public int Value;
            public FsmInput Command;
        }
    }
}
