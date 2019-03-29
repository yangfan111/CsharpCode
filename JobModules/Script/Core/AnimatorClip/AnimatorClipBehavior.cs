using Core.Compare;
using Core.Fsm;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.CharacterState;

namespace Core.Animation
{
    class AnimatorClipBehavior : IClipBehavior
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AnimatorClipBehavior));
        private  StateBehaviorParam[] _currentStateInfos = new  StateBehaviorParam[16];
        public readonly static AnimatorClipInfo NullClipInfo = new AnimatorClipInfo();
        public readonly static AnimatorStateInfo NullStateInfo = new AnimatorStateInfo();
        protected readonly AnimationClipNameMatcher _matcher = new AnimationClipNameMatcher();
        protected Action<AnimationEvent> _animationCleanEventCallback;
        List<AnimatorClipInfo> _animatorClipInfos = new List<AnimatorClipInfo>();
        
        /// <summary>
        /// 出发动画进入和退出事件
        /// </summary>
        /// <param name="animator"></param>
        public void UpdateClipBehavior(Animator animator)
        {
            if (null == animator) return;
            for (var layIndex = 0; layIndex < animator.layerCount; ++layIndex)
            {
                if ( _currentStateInfos[layIndex]==null || layIndex > _currentStateInfos.Length)
                {
                    ArrayUtility.SafeSet(ref _currentStateInfos, layIndex,  new StateBehaviorParam());
                  
                }
                var stateBehaviorParam = _currentStateInfos[layIndex];
                var stateInfo = animator.GetCurrentAnimatorStateInfo(layIndex);
                if (stateInfo.fullPathHash == stateBehaviorParam.CurrentStateInfo.fullPathHash)
                {
                    stateBehaviorParam.CurrentStateInfo = stateInfo;
                }

                //如果当前layer没有播放clip，NULL状态
                _animatorClipInfos.Clear();
                animator.GetCurrentAnimatorClipInfo(layIndex,_animatorClipInfos);
                var clipInfo = _animatorClipInfos.Count > 0 ? _animatorClipInfos[0]: NullClipInfo;

                var clipName = clipInfo.clip == null ? null : clipInfo.clip.name;
                // 打断
                if (!IsEqual(clipName, stateBehaviorParam.CurrentClipInfoName))
                {
                    stateBehaviorParam.IsEnter = !stateBehaviorParam.IsEnter; //进入退出状态
                    if (stateBehaviorParam.IsEnter)   //触发进入
                    {
                        OnClipEnter(animator, clipInfo, layIndex);
                    }
                    else    //触发退出
                    {
                        OnClipExit(animator, stateBehaviorParam.CurrentClipInfo, stateBehaviorParam.CurrentStateInfo, layIndex);
                        if (null != clipInfo.clip)
                        {
                            stateBehaviorParam.IsEnter = !stateBehaviorParam.IsEnter; //又进入下一状态
                            OnClipEnter(animator, clipInfo, layIndex);
                        }
                    }
                    stateBehaviorParam.CurrentClipInfo = clipInfo;
                    stateBehaviorParam.CurrentStateInfo = stateInfo;
                    stateBehaviorParam.CurrentClipInfoName = clipName;
                }
            }
        }

        virtual public void OnClipEnter(Animator animator, AnimatorClipInfo clipInfo, int layerIndex)
        {
            //if (clipInfo.clip)
            //    _logger.Debug(animator.name + "  AnimationClipName   ************Enter:  " + clipInfo.clip.name);
            //else
            //    _logger.Debug(animator.name + "  AnimatorEnterClipNull*******************************");
        }

        private static string _cleanStr = "Clean";
        virtual public void OnClipExit(Animator animator, AnimatorClipInfo clipInfo, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (clipInfo.clip)
            //    _logger.Debug(animator.name + "  AnimationClipName   ---------------Exit:  " + clipInfo.clip.name);
            //else
            //    _logger.Debug(animator.name + "  AnimatorExitClipNull-----------------------------------");

            // 动画结束(可能是中途打断)时
            // 判断当前动画结束时间，与动画上的AnimationEvent中的Time做比较(得知打断是否在Event之前)
            // AnimationEvent分两种  1.正常操作 2.复位操作
            var clip = clipInfo.clip;
            if (null != animator && null != clip)
            {
                // 动画结束时当前播放时间
                var normlizeTime = stateInfo.normalizedTime;
                var aimEvents = clip.events;
                var eventTimeNor = (aimEvents.Length > 0 ? aimEvents[aimEvents.Length - 1].time : 0) / clip.length;
                if (normlizeTime < eventTimeNor)
                {
                    for (var i = 0; i < aimEvents.Length; ++i)
                    {
                        var aimEvent = aimEvents[i];
                        var isClean = _matcher.Match(aimEvent.stringParameter).Equals(_cleanStr, System.StringComparison.Ordinal);
                        if (isClean)
                        {
                            if (null != _animationCleanEventCallback)
                                _animationCleanEventCallback.Invoke(aimEvent);
                            return;
                        }
                    }
                }
            }
        }

        virtual public void ChangeSpeedMultiplier(float speed, bool reset = false)
        {

        }

        virtual public void Update(Action<FsmOutput> addOutput, CharacterView view, float stateSpeedBuff)
        {
        }

        private bool IsEqual(string left, string right)
        {
            if (null == left && null == right) return true;
            if (null == left || null == right) return false;
            return left.Equals(right, System.StringComparison.Ordinal);
        }
    }

    class StateBehaviorParam
    {
        public AnimatorClipInfo CurrentClipInfo = AnimatorClipBehavior.NullClipInfo;
        public AnimatorStateInfo CurrentStateInfo = AnimatorClipBehavior.NullStateInfo;
        public string CurrentClipInfoName = null;
        public bool IsEnter = false;
    }
}
