using System;
using System.Collections.Generic;
using Core.Compare;
using Core.Utils;
using UnityEngine;

namespace Core.Animation
{
    public class AnimatorPoseReplayer
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AnimatorPoseReplayer));

        public void ReplayPose(List<NetworkAnimatorLayer> layerList,
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
                }else if (AnimatorControllerParameterType.Trigger == param.ParamType ||
                    AnimatorControllerParameterType.Int == param.ParamType)
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
            float maxTransitionTime = 0;
            // check whether Transition exists or not
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];
                if (layer.LayerIndex >= 0)
                {
                    
                    if (!CompareUtility.IsApproximatelyEqual(layer.TransitionNormalizedTime, NetworkAnimatorLayer.NotInTransition))
                    {
                        maxTransitionTime = Mathf.Max(maxTransitionTime, layer.StateDuration * (layer.NormalizedTime - (float)Math.Floor(layer.NormalizedTime)));
                    }
                    animator.SetLayerWeight(layer.LayerIndex, layer.Weight);
                }
            }
            
            if (CompareUtility.IsApproximatelyEqual(maxTransitionTime, 0))
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    var layer = layers[i];
                    if (layer.LayerIndex >= 0)
                    {
                        animator.Play(layer.CurrentStateHash, layer.LayerIndex, layer.NormalizedTime);
                    }
                }
            }
            else
            {
                List<SortedLayer> sortedLayers = new List<SortedLayer>();
                for (int i = 0; i < layers.Count; i++)
                {
                    if (layers[i].LayerIndex >= 0)
                    {
                        if (CompareUtility.IsApproximatelyEqual(layers[i].TransitionNormalizedTime, NetworkAnimatorLayer.NotInTransition))
                        {
                            sortedLayers.Add(new SortedLayer
                            {
                                LayerIndex = layers[i].LayerIndex,
                                TransitionTime = layers[i].StateDuration * (layers[i].NormalizedTime - (float)Math.Floor(layers[i].NormalizedTime))
                            });
                        }
                        else
                        {
                            sortedLayers.Add(new SortedLayer
                            {
                                LayerIndex = layers[i].LayerIndex,
                                TransitionTime = 0
                            });
                        }
                    }
                }
                sortedLayers.Sort((x, y) =>
                {
                    if (x.TransitionTime > y.TransitionTime)
                    {
                        return -1;
                    }
                    if (x.TransitionTime < y.TransitionTime)
                    {
                        return 1;
                    }
                    return 0;
                });

                for (int i = 0; i < sortedLayers.Count; i++)
                {
                    var layerIndex = sortedLayers[i].LayerIndex;
                    var layer = layers[layerIndex];
                    float deltaTime = 0.0f;
                    
                    if (!CompareUtility.IsApproximatelyEqual(layer.TransitionNormalizedTime, NetworkAnimatorLayer.NotInTransition))
                    {
                        if (i != sortedLayers.Count - 1)
                        {
                            deltaTime = sortedLayers[i].TransitionTime - sortedLayers[i + 1].TransitionTime;
                        }
                        else
                        {
                            deltaTime = sortedLayers[i].TransitionTime;
                        }

                        animator.Play(layer.CurrentStateHash, layerIndex, 0);

                        // make normalized time effect
                        animator.Update(0, false);
                        animator.Update(deltaTime, false);
                    }
                    else
                    {
                        animator.Play(layer.CurrentStateHash, layerIndex, layer.NormalizedTime);
                        animator.Update(0, false);
                    }
                }
            }
            
            animator.UpdateAndCacheDatas(0);
        }

        struct SortedLayer
        {
            public int LayerIndex;
            public float TransitionTime;
        }
    }
}
