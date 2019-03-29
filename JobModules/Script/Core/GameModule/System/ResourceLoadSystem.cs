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
        private IUnityAssetManager _assetManager;
        public ResourceLoadSystem(IGameModule module, IUnityAssetManager assetManager)
        {
            _systems = module.ResourceLoadSystems;
            _assetManager = assetManager;
            Init();
        }


        public override IList<IResourceLoadSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IResourceLoadSystem system)
        {
            system.OnLoadResources(_assetManager);
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
