using App.Shared.GameModules.Vehicle;
using Core.Prediction;

namespace App.Server
{
    public class ClientVehicleCmdExecuteSystemHandler : AbstractVehicleCmdExecuteSystemHandler
    {
        private Contexts _serverContexts;
        private VehicleEntity[] _one = new VehicleEntity[1];
        private VehicleEntity[] _zero = new VehicleEntity[0];
        public ClientVehicleCmdExecuteSystemHandler(Contexts serverContexts) : base(serverContexts.vehicle)
        {
            _serverContexts = serverContexts;
        }

        protected override VehicleEntity[] GetLatestEntities()
        {
            var vehicleEntity = PlayerVehicleUtility.GetControlledVehicle(_serverContexts.player.flagSelfEntity, _serverContexts.vehicle);
            if (vehicleEntity != null)
            {
                _one[0] = vehicleEntity;
                return _one;

            }
            return _zero;
        }
    }
}