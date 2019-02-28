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
    public sealed class ClientPreparePlayerMainFeature : Feature
    {
        public ClientPreparePlayerMainFeature(string name,
            IGameModule topLevelGameModule,
            ICommonSessionObjects commonSessionObjects) : base(name)
        {
            topLevelGameModule.Init();
            
            Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            Add(new EntityCreateSystem(topLevelGameModule)); 
            
            Add(new UnityAssetManangerSystem(commonSessionObjects));
            Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            Add(new EntityCleanUpSystem(topLevelGameModule));
          
            Add(new CommonDestroySystem(commonSessionObjects));

        }
    } 
}