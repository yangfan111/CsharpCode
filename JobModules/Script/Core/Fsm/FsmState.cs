using System;
using System.Collections.Generic;
using Core.Utils;

namespace Core.Fsm
{
    public abstract class FsmState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FsmState));

        public short StateId { get; private set; }
        public int ElapsedTime { get; private set; }

        private List<FsmTransition> _transitions = new List<FsmTransition>();
        private Dictionary<FsmInput, FsmTransition> _quickAccessTransition = new Dictionary<FsmInput, FsmTransition>(FsmInputEqualityComparer.Instance);
        private FsmTransition _activeTransition;

        protected Action<int, IFsmInputCommand, string, Action<FsmOutput>> TransitionCallback;

        protected FsmState(short id)
        {
            StateId = id;
        }

        protected void AddTransition(Func<IFsmInputCommand, Action<FsmOutput>, bool> transfer,
                                     Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> interrupt,
                                     short target,
                                     Action<float, Action<FsmOutput>> update,
                                     int duration,
                                     FsmInput[] keys)
        {
            var transition = new FsmTransition(AvailableTransitionId(), transfer, interrupt, target, update, duration);
            AddTransition(transition, keys);
        }

        protected void AddTransition(Func<IFsmInputCommand, Action<FsmOutput>, FsmStateResponseType> transfer,
                                     Func<IFsmInputCommand, Action<FsmOutput>, FsmTransitionResponseType> interrupt,
                                     short target,
                                     Action<float, Action<FsmOutput>> update,
                                     int duration,
                                     FsmInput[] keys)
        {
            var transition = new FsmTransition(AvailableTransitionId(), transfer, interrupt, target, update, duration);
            AddTransition(transition, keys);
        }

        protected void AddTransition(FsmTransition transition, FsmInput[] keys)
        {
            foreach (var key in keys)
            {
                _quickAccessTransition.Add(key, transition);
            }
            _transitions.Add(transition);
        }

        protected short AvailableTransitionId()
        {
            if (_transitions.Count > short.MaxValue)
            {
                throw new Exception(string.Format("AvailableTransitionId:{0} is exceeted the short max value:{1}", _transitions.Count, short.MaxValue));
            }
            return (short)_transitions.Count;
        }

        public void GetStateSnapshot(FsmSnapshot snapshot)
        {
            snapshot.StateId = StateId;
            snapshot.StateProgress = ElapsedTime;
            snapshot.TransitoinId = _activeTransition != null ? _activeTransition.Id : (short)-1;
            snapshot.TransitionProgress = _activeTransition != null ? _activeTransition.NormalizedTime : -1;
        }

        public void SetStateSnapshot(FsmSnapshot snapshot)
        {
            ElapsedTime = snapshot.StateProgress;
            if (snapshot.TransitoinId != -1)
            {
                _activeTransition = _transitions[snapshot.TransitoinId];
                _activeTransition.NormalizedTime = snapshot.TransitionProgress;
            }
            else
            {
                _activeTransition = null;
            }
        }

        public virtual void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            _activeTransition = null;
            ElapsedTime = 0;
        }

        public virtual void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
        }

        public virtual void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            ElapsedTime += frameInterval;

            if (_activeTransition != null)
            {
                if (_activeTransition.Update(frameInterval, addOutput))
                {
                    TransitionCallback(_activeTransition.To, null, StateId.ToString(), addOutput);
                }
            }
        }

        public virtual FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            FsmStateResponseType response = FsmStateResponseType.Pass;

            if (_activeTransition != null)
            {
                var ret = _activeTransition.InterruptTest(command, addOutput);
                if (ret == FsmTransitionResponseType.ChangeRoad)
                {
                    response = TryTransition(command, addOutput);
                }
                else if (ret == FsmTransitionResponseType.ForceEnd)
                {
                    TransitionCallback(_activeTransition.To, command, StateId.ToString(), addOutput);
                    response = FsmStateResponseType.Reenter;
                }
                else if (ret == FsmTransitionResponseType.ExternalEnd)
                {
                    TransitionCallback(_activeTransition.To, command, StateId.ToString(), addOutput);
                }
            }
            else
            {
                response = TryTransition(command, addOutput);
            }

            return response;
        }

        public FsmTransition ActiveTransition { get { return _activeTransition; } }

        protected bool UpdateForTheFirstTime
        {
            get { return ElapsedTime == 0; }
        }

        protected FsmStateResponseType TryTransition(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            var ret = FsmStateResponseType.Pass;

            if (_quickAccessTransition.ContainsKey(command.Type))
            {
                var transition = _quickAccessTransition[command.Type];
                
                ret = transition.TransferTest(command, addOutput);
                if (ret == FsmStateResponseType.Transfer || ret == FsmStateResponseType.Reenter)
                {
                    if (transition.Duration <= 0)
                    {
                        TransitionCallback(transition.To, command, StateId.ToString(), addOutput);
                    }
                    else
                    {
                        _activeTransition = transition;
                        _activeTransition.Init(command);
                    }

                    if (ret == FsmStateResponseType.Transfer)
                    {
                        ret = FsmStateResponseType.Pass;
                    }
                }               
            }
            
            return ret;
        }

        public void SetTransitionCallback(Action<int, IFsmInputCommand, string, Action<FsmOutput>> callBack)
        {
            TransitionCallback = callBack;
        }

        public void SetTransitionHelper(IFsmTransitionHelper infoProvider)
        {
            foreach (var v in _transitions)
            {
                v.SetTransitionHelper(infoProvider);
            }
        }
    }
}
