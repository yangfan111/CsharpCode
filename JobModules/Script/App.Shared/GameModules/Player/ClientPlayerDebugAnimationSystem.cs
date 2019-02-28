using System;
using System.Collections.Generic;
using App.Shared.Util;
using Core.CharacterState;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class ClientPlayerDebugAnimationSystem:AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientPlayerDebugAnimationSystem));
        private static List<AnimatorClipInfo> _currentClips = new List<AnimatorClipInfo>(10);
        private static List<AnimatorClipInfo> _nextClips = new List<AnimatorClipInfo>(10);
        private static readonly int DefaultLayerIndex = 0;

        protected override bool filter(PlayerEntity playerEntity)
        {
            return SharedConfig.DebugAnimation && playerEntity.hasThirdPersonAnimator;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var animator = playerEntity.thirdPersonAnimator.UnityAnimator;
            var seq = cmd.Seq;
            int layerIndex = DefaultLayerIndex;
            try
            {
                PrintAnimator(animator, layerIndex, seq);
                PrintAnimatorParam(animator);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void PrintAnimatorParam(Animator animator)
        {
            Logger.InfoFormat("FullBodySpeed:{0}, UpperBodySpeed:{1}",
                animator.GetFloat(AnimatorParametersHash.Instance.FullBodySpeedRatioHash),
                animator.GetFloat(AnimatorParametersHash.Instance.UpperBodySpeedRatioHash));
        }

        private static void PrintAnimator(Animator animator, int layerIndex, int seq)
        {
            _currentClips.Clear();
            _nextClips.Clear();
            var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            animator.GetCurrentAnimatorClipInfo(layerIndex, _currentClips);
            if (animator.IsInTransition(layerIndex))
            {
                var nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(layerIndex);
                animator.GetNextAnimatorClipInfo(layerIndex, _nextClips);
                var transitionAnimationInfo = animator.GetAnimatorTransitionInfo(layerIndex);
                Logger.InfoFormat(
                    "in transition ,layer:{0}, currentState:{7}, clip:{1}, normalizetime:{2},nextState:{8},next clip:{3}, normalizeTime:{4}, transtionNormalizeTime:{5},seq:{6}",
                    layerIndex, _currentClips.Count > 0 ? _currentClips[0].clip.name : "null",
                    currentAnimatorStateInfo.normalizedTime,
                    _nextClips[0].clip.name,
                    nextAnimatorStateInfo.normalizedTime,
                    transitionAnimationInfo.normalizedTime,
                    seq,
                    currentAnimatorStateInfo.fullPathHash,
                    nextAnimatorStateInfo.fullPathHash);
            }
            else
            {
                Logger.InfoFormat("layer:{0}, currentState:{4},current clip:{1}, normalizetime:{2}, seq:{3}", layerIndex,
                    _currentClips.Count > 0 ? _currentClips[0].clip.name : "null",
                    currentAnimatorStateInfo.normalizedTime,
                    seq,
                    currentAnimatorStateInfo.fullPathHash
                );
            }
        }
    }
}