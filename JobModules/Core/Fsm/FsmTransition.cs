using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Animation;
using Core.CharacterState;
using UnityEngine;
using Core.Utils;
using Utils.CharacterState;

namespace Core.Fsm
{
    public interface IFsmTransitionHelper
    {
        float GetDurationCoefficient(string id);
    }

    public class FsmTransition
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FsmTransition));

        public static Action<float, Action<FsmOutput>> GetLerpFunc(FsmOutputType type,
                                                                   float from,
                                                                   float to)
        {
            return (normalizedTime, addOutput) =>
            {
                FsmOutput.Cache.SetValue(type,
                                         normalizedTime <= 1 ? Mathf.Lerp(from, to, normalizedTime) : to);
                addOutput(FsmOutput.Cache);
            };
        }

        public static Action<float, Action<FsmOutput>> GetLerpFunc(int targetHash,
                                                                   string target,
                                                                   float from,
                                                                   float to,
                                                                   CharacterView view)
        {
            return (normalizedTime, addOutput) =>
            {
                FsmOutput.Cache.SetValue(targetHash,
                                         target,
                                         normalizedTime <= 1 ? Mathf.Lerp(from, to, normalizedTime) : to,
                                         view);
                addOutput(FsmOutput.Cache);
            };
        }

        public static bool SimpleCommandHandler(IFsmInputCommand command, FsmInput type)
        {
            var ret = command.IsMatch(type);
            if (ret)
                command.Handled = true;
            return ret;
        }

        public FsmTransition(short id,
                             Func<IFsmInputCommand, Action<FsmOutput>, bool> transfer,
                             Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> interrupt,
                             short target,
                             Action<float, Action<FsmOutput>> update,
                             int duration,
                             string durationCoefficientId = null) : this(id, target, duration)
        {
            _simpleTransferCondition = transfer;
            _interruptCondition = interrupt;
            _update = update;
            _durationCoefficientId = durationCoefficientId;
        }

        public FsmTransition(short id,
                             Func<IFsmInputCommand, Action<FsmOutput>, FsmStateResponseType> transfer,
                             Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> interrupt,
                             short target,
                             Action<float, Action<FsmOutput>> update,
                             int duration,
                             string durationCoefficientId = null) : this(id, target, duration)
        {
            _responsiveTransferCondition = transfer;
            _interruptCondition = interrupt;
            _update = update;
            _durationCoefficientId = durationCoefficientId;
        }

        public FsmTransition(short id, short target, int duration)
        {
            Id = id;
            To = target;
            Duration = _baseDuration = duration;
        }

        public virtual void Init(IFsmInputCommand command)
        {
            NormalizedTime = 0.0f;
            InitValue = command.AdditioanlValue;
        }

        public virtual bool Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            NormalizedTime += frameInterval * _speedRatio / Duration;

            if (_update != null)
            {
                _update(NormalizedTime, addOutput);
            }

            return NormalizedTime >= 1;
        }

        public virtual FsmStateResponseType TransferTest(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            var ret = FsmStateResponseType.Pass;

            if (_simpleTransferCondition != null)
            {
                if (_simpleTransferCondition(command, addOutput))
                {
                    ret = FsmStateResponseType.Transfer;
                }
            }
            if (_responsiveTransferCondition != null)
            {
                ret = _responsiveTransferCondition(command, addOutput);
            }

            if (ret != FsmStateResponseType.Pass)
            {
                if (_durationCoefficientId != null)
                {
                    Duration = (int)(_baseDuration * _infoProvider.GetDurationCoefficient(_durationCoefficientId));
                }
                else
                {
                    Duration = _baseDuration;
                }
            }

            return ret;
        }

        public virtual FsmTransitionResponseType InterruptTest(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (_interruptCondition != null)
            {
                return _interruptCondition(command, addOutput);
            }
            return FsmTransitionResponseType.NoResponse;
        }

        public void SetTransitionHelper(IFsmTransitionHelper infoProvider)
        {
            _infoProvider = infoProvider;
        }

        public float NormalizedTime { get; set; }
        protected float _speedRatio = 1;

        public short Id { get; private set; }
        public short To { get; private set; }
        public int Duration { get; private set; }

        private IFsmTransitionHelper _infoProvider;
        private int _baseDuration;
        private string _durationCoefficientId;

        protected float InitValue;

        protected Func<IFsmInputCommand, Action<FsmOutput>, bool> _simpleTransferCondition;
        protected Func<IFsmInputCommand, Action<FsmOutput>, FsmStateResponseType> _responsiveTransferCondition;
        protected Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> _interruptCondition;
        protected Action<float, Action<FsmOutput>> _update;
    }
}
