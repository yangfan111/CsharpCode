using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Entitas;
using Entitas.VisualDebugging.Unity;

namespace Core.SessionState
{
    public abstract class AbstractSessionState : ISessionState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractSessionState));
        private HashSet<string> _conditions = new HashSet<string>();
        private Systems _updateSystems;
        private Systems _onDrawGizmosSystems;
        private Systems _lateUpdateSystems;
        private Systems _onGUISystems;
        private bool _isUpdateSystemsInistalized;
        private bool _isLateUpdateSystemsInistalized;
        private bool _isOnDrawGizmosSystemsInistalized;
        private bool _isOnGuiSystemsIntialized;

        public virtual int LoadingProgressNum
        {
            get { return 0; }
        }

        public virtual string LoadingTip
        {
            get { return ""; }
        }

        public HashSet<string> Conditions
        {
            get { return _conditions; }
        }
        public  int StateId { get; private set; }
        public  int NextStateId { get; private set; }

        private IContexts _contexts;
        private bool _initilized;
        protected AbstractSessionState(IContexts contexts, int stateId, int nextStateId)
        {
            _contexts = contexts;
            StateId = stateId;
            NextStateId = nextStateId;
        }

        public void Initialize()
        {
            if (_initilized)
                return;
            _initilized = true;
            _updateSystems = CreateUpdateSystems(_contexts);
            _onDrawGizmosSystems = CreateOnDrawGizmos(_contexts);
            _lateUpdateSystems = CreateLateUpdateSystems(_contexts);
            _onGUISystems = CreateOnGuiSystems(_contexts);
            if (_onDrawGizmosSystems == null)
                _onDrawGizmosSystems = new Systems();
            if (_lateUpdateSystems == null)
                _lateUpdateSystems = new Systems();
            if (_onGUISystems == null)
                _onGUISystems = new Systems(); 
        }

        public virtual void Leave()
        {
            GC.Collect();
            var root = GetUpdateSystems();
            root.ClearReactiveSystems();
            _logger.InfoFormat("{0}  Leave",GetType());
            if (root is DebugSystems)
            {
                var go = ((DebugSystems) root).gameObject;
                if (go != null)
                {
                    _logger.InfoFormat("{0}  DestroyGameObject", go.name);
                    go.DestroyGameObject();
                }
                
            }
        }

        public abstract Systems CreateUpdateSystems(IContexts contexts);

        public virtual Systems CreateLateUpdateSystems(IContexts contexts)
        {
            return null;
        }

        public virtual Systems CreateOnDrawGizmos(IContexts contexts)
        {
            return null;
        }

        public virtual Systems CreateOnGuiSystems(IContexts contexts)
        {
            return null;
        }



        public Systems GetUpdateSystems()
        {
            if (!_isUpdateSystemsInistalized)
            {
				_isUpdateSystemsInistalized = true;
                _updateSystems.Initialize();
            }
            return _updateSystems;
        }

        public Systems GetLateUpdateSystems()
        {
            if (!_isLateUpdateSystemsInistalized)
            {
                _isLateUpdateSystemsInistalized = true;
                _lateUpdateSystems.Initialize();
            }
            return _lateUpdateSystems;
        }

        public Systems GetOnDrawGizmos()
        {
            if (!_isOnDrawGizmosSystemsInistalized)
            {
                _isOnDrawGizmosSystemsInistalized = true;
                _onDrawGizmosSystems.Initialize();
            }
            return _onDrawGizmosSystems;
        }

        public void CreateExitCondition(string conditionId)
        {
            _logger.InfoFormat("{0} create exit condition {1}", GetType(), conditionId);
            _conditions.Add(conditionId);
        }

        public void FullfillExitCondition(string conditionId)
        {
            if (_conditions.Contains(conditionId))
            {
                _logger.InfoFormat("{0} fullfill exit condition {1}", GetType(), conditionId);
                _conditions.Remove(conditionId);
            }
        }

        public void CreateExitCondition(Type conditionId)
        {
            CreateExitCondition(conditionId.FullName);
        }

        public void FullfillExitCondition(Type conditionId)
        {
            FullfillExitCondition(conditionId.FullName);
        }

	    private DateTime _lastPrintTime = DateTime.Now;
        private int _logInterval = 5000;
        public bool IsFullfilled
        {
            get
            {
                bool rc = _conditions.Count == 0;
                if (!rc && _logger.IsWarnEnabled)
                {
	                if ((DateTime.Now - _lastPrintTime).TotalMilliseconds > _logInterval)
	                {
		                _lastPrintTime = DateTime.Now;
						_logger.ErrorFormat("remain contains {0}", string.Join(",", _conditions.ToArray()));
	                    if (_logInterval < 60000)
	                        _logInterval *= 2;

	                }
                }
                return rc;
            }
        }

        public Systems GetOnGuiSystems()
        {
            if (!_isOnGuiSystemsIntialized)
            {
                _isOnGuiSystemsIntialized = true;
                _onGUISystems.Initialize();
            }
            return _onGUISystems;
        }


        public virtual void Dispose()
        {
          
        }
    }
}