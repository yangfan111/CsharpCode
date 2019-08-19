using System;
using System.Collections.Generic;
using System.Text;
using Core.Animation;
using Core.Playback;
using Core.Prediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    public abstract class AbstractNetworkAnimator:IInterpolatableComponent, IRewindableComponent,ICloneableComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractNetworkAnimator));
        
        [NetworkProperty] [DontInitilize] public bool NeedChangeServerTime;
        [NetworkProperty] [DontInitilize] public int BaseServerTime;
        
        [NetworkProperty] public List<NetworkAnimatorLayer> AnimatorLayers;
//        流量优化用
//        [NetworkProperty] [DontInitilize(false)] public List<CompressedNetworkAnimatorParameter> FloatAnimatorParameters;
        
        private List<NetworkAnimatorParameter> _animatorParameters;
        [NetworkProperty] public List<NetworkAnimatorParameter> AnimatorParameters
        {
            set
            {
                _animatorParameters = value;
                _animatorParameterIndex = null;
//                流量优化用                
//                if (FloatAnimatorParameters == null)
//                    FloatAnimatorParameters = new List<CompressedNetworkAnimatorParameter>();
//
//                ConvertStructureDataToCompressData();
            }
            get
            {
                return _animatorParameters;
            }
        }

        public void SetAnimatorParamsWithoutChangeData(List<NetworkAnimatorParameter> animatorParameters)
        {
            _animatorParameters = animatorParameters;
        }

        private Dictionary<int, int> _animatorParameterIndex;
        public Dictionary<int, int> AnimatorParameterIndex
        {
            get
            {
                if (_animatorParameterIndex == null)
                {
                    _animatorParameterIndex = new Dictionary<int, int>();
                    for (int i = 0; i < AnimatorParameters.Count; i++)
                        _animatorParameterIndex.Add(AnimatorParameters[i].NameHash, i);
                }

                return _animatorParameterIndex;
            }
        }
        
