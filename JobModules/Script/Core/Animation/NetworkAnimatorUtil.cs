using Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using Core.CharacterState;
using UnityEngine;

namespace Core.Animation
{
    public class NetworkAnimatorUtil
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NetworkAnimatorUtil));

        public static List<NetworkAnimatorLayer> CreateAnimatorLayers(Animator animator)
        {
            int layerCount = animator.layerCount;
            List<NetworkAnimatorLayer> layerList = new List<NetworkAnimatorLayer>(layerCount);

            for (int i = 0; i < layerCount; i++)
            {
                if (animator.GetLayerName(i) == "UpperBody Layer")
                    layerList.Add(new NetworkAnimatorLayer(-1));
                else
                    layerList.Add(new NetworkAnimatorLayer());
            }

            GetAnimatorLayers(animator, layerList);
            return layerList;
        }

        public static void GetAnimatorLayers(Animator animator, List<NetworkAnimatorLayer> layers)
        {
            int layerCount = animator.layerCount;
            
            for (int i = 0; i < layerCount; i++)
            {
                if (layers[i].LayerIndex < 0)
                    continue;
                
                var layer = layers[i];
                var layerWeight = animator.GetLayerWeight(i);

                var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
                float currentStateNormalizedTime = currentAnimatorStateInfo.normalizedTime;
                int transitionFullPathHash = -1;
                byte transitionIndex = 0;
                float transitionNormalizedTime = NetworkAnimatorLayer.NotInTransition;
                float nextStateNormalizedTime = -1;
                float transitionStartTime = -1;

                if (animator.IsInTransition(i))
                {
                    var transitionInfo = animator.GetAnimatorTransitionInfo(i);
                    currentStateNormalizedTime = transitionInfo.currentStateTime;
                    transitionFullPathHash = transitionInfo.fullPathHash;
                    transitionIndex = (byte)transitionInfo.index;
                    transitionStartTime = transitionInfo.startTime;
                    nextStateNormalizedTime = transitionInfo.nextStateTime;
                    transitionNormalizedTime = transitionInfo.normalizedTime;
                }

                layer.SetCurrentStateInfo(i,
                    layerWeight,
                    currentAnimatorStateInfo.fullPathHash,
                    currentStateNormalizedTime,
                    transitionFullPathHash,
                    transitionIndex,
                    transitionStartTime,
                    nextStateNormalizedTime,
                    transitionNormalizedTime,
                    currentAnimatorStateInfo.length);
            }
        }

        public static List<NetworkAnimatorParameter> GetAnimatorParams(Animator animator)
        {
            List<NetworkAnimatorParameter> dumppedparamList = new List<NetworkAnimatorParameter>();
            AnimatorControllerParameter[] paramList = animator.parameters;
            //Logger.InfoFormat("Parameters Count; {0}", paramList.Length);

            for (int i = 0; i < paramList.Length; i++)
            {
                AnimatorControllerParameter param = paramList[i];
                NetworkAnimatorParameter copyedParam = null;
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        bool boolVal = animator.GetBool(param.nameHash);
                        copyedParam = new NetworkAnimatorParameter(param.type, boolVal,param.nameHash);
                        break;
                    case AnimatorControllerParameterType.Float:
                        float floatVal = animator.GetFloat(param.nameHash);
                        copyedParam = new NetworkAnimatorParameter(param.type, floatVal, param.nameHash);
                        break;
                    case AnimatorControllerParameterType.Int:
                        int intVal = animator.GetInteger(param.nameHash);
                        copyedParam = new NetworkAnimatorParameter(param.type, intVal, param.nameHash);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        throw new Exception("not supported");
                }
                
                dumppedparamList.Add(copyedParam);
            }

            return dumppedparamList;
        }

        public static void ForceChangeNetworkAnimator(List<NetworkAnimatorLayer> layers, int layer, float layerWeight,
            int stateHash, float normalizeTime)
        {
            try
            {
                if (layer >= 0 && layer < layers.Count)
                {
                    var animation = layers[layer];
                    animation.Weight = layerWeight;
                    animation.CurrentStateFullPathHash = stateHash;
                    animation.CurrentStateNormalizedTime = normalizeTime;
                    animation.TransitionNormalizedTime = NetworkAnimatorLayer.NotInTransition;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ForceToInjureState(List<NetworkAnimatorLayer> layers, float normalizeTime)
        {
            ForceChangeNetworkAnimator(layers, NetworkAnimatorLayer.PlayerUpperBodyAddLayer, 1.0f, AnimatorParametersHash.InjureyStateHash, normalizeTime);
        }
    }
}
