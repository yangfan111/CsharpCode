using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Core.Utils;
using UnityEngine;

namespace Core.CharacterState.Movement
{
    public class DiveTransition:FsmTransition
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DiveTransition));
        private bool _reverse = false;

        public DiveTransition(short id, Func<IFsmInputCommand, Action<FsmOutput>, bool> transfer, Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> interrupt, short target, Action<float, Action<FsmOutput>> update, int duration, string durationCoefficientId = null, bool reverse = false) : base(id, transfer, interrupt, target, update, duration, durationCoefficientId)
        {
            _reverse = reverse;
        }

        public override bool Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            var ret = base.Update(frameInterval, addOutput);
            return ret;
        }

        public override FsmTransitionResponseType InterruptTest(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            var ret = base.InterruptTest(command, addOutput);
            if (ret == FsmTransitionResponseType.ForceEnd)
            {
                command.AdditioanlValue = NormalizedTime;
            }
            return ret;
        }

        public override void Init(IFsmInputCommand command)
        {
            base.Init(command);
            if (InitValue > 0)
            {
                NormalizedTime = 1 - command.AdditioanlValue;
                Logger.InfoFormat("Init normalizeTime:{0}", NormalizedTime);
            }
        }
    }
}
