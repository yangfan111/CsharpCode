using Utils.AssetManager;
using Core.GameModule.Module;
using Entitas;

namespace Core.GameModule.System
{

    public class ModuleInitSystem : IInitializeSystem
    {
        private readonly IGameModule _module;
        private ILoadRequestManager _loadRequestManager;

        public ModuleInitSystem(IGameModule module, 
            ILoadRequestManager loadRequestManager)
        {
            _loadRequestManager = loadRequestManager;
            _module = module;
        }


        public void Initialize()
        {
          foreach (var module in _module.ModuleInitSystems)
            {
                module.OnInitModule(_loadRequestManager);
            }
            
        }
    }

  
}
