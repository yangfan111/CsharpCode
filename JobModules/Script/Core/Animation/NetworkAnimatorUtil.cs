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
        private static List<NetworkAnimatorLayer> Cache = new List<NetworkAnimatorLayer>();

        static NetworkAnimatorUtil()
        {
            for (int i = 0; i < 10; i++)
            {
                Cache.Add(new NetworkAnimatorLayer());
            }
        }

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

            GetAnimatorLayers(animator, layerList, true);
            return layerList;
        }

        public static bool GetAnimatorLayers(Animator animator, List<NetworkAnimatorLayer> layers, bool force)
        {
            int layerCount = animator.layerCount;

            List<NetworkAnimatorLayer> tmpTarget = layers;
            if (!force)
            {
                ExpandCache(layerCount);
                tmpTarget = Cache;
            }
            
            bool currentAnimationChanged = force;

            for (int i = 0; i < layerCount; i++)
            {
                if (layers[i].LayerIndex < 0)
                    continue;
                
                var layer = tmpTarget[i];
                var layerWeight = animator.GetLayerWeight(i);
                var layerIndex = i;
                AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

                int currentStateHash = currentAnimatorStateInfo.fullPathHash;
                float normalizedTime = currentAnimatorStateInfo.normalizedTime;

                currentAnimationChanged = currentAnimationChanged || currentStateHash != layers[i].CurrentStateHash;

                //if (currentStateHash != layers[i].CurrentStateHash)
                //{
                //    Logger.InfoFormat("cur:{0}, prev:{1} is not same",currentStateHash , layers[i].CurrentStateHash);
                //}
                
                float transitionNormalizedTime = NetworkAnimatorLayer.NotInTransition;
                float transitionDuration = 0;
                if (animator.IsInTransition(layerIndex))
                {
                    transitionNormalizedTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
                    var nextState = animator.GetNextAnimatorStateInfo(i);
                    transitionDuration = transitionNormalizedTime != 0
                        ? (nextState.length * (float.IsNaN(nextState.speedMultiplier) ? 1.0f:nextState.speedMultiplier) * nextState.normalizedTime) / transitionNormalizedTime
                        : 0;
                }

                layer.SetCurrentStateInfo(layerIndex,
                                          layerWeight,
                                          currentStateHash,
                                          normalizedTime,
                                          currentAnimatorStateInfo.length * (float.IsNaN(currentAnimatorStateInfo.speedMultiplier) ? 1.0f:currentAnimatorStateInfo.speedMultiplier),
                                          transitionNormalizedTime,
                                          transitionDuration);
            }

            if (!force && currentAnimationChanged)
            {
                for (int i = 0; i < layerCount; i++)
                {
                    var layer = layers[i];
                    
                    if (layer.LayerIndex < 0)
                        continue;

                    var cache = Cache[i];
                    layer.CopyFrom(cache);
                }
            }

            return currentAnimationChanged;
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

        private static void ExpandCache(int newSize)
        {
            if (newSize > Cache.Count)
            {
                for (int i = Cache.Count; i < newSize; i++)
                {
                    Cache.Add(new NetworkAnimatorLayer());
                }
            }
        }

        public static void ForceChangeNetworkAnimator(List<NetworkAnimatorLayer> layers, int layer, float layerWeight,
            int stateHash, float normalizeTime, float stateDuration, float transitionNormalizedTime = NetworkAnimatorLayer.NotInTransition,
            float transitionDuration = 0)
        {
            try
            {
                if (layer >= 0 && layer < layers.Count)
                {
                    var animation = layers[layer];
                    animation.Weight = layerWeight;
                    animation.CurrentStateHash = stateHash;
                    animation.NormalizedTime = normalizeTime;
                    animation.StateDuration = stateDuration;
                    animation.TransitionNormalizedTime = transitionNormalizedTime;
                    animation.TransitionDuration = transitionDuration;
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
            ForceChangeNetworkAnimator(layers, NetworkAnimatorLayer.PlayerUpperBodyAddLayer, 1.0f, AnimatorParametersHash.InjureyStateHash, normalizeTime, AnimatorParametersHash.InjureyStateDuration);
        }
    }
}
