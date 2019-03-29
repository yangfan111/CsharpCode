using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.GameModules.Vehicle;
using Core.EntityComponent;
using Core.GameModule.System;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Vehicle
{
    public class ClientVehicleOwnerIdActiveSystem : ReactiveSystem<VehicleEntity>
    {
        private PlayerContext _playerContext;

        public ClientVehicleOwnerIdActiveSystem(Contexts context) : base(context.vehicle)
        {
            _playerContext = context.player;
        }

        protected override bool Filter(VehicleEntity entity)
        {
            return entity.HasDynamicData();
        }

        protected override ICollector<VehicleEntity> GetTrigger(IContext<VehicleEntity> context)
        {
            return context.CreateCollector(VehicleMatcher.OwnerId.Added(), VehicleMatcher.OwnerId.Removed());
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientVehicleOwnerIdActiveSystem));

        protected override void Execute(List<VehicleEntity> entities)
        {
            try
            {
                if (SharedConfig.DynamicPrediction)
                {
                    var selfEntityKey = _playerContext.flagSelfEntity.entityKey.Value;
                    foreach (var vehicle in entities)
                    {
                        try
                        {
                            SinagleExecute(vehicle, selfEntityKey);
                        }
                        catch (Exception e)
                        {
                            _logger.ErrorFormat("error:{0}", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("error:{0}", e);
            }
        }

        private static void SinagleExecute(VehicleEntity vehicle, EntityKey selfEntityKey)
        {
            if (vehicle.hasOwnerId && vehicle.ownerId.Value.Equals(selfEntityKey))
            {
                VehicleStateUtility.SetVehicleSyncLatest(vehicle, false);
            }
            else if (!vehicle.hasOwnerId)
            {
                VehicleStateUtility.SetVehicleSyncLatest(vehicle, true);
            }
        }
    }
}