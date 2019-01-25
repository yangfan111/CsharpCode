using System.Linq;
using Entitas;

namespace App.Server
{
    public class ServerVehicleCmdExecuteSystemHandler : AbstractVehicleCmdExecuteSystemHandler
    {
        private IGroup<VehicleEntity> _vehicles;
        public ServerVehicleCmdExecuteSystemHandler(Contexts serverContexts) : base(serverContexts.vehicle)
        {
            _vehicles = serverContexts.vehicle.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.EntityKey));
        }
        protected override VehicleEntity[] GetLatestEntities()
        {
            return _vehicles.GetEntities();
        }
    }
}