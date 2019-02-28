using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using XmlConfig;

namespace Core.CharacterState.Posture.Transitions
{
    public class PostureTransition : FsmTransition
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PostureTransition));

        private readonly float _fromValueP3;
        private readonly float _toValueP3;
        private readonly float _fromValueP1;
        private readonly float _toValueP1;

        /// <summary>
        /// 用于第一人称的向前偏移
        /// </summary>
        private readonly float _firstPersonForwardOffsetFromValue;

        private readonly float _firstPersonForwardOffsetToValue;

        private readonly CharacterControllerCapsule _fromCharacterControllerConfig;
        private readonly CharacterControllerCapsule _toCharacterControllerConfig;
        private bool _updateP3;

        /// <summary>
        /// 现在站蹲趴不允许被打断了
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transfer"></param>
        /// <param name="interrupt"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        /// <param name="firstPersonFromValue"></param>
        /// <param name="firstPersonToValue"></param>
        /// <param name="thirdPersonFromValue"></param>
        /// <param name="thirdPersonToValue"></param>
        /// <param name="fromCharacterControllerConfig"></param>
        /// <param name="toCharacterControllerConfig"></param>
        /// <param name="updateP3"></param>
        internal PostureTransition(short id,
            Func<IFsmInputCommand, Action<FsmOutput>, bool> transfer,
            Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> interrupt,
            PostureStateId target,
            int duration,
            float firstPersonFromValue,
            float firstPersonToValue,
            float firstPersonForwardOffsetFromValue,
            float firstPersonForwardOffsetToValue,
            float thirdPersonFromValue,
            float thirdPersonToValue,
            CharacterControllerCapsule fromCharacterControllerConfig,
            CharacterControllerCapsule toCharacterControllerConfig,
            bool updateP3 = true) : base(id, (short) target, duration)
        {
            _fromValueP1 = firstPersonFromValue;
            _toValueP1 = firstPersonToValue;
            _firstPersonForwardOffsetFromValue = firstPersonForwardOffsetFromValue;
            _firstPersonForwardOffsetToValue = firstPersonForwardOffsetToValue;
            _fromValueP3 = thirdPersonFromValue;
            _toValueP3 = thirdPersonToValue;
            _fromCharacterControllerConfig = fromCharacterControllerConfig;
            _toCharacterControllerConfig = toCharacterControllerConfig;

            _simpleTransferCondition = transfer;
            _interruptCondition = interrupt;

            _updateP3 = updateP3;
        }

        public override bool Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            var ret = base.Update(frameInterval, addOutput);

            if (_updateP3)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                    AnimatorParametersHash.Instance.PostureName,
                    NormalizedTime <= 1 ? Mathf.Lerp(_fromValueP3, _toValueP3, NormalizedTime) : _toValueP3,
                    CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
            }

            FsmOutput.Cache.SetValue(FsmOutputType.FirstPersonHeight,
                NormalizedTime <= 1 ? Mathf.Lerp(_fromValueP1, _toValueP1, NormalizedTime) : _toValueP1);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(FsmOutputType.FirstPersonForwardOffset,
                NormalizedTime <= 1
                    ? Mathf.Lerp(_firstPersonForwardOffsetFromValue, _firstPersonForwardOffsetToValue, NormalizedTime)
                    : _firstPersonForwardOffsetToValue);
            addOutput(FsmOutput.Cache);

            LerpCharacterControllerCapsule(_fromCharacterControllerConfig, _toCharacterControllerConfig, NormalizedTime,
                addOutput);

            return ret;
        }

        protected static void LerpCharacterControllerCapsule(CharacterControllerCapsule from,
            CharacterControllerCapsule to, float normalizedTime, Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerHeight,
                normalizedTime <= 1 ? Mathf.Lerp(from.Height, to.Height, normalizedTime) : to.Height);
            addOutput(FsmOutput.Cache);
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerRadius,
                normalizedTime <= 1 ? Mathf.Lerp(from.Radius, to.Radius, normalizedTime) : to.Radius);
            addOutput(FsmOutput.Cache);
        }
    }
}