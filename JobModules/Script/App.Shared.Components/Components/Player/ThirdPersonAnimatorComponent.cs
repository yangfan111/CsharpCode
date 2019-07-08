using Entitas;
using System;
using System.Collections.Generic;
using System.Text;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Core.Utils;

namespace App.Shared.Components.Player
{
    [Player]
    public class ThirdPersonAnimatorComponent : IComponent
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThirdPersonAnimatorComponent));

        private Animator _unityAnimator;
        public Animator UnityAnimator
        {
            get { return _unityAnimator; }
            set
            {
                _unityAnimator = value;
                CacheAnimatorParameters();
                _unityAnimator.enabled = false;
            }
        }

        private int _parameterCount;

        public int ParameterCount
        {
            get { return _parameterCount; }
        }

        private AnimatorControllerParameter[] _paramArray;
        private int[] _paramHashArray;
        
        private void CacheAnimatorParameters()
        {
            _parameterCount = _unityAnimator.parameterCount;
            Logger.InfoFormat("CharacterLog-- Parameters Count: {0}", ParameterCount);
            _paramArray = new AnimatorControllerParameter[ParameterCount];
            _paramHashArray = new int[ParameterCount];
            
            for (int i = 0; i < ParameterCount; i++)
            {
                _paramArray[i] = _unityAnimator.GetParameter(i);
                _paramHashArray[i] = _paramArray[i].nameHash;
            }
        }

        public AnimatorControllerParameter GetParameter(int index)
        {
            if (index < ParameterCount)
                return _paramArray[index];
            return null;
        }

        public int GetParameterHash(int index)
        {
            if (index < ParameterCount)
                return _paramHashArray[index];
            return -1;
        }

        public string DebugJumpInfo()
        {
            string result = string.Empty;
            
            var clip = _unityAnimator.GetCurrentAnimatorClipInfo(0);
            var state = _unityAnimator.GetCurrentAnimatorStateInfo(0);
                    
            result += string.Format("clipName:{0}\n", clip[0].clip.name);
            result += string.Format("state hash:{0}, normalizeTime:{1}, speedMultiplier:{2}\n", state.fullPathHash,
                state.normalizedTime, state.speedMultiplier);
            if (_unityAnimator.IsInTransition(0))
            {
                var tras = _unityAnimator.GetAnimatorTransitionInfo(0);
                result += string.Format("transition hash:{0}, normalizeTime:{1}\n", tras.fullPathHash,
                    tras.normalizedTime);
                var nextState = _unityAnimator.GetNextAnimatorStateInfo(0);
                var nextClip = _unityAnimator.GetNextAnimatorClipInfo(0);
                result += string.Format("next clipName:{0}\n", nextClip[0].clip.name);
                result += string.Format("next state hash:{0}, normalizeTime:{1}, speedMultiplier:{2}\n", nextState.fullPathHash,
                    nextState.normalizedTime, nextState.speedMultiplier);
            }
            result += string.Format("Jump:{0}, JumpLoop:{1}, Speed:{2},XDirection:{3}, ZDirection:{4}, State:{5}\n", _unityAnimator.GetBool("Jump"),
                _unityAnimator.GetBool("JumpLoop"), _unityAnimator.GetFloat("Speed"), _unityAnimator.GetFloat("XDirection"),
                _unityAnimator.GetFloat("ZDirection"), _unityAnimator.GetFloat("State")
                );
            
            return result;
        }
    }
}
