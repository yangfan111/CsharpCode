using System.Collections.Generic;
using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.Utils;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class ResourceLoadSystem : AbstractFrameworkSystem<IResourceLoadSystem>
    {
        private IList<IResourceLoadSystem> _systems;
        private ILoadRequestManager _loadRequestManager;
        public ResourceLoadSystem(IGameModule module, ILoadRequestManager loadRequestManager)
        {
            _systems = module.ResourceLoadSystems;
            _loadRequestManager = loadRequestManager;
            Init();
        }


        public override IList<IResourceLoadSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IResourceLoadSystem system)
        {
            system.OnLoadResources(_loadRequestManager);
        }

        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.ResourceLoad);
                base.Execute();
            }
            finally {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.ResourceLoad);
            }
           
        }
    }

    
}
