using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Animation;
using Core.Utils;
using Utils.Appearance;

namespace Core.Fsm
{
    public abstract class FiniteStateMachine
    {
        static readonly  LoggerAdapter Logger = new LoggerAdapter(typeof(FiniteStateMachine));

        private Dictionary<int, FsmState> _states = new Dictionary<int, FsmState>();
        private FsmState _currentState;
        private int _defaultStateId;
        protected FsmState CurrentState { get { return _currentState; } }

        private string _name;
        private bool _currentStateChanged;

        public String Name
        {
            set { _name = string.Format("{0}:{1}",value,_name); }
        }

        protected FiniteStateMachine(string name)
        {
            _name = name;
        }

        public void GetFsmSnapshot(FsmSnapshot snapshot)
        {
            _currentState.GetStateSnapshot(snapshot);
        }

        public void SetFsmSnapshot(FsmSnapshot snapshot)
        {
            if (_states.ContainsKey(snapshot.StateId))
            {
                if (snapshot.StateId != _currentState.StateId)
                {
                    if (_states.ContainsKey(snapshot.StateId))
                    {
                        _currentState = _states[snapshot.StateId];
                        Logger.DebugFormat("rewind to {0} in {1}", snapshot.StateId, _name);
                    }
                    else
                    {
                        Logger.ErrorFormat("rewind target state not exists: {0} in {1}", snapshot.StateId, _name);
                    }
                }
                _currentState.SetStateSnapshot(snapshot);
            }
            else
            {
                Logger.ErrorFormat("wrong state id {0} in {1}", snapshot.StateId, _name);
            }
        }

        public void AddState(FsmState state, IFsmTransitionHelper infoProvider)
        {
            if (!_states.ContainsKey(state.StateId))
            {
                if (_states.Count == 0)
                {
                    _defaultStateId = state.StateId;
                    _currentState = state;
                }

                // set callback to change current state
                state.SetTransitionCallback(SetCurrentState);
                state.SetTransitionHelper(infoProvider);
                // add state to FSM
                _states.Add(state.StateId, state);
            }
            else
            {
                Logger.ErrorFormat("duplicate fsmState: {0} in {1}", state.StateId, _name);
            }
        }

        // update based
        public void Update(IAdaptiveContainer<IFsmInputCommand> commands, int frameInterval, Action<FsmOutput> addOutput)
        {
            _currentStateChanged = false;
            _currentState.Update(frameInterval, addOutput);
            int lenght = commands.Length;
            for (int i = 0; i < lenght; ++i)
            {
                var command = commands[i];
                if (command.Type != FsmInput.None)
                {
                    // _currentState will not the same if FsmStateResponseType.Reenter is returned
                    while (_currentState.HandleInput(command, addOutput) == FsmStateResponseType.Reenter)
                    {
                    }
                }
            }
        }

        public virtual void Reset(Action<FsmOutput> addOutput)
        {
            SetCurrentState(_defaultStateId, null, "Reset", addOutput);
        }

        protected virtual void SetCurrentState(int id, IFsmInputCommand command, string msg, Action<FsmOutput> addOutput)
        {
            if (_states.ContainsKey(id))
            {
                _currentState.DoBeforeLeaving(addOutput);
                _currentState = _states[id];
                Logger.DebugFormat("{0}: Source: {1} Target: {2}", _name, msg, id);
                _currentState.DoBeforeEntering(command, addOutput);

                _currentStateChanged = true;
            }
            else
            {
                Logger.ErrorFormat("target state not exists: {0} in {1}", id, _name);
            }
        }

        public bool InTransition()
        {
            return _currentState.ActiveTransition != null;
        }

        public float TransitionRemainTime()
        {
            if (InTransition())
            {
                //Logger.InfoFormat("_currentState:{0}, transition id:{1}, transition normalizedTime:{2}, transition duration:{3}", _currentState.StateId, _currentState.ActiveTransition.Id, _currentState.ActiveTransition.NormalizedTime,_currentState.ActiveTransition.Duration );
                return (1 - _currentState.ActiveTransition.NormalizedTime) * _currentState.ActiveTransition.Duration;
            }
            else
            {
                return 0;
            }
        }

        public float TransitionTime()
        {
            if (InTransition())
            {
                return _currentState.ActiveTransition.Duration;
            }
            return 0;
        }
    }
}
