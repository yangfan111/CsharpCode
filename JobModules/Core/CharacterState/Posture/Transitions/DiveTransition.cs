using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Utils.CharacterState;

namespace Core.CharacterState.Posture.Transitions
{
    class DiveTransition:FsmTransition
    {
        private float _fromValue;
        private float _toValue;
        public DiveTransition(short id, short target, int duration, float fromValue, float toValue, FsmInput enterInput) : base(id, target, duration)
        {
            _fromValue = fromValue;
            _toValue = toValue;
            _simpleTransferCondition =
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, enterInput);
        }

        public override bool Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            var ret =  base.Update(frameInterval, addOutput);
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SwimStateHash,
                AnimatorParametersHash.Instance.SwimStateName,
                NormalizedTime <= 1 ? Mathf.Lerp(_fromValue, _toValue, NormalizedTime) : _toValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
            //Logger.InfoFormat("name:{0}, time:{1} to value:{2}", AnimatorParametersHash.Instance.SwimStateName, NormalizedTime, NormalizedTime <= 1 ? Mathf.Lerp(_fromValue, _toValue, NormalizedTime) : _toValue);
            return ret;
        }
    }
}
