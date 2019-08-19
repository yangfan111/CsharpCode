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
       
        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
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

            if (null == leftComp || null == rightComp) return;

            if (leftComp.FirstPersonAnimName.Equals(rightComp.FirstPersonAnimName))
            {
                FirstPersonAnimName = leftComp.FirstPersonAnimName;
                FirstPersonAnimProgress = leftComp.FirstPersonAnimProgress + 
                                          (rightComp.FirstPersonAnimProgress - leftComp.FirstPersonAnimProgress) * interpolationInfo.Ratio;
            }
            else
            {
                FirstPersonAnimName = rightComp.FirstPersonAnimName;
                FirstPersonAnimProgress = rightComp.FirstPersonAnimProgress;
            }

            if (leftComp.ThirdPersonAnimName.Equals(rightComp.ThirdPersonAnimName))
            {
                ThirdPersonAnimName = leftComp.ThirdPersonAnimName;
                ThirdPersonAnimProgress = leftComp.ThirdPersonAnimProgress + 
                                          (rightComp.ThirdPersonAnimProgress - leftComp.ThirdPersonAnimProgress) * interpolationInfo.Ratio;
            }
            else
            {
                ThirdPersonAnimName = rightComp.ThirdPersonAnimName;
                ThirdPersonAnimProgress = rightComp.ThirdPersonAnimProgress;
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as NetworkWeaponAnimationComponent;
            
            FirstPersonAnimName = right.FirstPersonAnimName;
            FirstPersonAnimProgress = right.FirstPersonAnimProgress;
            ThirdPersonAnimName = right.ThirdPersonAnimName;
            ThirdPersonAnimProgress = right.ThirdPersonAnimProgress;
        }
    }
}
