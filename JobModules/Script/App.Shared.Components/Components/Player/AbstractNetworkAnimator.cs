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
        protected static LoggerAdapter _networkAnimatorLogger = new LoggerAdapter("NetworkAnimatorLogger");
        
        [NetworkProperty] [DontInitilize] public bool NeedChangeServerTime;
        [NetworkProperty] [DontInitilize] public int BaseServerTime;
        [NetworkProperty] [DontInitilize] public int BaseClientTime;
        
        [NetworkProperty]
        public List<NetworkAnimatorLayer> AnimatorLayers;

        [NetworkProperty]
        public List<NetworkAnimatorParameter> AnimatorParameters;
        
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
        
        private bool _needRewind = false;
        [DontInitilize]
        public bool NeedRewind
        {
            get { var ret = _needRewind; _needRewind = false; return ret; }
            private set { _needRewind = value; }
        }
        
        // for debug
        private string _entityName = string.Empty;

        public void SetEntityName(string name)
        {
            _entityName = name;
        }
        
        private int _prevRenderTime = 0;
        
        //
        
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
                foreach (var param in leftComponent.AnimatorParameters)
                {
                    AnimatorParameters.Add(param.Clone());
                }

                _animatorParameterIndex = null;
            }
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public virtual void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            AbstractNetworkAnimator leftComponent = (AbstractNetworkAnimator)left;
            AbstractNetworkAnimator rightComponent = (AbstractNetworkAnimator)right;

            InitFields(leftComponent);

            BaseServerTime = leftComponent.BaseServerTime;
            BaseClientTime = leftComponent.BaseClientTime;
            NeedChangeServerTime = leftComponent.NeedChangeServerTime;
            
            if (AnimatorLayers.Count > 0 && _networkAnimatorLogger.IsDebugEnabled)
            {
                
                _networkAnimatorLogger.DebugFormat("EntityName:{0}, before Interpolate, layer 0:{1}", _entityName,
                     AnimatorLayers[0].ToString());
            }
            
            if (leftComponent.AnimatorLayers.Count > 0 && _networkAnimatorLogger.IsDebugEnabled)
            {
                _networkAnimatorLogger.DebugFormat("EntityName:{2},Interpolate\n left layer 0:{0}\nright layer 0:{1}",
                    leftComponent.AnimatorLayers[0], rightComponent.AnimatorLayers[0], _entityName);
            }
            
            AssignAnimatorLayers(leftComponent.AnimatorLayers,
                                 rightComponent.AnimatorLayers,
                                 leftComponent.BaseServerTime,
                                 interpolationInfo.LeftServerTime,
                                 interpolationInfo.CurrentRenderTime,
                                 rightComponent.BaseServerTime,
                                 interpolationInfo.RightServerTime,
                                 interpolationInfo.Ratio);
            AssignAnimatorParameters(leftComponent.AnimatorParameters);
            
            if (AnimatorLayers.Count > 0 && _networkAnimatorLogger.IsDebugEnabled)
            {
                _networkAnimatorLogger.DebugFormat("EntityName:{1},after Interpolate, layer 0:{0}", AnimatorLayers[0], _entityName);
            }
            
            if (_networkAnimatorLogger.IsDebugEnabled)
            {
                _networkAnimatorLogger.DebugFormat(
                    "EntityName:{9},NetworkAnimatorComponent Interpolate, left.BaseServerTime:{0},right.BaseServerTime:{1},  baseServerTime delta:{2}\n interpolate.LeftServerTime:{3}, interpolate.RightServerTime:{4},interpolate.renderTime:{5},interpolate.Ratio:{6}\n interpolate deltaTime:{7}, render time delta:{8} ",
                    leftComponent.BaseServerTime,
                    rightComponent.BaseServerTime,
                    rightComponent.BaseServerTime - leftComponent.BaseServerTime,
                    interpolationInfo.LeftServerTime,
                    interpolationInfo.RightServerTime,
                    interpolationInfo.CurrentRenderTime,
                    interpolationInfo.Ratio,
                    interpolationInfo.RightServerTime - interpolationInfo.LeftServerTime,
                    interpolationInfo.CurrentRenderTime - _prevRenderTime,
                    _entityName
                );
            }

            _prevRenderTime = interpolationInfo.CurrentRenderTime;
        }
        
        // @see http://192.168.0.6:8090/pages/viewpage.action?pageId=12046047
        private void AssignAnimatorLayers(List<NetworkAnimatorLayer> leftLayers,
                                          List<NetworkAnimatorLayer> rightLayers,
                                          int leftBaseServerTime,
                                          int leftServerTime,
                                          int currentRenderTime,
                                          int rightBaseServerTime,
                                          int rightServerTime,
                                          float ratio)
        {
            _logger.DebugFormat("currentTime:{0}, leftBaseTime:{1}, leftServerTime:{2}, rightBaseTime:{3}, rightServerTime:{4}",
                currentRenderTime,
                leftBaseServerTime,
                leftServerTime,
                rightBaseServerTime,
                rightServerTime);
            AssertUtility.Assert(AnimatorLayers.Count == leftLayers.Count);
            var elapsedFromLeft = (currentRenderTime - leftBaseServerTime) * 0.001f;
            var intervalFromLeft = (currentRenderTime - leftServerTime) * 0.001f;
            var intervalToRight = (rightServerTime - currentRenderTime) * 0.001f;

            for (int i = 0; i < AnimatorLayers.Count; i++)
            {
                var interpolatedLayer = AnimatorLayers[i];
                var leftLayer = leftLayers[i];
                var rightLayer = rightLayers[i];

                // 没有变量/动画发生变化，所以直接Update即可
                if (leftBaseServerTime == rightBaseServerTime)
                {
                    // 回放即是准确定位到某一时刻
                    interpolatedLayer.SetCurrentStateInfo(
                        leftLayer.LayerIndex,
                        leftLayer.Weight,
                        leftLayer.CurrentStateHash,
                        leftLayer.NormalizedTime + elapsedFromLeft / leftLayer.StateDuration,
                        leftLayer.StateDuration,
                        leftLayer.TransitionNormalizedTime,
                        leftLayer.TransitionDuration);
                }
                else // 有变量/动画发生变化
                {
                    // 当leftBaseTime << rightBaseTime
                    // 如果此时有因变量变化导致的Transition，且未结束，因为使用left的变量，所以不会有Transition的效果(不知道ExitTime)
                    if (leftLayer.CurrentStateHash == rightLayer.CurrentStateHash)
                    {
                        var leftNormalizedTime = leftLayer.NormalizedTime +
                                                 (leftServerTime - leftBaseServerTime) * 0.001f / leftLayer.StateDuration;
                        interpolatedLayer.SetCurrentStateInfo(
                            leftLayer.LayerIndex,
                            leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                            leftLayer.CurrentStateHash,
                            leftNormalizedTime + (rightLayer.NormalizedTime - leftNormalizedTime) * ratio,
                            leftLayer.StateDuration,
                            leftLayer.TransitionNormalizedTime,
                            leftLayer.TransitionDuration);
                    }
                    else
                    {
                        // 有过渡
                        if (leftLayer.TransitionNormalizedTime != NetworkAnimatorLayer.NotInTransition)
                        {
                            var transitionRemainTime =
                                leftLayer.TransitionDuration * (1 - leftLayer.TransitionNormalizedTime);
                            if (intervalFromLeft > transitionRemainTime)
                            {
                                interpolatedLayer.SetCurrentStateInfo(leftLayer.LayerIndex,
                                    leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                    rightLayer.CurrentStateHash,
                                    rightLayer.NormalizedTime - intervalToRight / rightLayer.StateDuration,
                                    rightLayer.StateDuration,
                                    rightLayer.TransitionNormalizedTime,
                                    rightLayer.TransitionDuration);
                            }
                            else
                            {
                                 interpolatedLayer.SetCurrentStateInfo(leftLayer.LayerIndex,
                                    leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                    leftLayer.CurrentStateHash,
                                    leftLayer.NormalizedTime + intervalFromLeft / leftLayer.StateDuration,
                                    leftLayer.StateDuration,
                                    leftLayer.TransitionNormalizedTime,
                                    leftLayer.TransitionDuration);                               
                            }
                        }
                        else
                        {
                            var rightCompTime = rightLayer.NormalizedTime * rightLayer.StateDuration;

                            if (rightCompTime >= intervalToRight)
                            {
                                interpolatedLayer.SetCurrentStateInfo(leftLayer.LayerIndex,
                                    leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                    rightLayer.CurrentStateHash,
                                    rightLayer.NormalizedTime - intervalToRight / rightLayer.StateDuration,
                                    rightLayer.StateDuration,
                                    rightLayer.TransitionNormalizedTime,
                                    rightLayer.TransitionDuration);
                            }
                            else
                            {
                                interpolatedLayer.SetCurrentStateInfo(leftLayer.LayerIndex,
                                    leftLayer.Weight + (rightLayer.Weight - leftLayer.Weight) * ratio,
                                    leftLayer.CurrentStateHash,
                                    leftLayer.NormalizedTime + elapsedFromLeft / leftLayer.StateDuration,
                                    leftLayer.StateDuration,
                                    leftLayer.TransitionNormalizedTime,
                                    leftLayer.TransitionDuration);
                            }
                        }
                    }
                }
            }
        }
        
        private void AssignAnimatorParameters(List<NetworkAnimatorParameter> paramsList)
        {
            AssertUtility.Assert(AnimatorParameters.Count == paramsList.Count);
            int count = AnimatorParameters.Count;
            for (int i = 0; i < count; i++)
            {
                var oldParam = AnimatorParameters[i];
                var newParam = paramsList[i];
                //减少函数调用，by zwl
                oldParam.ParamType = newParam.ParamType;
                oldParam.IntValue = newParam.IntValue;
                oldParam.FloatValue = newParam.FloatValue;
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
            BaseClientTime = right.BaseClientTime;
            NeedChangeServerTime = right.NeedChangeServerTime;
            
            int AnimatorLayersCount = AnimatorLayers.Count;
            for (int i = 0; i < AnimatorLayersCount; i++)
            {
                if(AnimatorLayers[i]==null)AnimatorLayers[i] = new NetworkAnimatorLayer();
                AnimatorLayers[i].RewindTo(right.AnimatorLayers[i]);
            }

            int AnimatorParametersCount = AnimatorParameters.Count;
            for (int i = 0; i < AnimatorParametersCount; i++)
            {
                if(AnimatorParameters[i]==null)AnimatorParameters[i] = new NetworkAnimatorParameter();
                AnimatorParameters[i].RewindTo(right.AnimatorParameters[i]);
            }

            NeedRewind = true;
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
                    sb.Append("[").Append(networkAnimatorLayer).Append("]").Append("\n");
                }
            }
            
//            sb.Append("\n}\n");
//            sb.Append("AnimatorParameters:").Append(AnimatorParameters.GetHashCode()).Append(" {\n");
//            foreach (var networkAnimatorParameter in AnimatorParameters)
//            {
//                sb.Append("[").Append(networkAnimatorParameter).Append("]").Append("\n");
//            }
//            sb.Append("\n}\n");
            sb.AppendFormat("Client Time:{0}, Server Time:{1}", BaseClientTime, BaseServerTime);
            return sb.ToString();
        }
    }
}