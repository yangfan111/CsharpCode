using System;
using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class UiSystem : AbstractFrameworkSystem<IUiSystem>
    {
        private IList<IUiSystem> _systems;

        public UiSystem(IGameModule module)
        {
            _systems = module.UiSystems;
            Init();
        }


        public override IList<IUiSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IUiSystem system)
        {
            system.OnUiRender(IntervalTime);
        }
     
        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UI);
                
                base.Execute();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UI);
            }
        }
    }

    public class UiHfrSystem : AbstractFrameworkSystem<IUiHfrSystem>
    {
        private IList<IUiHfrSystem> _systems;
      

        public UiHfrSystem(IGameModule module)
        {
            _systems = module.UiHfrSystems;
            Init();
        }


        public override IList<IUiHfrSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IUiHfrSystem system)
        {
            system.OnUiRender(IntervalTime);
        }
    
        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UI);
                
                base.Execute();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UI);
            }
        }
    }
}