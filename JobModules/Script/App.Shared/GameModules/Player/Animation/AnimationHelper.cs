using System.Collections.Generic;
using App.Shared.Components.Player;
using Core.Animation;
using Core.CharacterState;
using UnityEngine;

namespace App.Shared.GameModules.Player.Animation
{
    public static class AnimationHelper
    {
        private static List<AnimatorClipInfo> _currentClips = new List<AnimatorClipInfo>(10);
        private static List<AnimatorClipInfo> _nextClips = new List<AnimatorClipInfo>(10);
        
        public static string PrintNetworkAnimator(NetworkAnimatorComponent netAnimator, int layerIndex, int seq)
        {
            return string.Format("{0}, seq:{1}", netAnimator.AnimatorLayers[layerIndex], seq);
        }
        
        public static string PrintAnimatorParam(Animator animator)
        {
            return string.Format("FullBodySpeed:{0}, UpperBodySpeed:{1}, Speed:{2}, IsWalk:{3},Prone:{4}," +
                                 "Jump:{5}, JumpLoop:{6}",
                animator.GetFloat(AnimatorParametersHash.Instance.FullBodySpeedRatioHash),
                animator.GetFloat(AnimatorParametersHash.Instance.UpperBodySpeedRatioHash),
                animator.GetFloat(AnimatorParametersHash.Instance.MotionHash),
                animator.GetBool(AnimatorParametersHash.Instance.IsWalkHash),
                animator.GetBool(AnimatorParametersHash.Instance.ProneHash),
                animator.GetBool(AnimatorParametersHash.Instance.JumpStartHash),
                animator.GetBool(AnimatorParametersHash.Instance.FreeFallHash)
            );
        }

        public static string PrintAnimator(Animator animator, int layerIndex, int seq)
        {
            string ret = string.Empty;
            _currentClips.Clear();
            _nextClips.Clear();
            var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            animator.GetCurrentAnimatorClipInfo(layerIndex, _currentClips);
            if (animator.IsInTransition(layerIndex))
            {
                var nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(layerIndex);
                animator.GetNextAnimatorClipInfo(layerIndex, _nextClips);
                var transitionAnimationInfo = animator.GetAnimatorTransitionInfo(layerIndex);
                ret = string.Format(
                    "in transition ,layer:{0}, currentState:{7}, clip:{1}, normalizetime:{2},nextState:{8},next clip:{3}, normalizeTime:{4}, transtionNormalizeTime:{5},seq:{6}",
                    layerIndex, _currentClips.Count > 0 ? _currentClips[0].clip.name : "null",
                    currentAnimatorStateInfo.normalizedTime,
                    _nextClips[0].clip.name,
                    nextAnimatorStateInfo.normalizedTime,
                    transitionAnimationInfo.normalizedTime,
                    seq,
                    AnimatorParametersHash.Instance.GetHashString(currentAnimatorStateInfo.fullPathHash),
                    AnimatorParametersHash.Instance.GetHashString(nextAnimatorStateInfo.fullPathHash));
            }
            else
            {
                ret = string.Format("layer:{0}, currentState:{4},current clip:{1}, normalizetime:{2}, seq:{3}", layerIndex,
                    _currentClips.Count > 0 ? _currentClips[0].clip.name : "null",
                    currentAnimatorStateInfo.normalizedTime,
                    seq,
                    AnimatorParametersHash.Instance.GetHashString(currentAnimatorStateInfo.fullPathHash)
                );
            }

            return ret;
        }
    }
}