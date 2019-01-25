#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
using System;
using System.Collections.Generic;
using Core.GameModule.Step;
using Core.Utils;
using Entitas;
using Entitas.VisualDebugging.Unity;
using UnityEngine;
using Utils.Singleton;
using SystemInfo = Entitas.VisualDebugging.Unity.SystemInfo;

namespace Core.GameModule.System
{
    public class Feature : Entitas.VisualDebugging.Unity.DebugSystems
    {
        public Feature(string name) : base(name)
        {
        }

        public Feature() : base(true)
        {
            var typeName = Entitas.Utils.TypeSerializationExtension.ToCompilableString(GetType());
            var shortType = Entitas.Utils.TypeSerializationExtension.ShortTypeName(typeName);
            var readableType = Entitas.Utils.StringExtension.ToSpacedCamelCase(shortType);

            initialize(readableType);
        }

        public void AddSystemInfo(SystemInfo info)
        {
            _executeSystemInfos.Add(info);
        }
    }

    public class FakeExecuteSystem : IExecuteSystem
    {
        private string _name;

        public FakeExecuteSystem(string name)
        {
            _name = name;
        }

        public void Execute()
        {
        }

        public string Name
        {
            get { return _name; }
        }
    }
#endif
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
    public abstract class AbstractFrameworkSystem<T> : Feature, IReactiveSystem
    {
        private bool _initialized;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractFrameworkSystem<T>));


        public abstract IList<T> Systems { get; }
        private List<string> _systemNames = new List<string>();
        private List<CustomProfileInfo> _systemProfiles = new List<CustomProfileInfo>();
        public abstract void SingleExecute(T system);
        private float _lastTime = 0;
        public float IntervalTime { get; private set; }


        public override SystemTreeNode GetSystemTree()
        {
            SystemTreeNode node = new SystemTreeNode(systemInfo.systemName.Replace(" ", ""), !paused);
            foreach (var item in _executeSystemInfos)
            {
                node.subNode.Add(new SystemTreeNode(item.systemName.Replace(" ", ""), item.isActive));
            }
            return node;
        }

        public override bool SetStateHandler(string path, bool state,bool changeAllBelow)
        {
            bool result = changeAllBelow;
            if(changeAllBelow)
            {
                systemInfo.isActive = state;
                paused = !state;
            }
            foreach (var Info in _executeSystemInfos)
            {
                if (changeAllBelow || Info.systemName.Equals(path))
                {
                    Info.isActive = state;
                    result = true;
                }
            }
            return result;
        }

        protected void Init()
        {
            _initialized = true;
            foreach (var m in Systems)
            {
                var childSystemInfo = new SystemInfo(new FakeExecuteSystem(m.GetType().Name));
                childSystemInfo.parentSystemInfo = systemInfo;
                _executeSystemInfos.Add(childSystemInfo);
                var fullName = systemInfo.systemName + "_" + childSystemInfo.systemName;
                _systemNames.Add(fullName);
                _systemProfiles.Add(
                    SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(
                        systemInfo.systemName + "_" + childSystemInfo.systemName));
            }
        }

        public override void Execute()
        {
            if (!_initialized)
                throw new Exception("not initialized");
            ExecuteSystems();
        }

        public AbstractFrameworkSystem<T> WithExecFrameStep(EEcecuteStep step)
        {
            _execFrameStep = step;
            return this;
        }

        private int frameCount;
        private EEcecuteStep _execFrameStep = EEcecuteStep.NormalFrameStep;

        protected bool isStepExecute()
        {
            return StepExecuteManager.Instance.IsStepExecute(_execFrameStep);
        }

        protected void ExecuteSystems()
        {
            if (!isStepExecute()) return;

            if (!paused)
            {
                var time = Time.time;
                if (_lastTime == 0) _lastTime = time;
                IntervalTime = time - _lastTime;
                _lastTime = time;

                _executeDuration = 0;
                if (frameCount++ % (int) avgResetInterval == 0)
                {
                    ResetDurations();
                }

                for (int i = 0; i < Systems.Count; i++)
                {
                    var info = _executeSystemInfos[i];
                    var module = Systems[i];
                    if (info.isActive)
                    {
                        try
                        {
                            SingletonManager.Get<DurationHelp>().ProfileStart(_systemProfiles[i]);
                            SingleExecute(module);
                        }
                        catch (Exception e)
                        {
                            _logger.ErrorFormat("error executing {0}: {1}", module.GetType(), e);
                        }
                        finally
                        {
                            var duration=SingletonManager.Get<DurationHelp>().ProfileEnd(_systemProfiles[i]);
                            _executeDuration += duration;
                            info.AddExecutionDuration(duration);
                            
                        }
                    }
                }
            }
        }

        public override int totalExecuteSystemsCount
        {
            get { return Systems.Count; }
        }

        public void Activate()
        {
            
        }

        public void Deactivate()
        {
            
        }

        public void Clear()
        {
           
            for (int i = 0; i < Systems.Count; i++)
            {
                var module = Systems[i];

                if (module is IReactiveSystem)
                {
                    _logger.InfoFormat("Clear System :{0}",module.GetType());
                    ((IReactiveSystem)module).Deactivate();
                    ((IReactiveSystem)module).Clear();
                 
                }
            }
        }
    }
}
#else
using System;
using System.Collections.Generic;
using Core.Utils;
using Entitas;
using Core.GameModule.Step;
using UnityEngine;
using XmlConfig;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public abstract class AbstractFrameworkSystem<T> : Systems, IReactiveSystem
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractFrameworkSystem<T>));

        public abstract IList<T> Systems { get; }
        public abstract void SingleExecute(T system);
        
        private List<string> _subSystemNames = new List<string>();
        private List<bool> _subSystemState = new List<bool>();
        
        private List<CustomProfileInfo> _systemProfiles = new List<CustomProfileInfo>();
        private float _lastTime = 0;
        public float IntervalTime { get; private set; }
        protected void Init()
        {
            foreach (var m in Systems)
            {
                var name = m.GetType().Name;
                _subSystemNames.Add(name);
                _subSystemState.Add(true);
                _systemProfiles.Add(SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(name));
            }
        }

        public override void Execute()
        {
            ExecuteSystems();
        }
        private EEcecuteStep _execFrameStep = EEcecuteStep.NormalFrameStep;
        public AbstractFrameworkSystem<T> WithExecFrameStep(EEcecuteStep step)
        {
            _execFrameStep = step;
            return this;
        }
        protected bool isStepExecute()
        {
            return StepExecuteManager.Instance.IsStepExecute(_execFrameStep);
        }
        protected void ExecuteSystems()
        {
           
            if (!isStepExecute()) return;
             var time = Time.time;
             if (_lastTime == 0) _lastTime = time;
             IntervalTime = time - _lastTime;
             _lastTime = time;

            int len = Systems.Count;

            for (int i = 0; i < len; i++)
            {
                var module = Systems[i];
                try
                {
                    SingletonManager.Get<DurationHelp>().ProfileStart(_systemProfiles[i]);
                    if(_subSystemState[i])
                        SingleExecute(module);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("error executing {0}: {1}", module.GetType(), e);
                }
                finally
                {
                    SingletonManager.Get<DurationHelp>().ProfileEnd(_systemProfiles[i]);
                }
            }
        }
        
        public override SystemTreeNode GetSystemTree()
        {
            _logger.ErrorFormat("Realease: GetTree");
            SystemTreeNode node = new SystemTreeNode(this.GetType().Name.Replace(" ",""), ExecState);
            for(int i=0;i<_subSystemNames.Count;i++)
            {
                node.subNode.Add(new SystemTreeNode(_subSystemNames[i], _subSystemState[i]));
            }
            return node;
        }

        public override bool SetStateHandler(string path, bool state,bool changeAllBelow)
        {
            _logger.ErrorFormat("Realease: {0},{1},{2}", path, state, changeAllBelow);
            bool result = changeAllBelow;
            if(changeAllBelow)
            {
                ExecState = state;
            }
            
            for (int i = 0; i < _subSystemNames.Count; i++)
            {
                if (changeAllBelow || _subSystemNames[i].Replace(" ", "").Equals(path))
                {
                    _subSystemState[i] = state;
                    result = true;
                }
            }
            return result;
        }
        public void Activate()
        {
            
        }

        public void Deactivate()
        {
            
        }

        public void Clear()
        {
            for (int i = 0; i < Systems.Count; i++)
            {
                var module = Systems[i];

                if (module is IReactiveSystem)
                {
                    _logger.InfoFormat("Clear System :{0}",module.GetType());
                    ((IReactiveSystem)module).Deactivate();
                    ((IReactiveSystem)module).Clear();
                }
            }
        }
    }
}
#endif