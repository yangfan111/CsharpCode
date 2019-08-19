using System;
using System.Collections.Generic;
using Core.Compare;
using Core.Utils;
using UnityEngine;

namespace Core.Animation
{
    public static class AnimatorPoseReplayer
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AnimatorPoseReplayer));
        public static void ReplayPose(List<NetworkAnimatorLayer> layerList,
                                      List<NetworkAnimatorParameter> paramList,
                                      Animator animator)
        {
            UpdateAnimatorParameters(paramList, animator);
            UpdateAnimatorLayers(layerList, animator);
        }

        private static void UpdateAnimatorParameters(List<NetworkAnimatorParameter> paramList, Animator animator)
        {
            for (var i = 0; i < paramList.Count; i++)
            {
                var param = paramList[i];

                if (AnimatorControllerParameterType.Float == param.ParamType)
                {
                    animator.SetFloat(param.NameHash, param.FloatValue);
                }
                else if (AnimatorControllerParameterType.Bool == param.ParamType)
                {
                    animator.SetBool(param.NameHash, param.BoolValue);
                }else if (AnimatorControllerParameterType.Int == param.ParamType)
                {
                    animator.SetInteger(param.NameHash, param.IntValue);
                }
                else
                {
                    throw new Exception("animator trigger not supported for server&client sync");
                }
            }
        }

        private static void UpdateAnimatorLayers(List<NetworkAnimatorLayer> layers, Animator animator)
        {
            var layerCount = layers.Count;
            for (int i = 0; i < layerCount; ++i)
            {
                var layer = layers[i];
                if (layer.LayerIndex < 0)
                    continue;

                animator.SetLayerWeight(layer.LayerIndex, layer.Weight);
                if (layer.TransitionNormalizedTime < 0)
                    animator.Play(layer.CurrentStateFullPathHash, layer.LayerIndex, layer.CurrentStateNormalizedTime);
                else
                {
                    animator.PlayTransition(layer.LayerIndex,
                        layer.CurrentStateFullPathHash,
                        layer.TransitionFullPathHash,
                        layer.TransitionIndex,
                        layer.TransitionStartTime,
                        layer.CurrentStateNormalizedTime,
                        layer.NextStateNormalizedTime,
                        layer.TransitionNormalizedTime);
                }
            }
            
            animator.UpdateAndCacheDatas(0);
        }
    }
}
