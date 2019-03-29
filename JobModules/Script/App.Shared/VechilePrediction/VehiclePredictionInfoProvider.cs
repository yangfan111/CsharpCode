using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core.Compensation;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Prediction;
using Core.Prediction.VehiclePrediction;
using Core.Replicaton;

namespace App.Client.StartUp
{
    public class VehiclePredictionInfoProvider : AbstractPredictionInfoProvider<IVehiclePredictionComponent>
    {
        private VehicleContext _vehicleContext;
        private bool _serverAuthorative;
        public VehiclePredictionInfoProvider(
            ISnapshotSelectorContainer snapshotSelector,
            IGameContexts gameContexts, VehicleContext vehicleContext,
            bool serverAuthorative)
            : base(snapshotSelector, gameContexts)
        {
            _vehicleContext = vehicleContext;
            _serverAuthorative = serverAuthorative;

        }

        public override int RemoteHistoryId
        {
            get
            {
                return LatestSnapshot.VehicleSimulationTime;
            }
        }


        public override void AfterPredictionInit(bool isRewinded)
        {
            if (isRewinded && _serverAuthorative)
            {
                _vehicleContext.simulationTime.SimulationTime = LatestSnapshot.VehicleSimulationTime;
            }
           
        }

        public override bool IsReady()
        {
            return LatestSnapshot != null;
        }


    }
}