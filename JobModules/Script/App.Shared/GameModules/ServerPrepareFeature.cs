using App.Client.ClientGameModules.System;
using App.Shared.GameModules.Common;
using Core.AssetManager;
using Core.EntitasAdpater;
using Core.GameModule.Common;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Room;


namespace App.Shared.GameModules
{
    public sealed class ServerPrepareFeature : Feature
    {
        public ServerPrepareFeature(
            string name,
            IGameModule topLevelGameModule,
            ICommonSessionObjects commonSessionObjects): base(name)
        {
            topLevelGameModule.Init();
            Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            Add(new EntityCreateSystem(topLevelGameModule));
            Add(new UnityAssetManangerSystem(commonSessionObjects));
            Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));
           
        }
    }
}