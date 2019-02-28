using Core.GameModule.Interface;
using Utils.AssetManager;
using Core.GameModule.Module;
using Entitas;

namespace Core.GameModule.System
{

    public class ModuleInitSystem : IInitializeSystem
    {
        private readonly IGameModule _module;
        private IUnityAssetManager _assetManager;

        public ModuleInitSystem(IGameModule module, 
            IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;
            _module = module;
        }


        public void Initialize()
        {
          foreach (var module in _module.ModuleInitSystems)
            {
                module.OnInitModule(_assetManager);
            }
            
        }
    }

  
}
