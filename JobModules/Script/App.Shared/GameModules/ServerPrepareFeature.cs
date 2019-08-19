using App.Client.ClientGameModules.System;
using App.Shared.GameModules.Common;
using Core.EntityComponent;
using Core.GameModule.Common;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Room;
using Utils.AssetManager;


namespace App.Shared.GameModules
{
    public sealed class ServerPrepareFeature : Feature
    {
        public ServerPrepareFeature(string name,
            IGameModule topLevelGameModule, IUnityAssetManager assetManager): base(name)
        {
            topLevelGameModule.Init();
            Add(new ModuleInitSystem(topLevelGameModule, assetManager));
            Add(new EntityCreateSystem(topLevelGameModule));
           
            Add(new ResourceLoadSystem(topLevelGameModule, assetManager));
           
        }
    }
}