//        流量优化用
//        public void ConvertStructureDataToCompressData()
//        {
//            var paramCount = AnimatorParameters.Count;
//            int floatParamCount = 0;
//            for (int i = 0; i < paramCount; ++i)
//            {
//                var param = AnimatorParameters[i];
//                if (param.ParamType == AnimatorControllerParameterType.Float)
//                {
//                    short fixedPointValue = (short) (param.FloatValue * 1000);
//                    if (floatParamCount < FloatAnimatorParameters.Count)
//                        FloatAnimatorParameters[floatParamCount].Value = fixedPointValue;
//                    else
//                        FloatAnimatorParameters.Add(new CompressedNetworkAnimatorParameter { Value = fixedPointValue });
//
//                    floatParamCount++;
//                }
//            }
//        }
//
//        public void ConvertCompressDataToStructureData()
//        {
//            var paramCount = AnimatorParameters.Count;
//            var dataCount = FloatAnimatorParameters.Count;
//            int floatParamIndex = 0;
//            for (int i = 0; i < paramCount && floatParamIndex < dataCount; ++i)
//            {
//                var param = AnimatorParameters[i];
//                if (param.ParamType == AnimatorControllerParameterType.Float)
//                    param.FloatValue = FloatAnimatorParameters[floatParamIndex++].Value / 1000.0f;
//            }
//        }
        
        // for debug
        private string _entityName = string.Empty;

        public void SetEntityName(string name)
        {
            _entityName = name;
        }
        
        private void InitFields(AbstractNetworkAnimator leftComponent)
        {
            if (AnimatorLayers == null || AnimatorLayers.Count != leftComponent.AnimatorLayers.Count)
            {
                AnimatorLayers = new List<NetworkAnimatorLayer>(leftComponent.AnimatorLayers.Count);

                foreach (var layer in leftComponent.AnimatorLayers)
                {
                    AnimatorLayers.Add(layer.Clone());
                }
            }

            if (AnimatorParameters == null || AnimatorParameters.Count != leftComponent.AnimatorParameters.Count)
            {
                AnimatorParameters = new List<NetworkAnimatorParameter>(leftComponent.AnimatorParameters.Count);

                foreach (var parameter in leftComponent.AnimatorParameters)
                {
                    AnimatorParameters.Add(parameter.Clone());
                }
            }
            
//            if (FloatAnimatorParameters == null || FloatAnimatorParameters.Count != leftComponent.FloatAnimatorParameters.Count)
//            {
//                FloatAnimatorParameters = new List<CompressedNetworkAnimatorParameter>(leftComponent.FloatAnimatorParameters.Count);
//
//                foreach (var param in leftComponent.FloatAnimatorParameters)
//                {
//                    FloatAnimatorParameters.Add(param.Clone());
//                }
//            }
        }
        
        public bool IsInterpolateEveryFrame(){ return true; }
        public virtual void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            AbstractNetworkAnimator leftComponent = (AbstractNetworkAnimator)left;
            AbstractNetworkAnimator rightComponent = (AbstractNetworkAnimator)right;

            InitFields(leftComponent);

            BaseServerTime = leftComponent.BaseServerTime;
            NeedChangeServerTime = leftComponent.NeedChangeServerTime;
            
            // 当动画状态机切换时（人<->怪物）,不做插值，使用left，目前未对动画状态机切换做处理
            // 此处只是避免出错
            if (leftComponent.AnimatorLayers.Count == rightComponent.AnimatorLayers.Count)
            {
                AssignAnimatorLayers(leftComponent.AnimatorLayers,
                    rightComponent.AnimatorLayers,
                    leftComponent.BaseServerTime,
                    interpolationInfo.CurrentRenderTime,
                    rightComponent.BaseServerTime,
                    interpolationInfo.Ratio);
//                AssignAnimatorParameters(leftComponent.FloatAnimatorParameters, rightComponent.FloatAnimatorParameters,
//                    interpolationInfo.Ratio);
                AssignAnimatorParameters(leftComponent.AnimatorParameters, rightComponent.AnimatorParameters, interpolationInfo.Ratio);
            }
        }
        
        // @see http://192.168.0.6:8090/pages/viewpage.action?pageId=12046047
        private void AssignAnimatorLayers(List<NetworkAnimatorLayer> leftLayers,
                                          List<NetworkAnimatorLayer> rightLayers,
                                          int leftBaseServerTime,
                                          int currentRenderTime,
                                          int rightBaseServerTime,
                                          float ratio)
        {
            _logger.DebugFormat("currentTime:{0}, leftBaseTime:{1}, rightBaseTime:{2}",
                currentRenderTime,
                leftBaseServerTime,
                rightBaseServerTime);
            AssertUtility.Assert(AnimatorLayers.Count == leftLayers.Count);
            var elapsedTimeSinceStableState = (currentRenderTime - leftBaseServerTime) * 0.001f;

            for (int i = 0; i < AnimatorLayers.Count; i++)
            {
                var interpolatedLayer = AnimatorLayers[i];
                var leftLayer = leftLayers[i];
                var rightLayer = rightLayers[i];

                // 服务端下发的动画数据不够及时，缺乏有效数据进行内插，改为外插
                if (leftBaseServerTime == rightBaseServerTime)
                {
                    interpolatedLayer.SetCurrentStateInfo(
                        leftLayer.LayerIndex,
                        leftLayer.Weight,
                        leftLayer.CurrentStateFullPathHash,
                        leftLayer.CurrentStateNormalizedTime + elapsedTimeSinceStableState / leftLayer.StateDuration,
                        leftLayer.TransitionFullPathHash,
                        leftLayer.TransitionIndex,
                        leftLayer.TransitionStartTime,
                        leftLayer.NextStateNormalizedTime,
                        leftLayer.TransitionNormalizedTime,
                        leftLayer.StateDuration);
                }
                else // 动画正常播放
                {
                    bool leftInTransition = leftLayer.TransitionNormalizedTime >= 0;
                    bool rightInTransition = rightLayer.TransitionNormalizedTime >= 0;

                    // 如果要完全复现Transition的开始/结束点，需要Transition的长度，以及当前动画播放速率。
                    // 动画数据以实时帧率产生，两个Snapshot之间间隔较短
                    // 因此简化为Transition的开始/结束发生在两个Snapshot正中间
                    if (!leftInTransition && rightInTransition) // 进入Transition
                    {
                        if (ratio <= 0.5f) // stable state
                        {
                            interpolatedLayer.SetCurrentStateInfo(
                                leftLayer.LayerIndex,
                                leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                leftLayer.CurrentStateFullPathHash,
                                leftLayer.CurrentStateNormalizedTime + (rightLayer.CurrentStateNormalizedTime - leftLayer.CurrentStateNormalizedTime) * ratio,
                                leftLayer.TransitionFullPathHash,
                                leftLayer.TransitionIndex,
                                leftLayer.TransitionStartTime,
                                leftLayer.NextStateNormalizedTime,
                                leftLayer.TransitionNormalizedTime,
                                leftLayer.StateDuration);
                        }
                        else // transition
                        {
                            interpolatedLayer.SetCurrentStateInfo(
                                leftLayer.LayerIndex,
                                leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                rightLayer.CurrentStateFullPathHash,
                                leftLayer.CurrentStateNormalizedTime + (rightLayer.CurrentStateNormalizedTime - leftLayer.CurrentStateNormalizedTime) * ratio,
                                rightLayer.TransitionFullPathHash,
                                rightLayer.TransitionIndex,
                                rightLayer.TransitionStartTime,
                                rightLayer.NextStateNormalizedTime * (ratio - 0.5f) * 2,
                                rightLayer.TransitionNormalizedTime * (ratio - 0.5f) * 2,
                                rightLayer.StateDuration);
                        }
                    }
                    else if (leftInTransition && !rightInTransition) // 退出Transition
                    {
                        if (ratio >= 0.5f) // stable state
                        {
                            interpolatedLayer.SetCurrentStateInfo(
                                leftLayer.LayerIndex,
                                leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                rightLayer.CurrentStateFullPathHash,
                                leftLayer.NextStateNormalizedTime + (rightLayer.CurrentStateNormalizedTime - leftLayer.NextStateNormalizedTime) * ratio,
                                rightLayer.TransitionFullPathHash,
                                rightLayer.TransitionIndex,
                                rightLayer.TransitionStartTime,
                                rightLayer.NextStateNormalizedTime,
                                rightLayer.TransitionNormalizedTime,
                                rightLayer.StateDuration);
                        }
                        else // transition
                        {
                            interpolatedLayer.SetCurrentStateInfo(
                                leftLayer.LayerIndex,
                                leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                leftLayer.CurrentStateFullPathHash,
                                // rightLayer中没有关于leftLayer.CurrentStateNormalizedTime的信息，无法插值
                                leftLayer.CurrentStateNormalizedTime,
                                leftLayer.TransitionFullPathHash,
                                leftLayer.TransitionIndex,
                                leftLayer.TransitionStartTime,
                                leftLayer.NextStateNormalizedTime + (rightLayer.CurrentStateNormalizedTime - leftLayer.NextStateNormalizedTime) * ratio,
                                leftLayer.TransitionNormalizedTime + (1 - leftLayer.TransitionNormalizedTime) * ratio * 2,
                                leftLayer.StateDuration);
                        }
                    }
                    else // 前后两帧无Transition，或者都处于Transition中
                    {
                        // 两个状态硬切
                        if (leftLayer.CurrentStateFullPathHash != rightLayer.CurrentStateFullPathHash)
                        {
                            interpolatedLayer.SetCurrentStateInfo(
                                rightLayer.LayerIndex,
                                rightLayer.Weight,
                                rightLayer.CurrentStateFullPathHash,
                                rightLayer.CurrentStateNormalizedTime * ratio,
                                rightLayer.TransitionFullPathHash,
                                rightLayer.TransitionIndex,
                                rightLayer.TransitionStartTime,
                                rightLayer.NextStateNormalizedTime,
                                rightLayer.TransitionNormalizedTime,
                                rightLayer.StateDuration);                           
                        }
                        else
                        {
                            interpolatedLayer.SetCurrentStateInfo(
                                leftLayer.LayerIndex,
                                leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                leftLayer.CurrentStateFullPathHash,
                                leftLayer.CurrentStateNormalizedTime + (rightLayer.CurrentStateNormalizedTime - leftLayer.CurrentStateNormalizedTime) * ratio,
                                leftLayer.TransitionFullPathHash,
                                leftLayer.TransitionIndex,
                                leftLayer.TransitionStartTime,
                                leftLayer.NextStateNormalizedTime + (rightLayer.NextStateNormalizedTime - leftLayer.NextStateNormalizedTime) * ratio,
                                leftLayer.TransitionNormalizedTime + (rightLayer.TransitionNormalizedTime - leftLayer.TransitionNormalizedTime) * ratio,
                                leftLayer.StateDuration);
                        }
                    }
                }
            }
        }
        
