using Core.GameModule.Interface;
using Entitas;
using Utils.AssetManager;

namespace App.Shared.GameModules.Preparation
{
    public class InitialSceneLoadingSystem : IResourceLoadSystem
    {
        public InitialSceneLoadingSystem(Contexts ctx)
        {
//            ctx.session.
        }

        public void OnLoadResources(ILoadRequestManager loadRequestManager)
        {
            throw new System.NotImplementedException();
        }
    }
}