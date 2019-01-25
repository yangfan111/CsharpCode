using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player.CharacterState;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Tuple=Core.Utils.Tuple;
using Utils.CharacterState;

namespace App.Shared.GameModules.Player.Animation
{
    public class AnimationMonitor
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AnimationMonitor));

        private readonly AnimationClipNameMatcher _matcher = new AnimationClipNameMatcher();

        private readonly Dictionary<string, StateChange> _animationFinished;
        private readonly List<Core.Utils.Tuple<string, StateChange>> _animationFinishedSource;
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
                { "Holster",            FsmInput.HolsterProgressP3},
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

            // speed up _animationFinished
            _animationFinishedSource = new List<Tuple<string, StateChange>>
			{
			    Core.Utils.Tuple.Create("JumpStart",       new StateChange { Value = false, Command = FsmInput.Freefall } ),//有过渡
			    Core.Utils.Tuple.Create("JumpEnd",         new StateChange { Value = false, Command = FsmInput.JumpEndFinished } ),//有过渡
			    Core.Utils.Tuple.Create("Fire",            new StateChange { Value = false, Command = FsmInput.FireFinished } ),
			    Core.Utils.Tuple.Create("FireEnd",         new StateChange { Value = false, Command = FsmInput.FireEndFinished } ),
			    Core.Utils.Tuple.Create("Injury",          new StateChange { Value = false, Command = FsmInput.InjuryFinished } ),
			    Core.Utils.Tuple.Create("Reload",          new StateChange { Value = false, Command = FsmInput.ReloadFinished } ),
			    Core.Utils.Tuple.Create("ReloadEmpty",     new StateChange { Value = false, Command = FsmInput.ReloadFinished } ),
			    Core.Utils.Tuple.Create("Holster",         new StateChange { Value = false, Command = FsmInput.HolsterFinished } ),
			    Core.Utils.Tuple.Create("Select",          new StateChange { Value = false, Command = FsmInput.SelectFinished } ),
			    Core.Utils.Tuple.Create("ReloadEnd",       new StateChange { Value = false, Command = FsmInput.ReloadFinished } ),//有过渡
			    Core.Utils.Tuple.Create("PickUp",          new StateChange { Value = false, Command = FsmInput.PickUpEnd } ),
			    Core.Utils.Tuple.Create("OpenDoor",        new StateChange { Value = false, Command = FsmInput.OpenDoorEnd } ),
			    Core.Utils.Tuple.Create("Props",           new StateChange { Value = false, Command = FsmInput.PropsEnd } ),
			    Core.Utils.Tuple.Create("Melee",           new StateChange { Value = false, Command = FsmInput.MeleeAttackFinished } ),
			    Core.Utils.Tuple.Create("ThrowEnd",        new StateChange { Value = false, Command = FsmInput.GrenadeEndFinish } ),
			    Core.Utils.Tuple.Create("ParachuteOpen1",  new StateChange { Value = false, Command = FsmInput.ParachuteOpen1Finished } ),
			    Core.Utils.Tuple.Create("Enter",           new StateChange { Value = false, Command = FsmInput.ToProneTransitFinish } ),
			    Core.Utils.Tuple.Create("Quit",            new StateChange { Value = false, Command = FsmInput.OutProneTransitFinish } ),
                Core.Utils.Tuple.Create("Climb",           new StateChange { Value = false, Command = FsmInput.GenericActionFinished } ),
			    Core.Utils.Tuple.Create("Use",             new StateChange { Value = false, Command = FsmInput.BuriedBombFinished } ),
			    Core.Utils.Tuple.Create("Dismantle",       new StateChange { Value = false, Command = FsmInput.DismantleBombFinished } ),
            };

            // 只播放一遍的动画，依赖它们的结束触发命令
            // 有过渡的需要打开interruption source
            _animationFinished = new Dictionary<string, StateChange>();
                
            var count = _animationFinishedSource.Count;
            for (var i = 0; i < count; i++)
            {
                var item = _animationFinishedSource[i];
                _animationFinished.Add(item.Item1, item.Item2);
            }

            _animationLoopCount = new Dictionary<string, LoopCount>
            {
                { "ReloadLoop",   new LoopCount { Value = 0, Command = FsmInput.SpecialReloadTrigger } }
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
                    bool inTransition = animator.IsInTransition(i);
                    
                    var type = _matcher.Match(GetAnimationClipName(animState,_animatorClipInfos[0].clip));
                    if (_animationFinished.ContainsKey(type) && !inTransition)
                    {
                        _animationFinished[type].Value = true;
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
                    if (_animationFinished.ContainsKey(type) && !animatorP3.IsInTransition(i) && _animationFinished[type].Value)
                    {
                        _animationFinished[type].Value = false;
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

            var count = _animationFinishedSource.Count;
            for (var i = 0; i < count; i++)
            {
                var item = _animationFinishedSource[i].Item2;
                if (item.Value)
                {
                    SetCommand(commands, item.Command);
                    item.Value = false;
                }
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
        
        class StateChange
        {
            public bool Value;
            public FsmInput Command;
        }

        class LoopCount
        {
            public int Value;
            public FsmInput Command;
        }
    }
}