//        private void AssignAnimatorParameters(List<CompressedNetworkAnimatorParameter> leftParams,
//            List<CompressedNetworkAnimatorParameter> rightParams, float ratio)
        private void AssignAnimatorParameters(List<NetworkAnimatorParameter> leftParams, List<NetworkAnimatorParameter> rightParams, float ratio)
        {
//            AssertUtility.Assert(FloatAnimatorParameters.Count == leftParams.Count);
//            int count = FloatAnimatorParameters.Count;
//            for (int i = 0; i < count; i++)
//            {
//                FloatAnimatorParameters[i].Value = (short)(leftParams[i].Value + (rightParams[i].Value - leftParams[i].Value) * ratio);
//            }
//            
//            if (AnimatorParameters != null)
//                ConvertCompressDataToStructureData();

            AssertUtility.Assert(AnimatorParameters.Count == leftParams.Count);
            int count = AnimatorParameters.Count;
            for (int i = 0; i < count; ++i)
            {
                var oldParam = AnimatorParameters[i];
                var newParam = leftParams[i];
                oldParam.ParamType = newParam.ParamType;
                oldParam.IntValue = newParam.IntValue;
                oldParam.FloatValue= newParam.FloatValue + (rightParams[i].FloatValue - newParam.FloatValue) * ratio;
                oldParam.BoolValue = newParam.BoolValue;
                oldParam.NameHash = newParam.NameHash;
            }
        }

        public virtual void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public virtual void CopyFrom(object rightComponent)
        {
            var right = rightComponent as AbstractNetworkAnimator;
            
            InitFields(right);

            // used in clone
            BaseServerTime = right.BaseServerTime;
            NeedChangeServerTime = right.NeedChangeServerTime;
            
            int AnimatorLayersCount = AnimatorLayers.Count;
            for (int i = 0; i < AnimatorLayersCount; i++)
            {
                if(AnimatorLayers[i]==null)AnimatorLayers[i] = new NetworkAnimatorLayer();
                AnimatorLayers[i].RewindTo(right.AnimatorLayers[i]);
            }

            int AnimatorParamsCount = AnimatorParameters.Count;
            for (int i = 0; i < AnimatorParamsCount; ++i)
            {
                if(AnimatorParameters[i]==null)AnimatorParameters[i] = new NetworkAnimatorParameter();
                AnimatorParameters[i].RewindTo(right.AnimatorParameters[i]);
            }
//            int FloatParamCount = FloatAnimatorParameters.Count;
//            for (int i = 0; i < FloatParamCount; ++i)
//            {
//                if (FloatAnimatorParameters[i] == null)
//                    FloatAnimatorParameters[i] = new CompressedNetworkAnimatorParameter();
//
//                FloatAnimatorParameters[i].RewindTo(right.FloatAnimatorParameters[i]);
//            }
        }

        public override string ToString()
        {
            return String.Format("AbstractNetworkAnimator, serverTime:{0}, NeedChangeServerTime:{1}", BaseServerTime, NeedChangeServerTime);
        }

        public string ToStringExt()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("AnimatorLayers:").Append(AnimatorLayers.GetHashCode()).Append(" {\n");
            if (AnimatorLayers == null)
            {
                sb.Append("null\n");
            }
            else
            {
                foreach (var networkAnimatorLayer in AnimatorLayers)
                {
                    sb.Append("<").Append(networkAnimatorLayer).Append(">").Append("\n");
                }
            }
            
            sb.AppendFormat("Server Time:{0}", BaseServerTime);
            return sb.ToString();
        }
    }
}