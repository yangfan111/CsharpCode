using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using App.Shared.Components;
using Core.Utils;
using Entitas;
using Entitas.VisualDebugging.Unity;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.SessionState
{

    public class SessionStateMachine:IDisposable
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SessionStateMachine));
        private Dictionary<int, ISessionState> _states = new Dictionary<int, ISessionState>();
        private int _currentState;
        private ISessionStateMachineMonitor _monitor;

        public SessionStateMachine(ISessionStateMachineMonitor monitor = null)
        {
            _monitor = monitor;
        }
        
        public int CurrentState
        {
            get { return _currentState; }
        }

        public void AddState(ISessionState state)
        {
            _states[state.StateId] = state;
            if(_monitor != null)
                _monitor.AddState(state);
        }

        public void Initialize(int initState)
        {
            _currentState = initState;
            if (_monitor != null)
                _monitor.ChangeState(_states[_currentState]);
            _states[_currentState].Initialize();
            
        }

        public void Update()
        {
            try
            {
                GetUpdateSystems().Execute();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Update Error {0}", e);
            }
           
            ChangeState();
        }

        private Systems GetUpdateSystems()
        {
            var stateObj = _states[_currentState];
            Systems rc = stateObj.GetUpdateSystems();
           
            return rc;
        }

        public void OnGUI()
        {
            GetOnGuiSystems().Execute();
        }

        private Systems GetOnGuiSystems()
        {
            var stateObj = _states[_currentState];
            Systems rc = stateObj.GetOnGuiSystems();

            return rc;
        }

        private void ChangeState()
        {
            var stateObj = _states[_currentState];
            if (stateObj.IsFullfilled)
            {
                int nextState = stateObj.NextStateId;
                if (nextState >= 0 && nextState != _currentState)
                {
                    _logger.InfoFormat("change state {0}:=> {1}", _currentState, nextState);
                    _logger.InfoFormat("change state {0}:{1} => {2}:{3}",
                        _currentState, _states[_currentState].GetType(),
                        nextState, _states[nextState].GetType());
                    string condition = string.Format("change state {0}:{1} => {2}:{3}",
                        _currentState, _states[_currentState].GetType().Name,
                        nextState, _states[nextState].GetType().Name);
                    _states[_currentState].CreateExitCondition(condition);
                    if (_monitor != null)
                        _monitor.ChangeState(_states[nextState]);
                    _states[_currentState].Leave();
                    _states[nextState].Initialize();
                    _currentState = nextState;
                    _states[_currentState].FullfillExitCondition(condition);
                    
                }
            }
        }

        public void ShutDown()
        {
            _states[_currentState].Leave();
        }

        public bool ForbidSystems(string path)
        {
            return _states[_currentState].GetUpdateSystems().SetState(path, false);
        }

        public bool PermitSystem(string path)
        {
            return _states[_currentState].GetUpdateSystems().SetState(path, true);
        }

        public SystemTreeNode GetUpdateSystemTree()
        {
            return _states[_currentState].GetUpdateSystems().GetSystemTree();
        }

        public void OnDrawGizmos()
        {
            GetOnDrawGizmosSystems().Execute();
        }

        private Systems GetOnDrawGizmosSystems()
        {
            var stateObj = _states[_currentState];
            var rc = stateObj.GetOnDrawGizmos();

            return rc;
        }

        public void LateUpdate()
        {
            GetLateUpdateSystems().Execute();
            if (_monitor != null)
                _monitor.LateUpdate();
        }

        private Systems GetLateUpdateSystems()
        {
            var stateObj = _states[_currentState];
            var rc = stateObj.GetLateUpdateSystems();

          
            return rc;
        }

        public void Dispose()
        {
            foreach (var sessionState in _states.Values)
            {
                if(sessionState!=null)
                    sessionState.Dispose();
            }
        }
    }
}