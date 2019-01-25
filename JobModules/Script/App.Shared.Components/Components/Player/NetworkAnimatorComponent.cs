using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Core.Animation;
using Core.CharacterState;
using Core.Compare;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Core.Utils;
using Core.WeaponAnimation;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
// ReSharper disable InvalidXmlDocComment
namespace App.Shared.Components.Player
{
    /// <summary>
    /// 第三人称动画数据
    /// </summary>
    /// <Write>
    /// <see cref="ServerTimeWriterSystem"/>
    /// <see cref="PlayerStateUpdateSystem"/>
    /// </Write>
    /// <Read>
    /// <see cref="PlayerAppearancePlaybackSystem"/>
    /// </Read>
    /// <author>
    /// yzx
    /// </author>
    [Player]
    public class NetworkAnimatorComponent :AbstractNetworkAnimator, IPlaybackComponent, ICompensationComponent, IUpdateComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkAnimatorComponent));

        public int GetComponentId() { return (int)EComponentIds.AnimatorData; }

     

        public override void CopyFrom(object rightComponent)
        {
            base.CopyFrom(rightComponent);
        }
        
        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }


        /// <summary>
        /// warning， 一直返回true,计算以客户端为主，原因， replayPose是不能完全恢复现场，
        /// 其1:在过渡状态， 有exitTime, normalizeTime会把整数部分截掉，比如5.88->0.88
        /// 其2:在过渡状态，没有exitTime
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool IsApproximatelyEqual(object right)
        {
            bool ret = true;
            var rightObj = right as NetworkAnimatorComponent;
            if (rightObj != null)
            {
//                for (int i = 0; i < AnimatorLayers.Count; i++)
//                {
//                    var equal = AnimatorLayers[i].IsApproximatelyEqual(rightObj.AnimatorLayers[i]);
//                    if (!equal)
//                    {
//                        _logger.Info(AnimatorLayers[i].ToString());
//                        _logger.Info(rightObj.AnimatorLayers[i].ToString());
//                    }
//                    ret = ret && equal;
//                }
                
                for (int i = 0; i < AnimatorParameters.Count; i++)
                {
                    var equal = AnimatorParameters[i].IsApproximatelyEqual(rightObj.AnimatorParameters[i]);
                    if (!equal)
                    {
                        _logger.Info(AnimatorParameters[i].ToString());
                        _logger.Info(rightObj.AnimatorParameters[i].ToString());
                    }
                    ret = ret && equal;
                }
                
                ret = ret && CompareUtility.IsApproximatelyEqual(BaseClientTime, rightObj.BaseClientTime);
            }

            return ret;
        }

        public override string ToString()
        {
            return string.Format("NetworkAnimatorComponent {0}", base.ToString());
        }

    }

    /// <summary>
    /// 第三人称动画状态变化的服务端时间
    /// </summary>
    [Player]
    public class NetworkAnimatiorServerTime : IGameComponent
    {

        [NetworkProperty] public int ServerTime;
        
        public int GetComponentId()
        {
            return (int) EComponentIds.AnimatiorServerTime;
        }
    }
    
    [Player]
    
    public class NetworkWeaponAnimationComponent : IWeaponAnimProgress/*, IUserPredictionComponent*/, IPlaybackComponent, IUpdateComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkAnimatorComponent));
        
        [NetworkProperty]
        public string FirstPersonAnimName { get; set; }
        [NetworkProperty]
        public float FirstPersonAnimProgress { get; set; }
        [NetworkProperty]
        public string ThirdPersonAnimName { get; set; }
        [NetworkProperty]
        public float ThirdPersonAnimProgress { get; set; }

        public int GetComponentId() { { return (int)EComponentIds.WeaponAnimation; } }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var leftComp = left as NetworkWeaponAnimationComponent;
            var rightComp = right as NetworkWeaponAnimationComponent;

            FirstPersonAnimName = leftComp.FirstPersonAnimName;
            FirstPersonAnimProgress = leftComp.FirstPersonAnimProgress + 
                (rightComp.FirstPersonAnimProgress - leftComp.FirstPersonAnimProgress) * interpolationInfo.Ratio;
            
            ThirdPersonAnimName = leftComp.ThirdPersonAnimName;
            ThirdPersonAnimProgress = leftComp.ThirdPersonAnimProgress + 
                (rightComp.ThirdPersonAnimProgress - leftComp.ThirdPersonAnimProgress) * interpolationInfo.Ratio;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as NetworkWeaponAnimationComponent;
            
            FirstPersonAnimName = right.FirstPersonAnimName;
            FirstPersonAnimProgress = right.FirstPersonAnimProgress;
            ThirdPersonAnimName = right.ThirdPersonAnimName;
            ThirdPersonAnimProgress = right.ThirdPersonAnimProgress;
        }

        /// <summary>
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool IsApproximatelyEqual(object right)
        {

            var rightComponent = right as NetworkWeaponAnimationComponent;

            var ret = FirstPersonAnimName.Equals(rightComponent.FirstPersonAnimName) &&
                ThirdPersonAnimName.Equals(rightComponent.ThirdPersonAnimName) &&
                /*CompareUtility.IsApproximatelyEqual(FirstPersonAnimProgress, rightComponent.FirstPersonAnimProgress) &&*/
                CompareUtility.IsApproximatelyEqual(ThirdPersonAnimProgress, rightComponent.ThirdPersonAnimProgress);

            if (!ret)
                _logger.InfoFormat("Local P1:{0}/{1} P3:{2}/{3}\nRemote P1:{4}/{5} P3:{6}/{7}",
                    FirstPersonAnimName, FirstPersonAnimProgress, ThirdPersonAnimName, ThirdPersonAnimProgress,
                    rightComponent.FirstPersonAnimName, rightComponent.FirstPersonAnimProgress,
                    rightComponent.ThirdPersonAnimName, rightComponent.ThirdPersonAnimProgress);

            return ret;
        }
    }
}
