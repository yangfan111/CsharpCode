using App.Shared.GameModules.Common;
using Core;
using Core.AssetManager;
using Core.EntitasAdpater;
using Core.GameModule.Common;
using Core.GameModule.Module;
using Core.GameModule.System;

using Core.Playback;
using Core.Prediction;
using Core.Prediction.VehiclePrediction.TimeSync;

namespace App.Shared.GameModules
{
    public sealed class ClientPreLoginFeature : Feature
    {
        public ClientPreLoginFeature(
            string name,
            IGameModule topLevelGameModule, 
            ICommonSessionObjects sessionObjects) : base(name)
        {
            topLevelGameModule.Init();
            Add(new ModuleInitSystem(topLevelGameModule, sessionObjects.AssetManager));
            Add(new EntityCreateSystem(topLevelGameModule));
           

            Add(new UnityAssetManangerSystem(sessionObjects));
            Add(new ResourceLoadSystem(topLevelGameModule, sessionObjects.AssetManager));
            
            Add(new RenderSystem(topLevelGameModule));

            Add(new CommonLifeTimeSystem(sessionObjects.GameContexts));
            Add(new EntityCleanUpSystem(topLevelGameModule));
            Add(new CommonDestroySystem(sessionObjects));
        }
    }
}