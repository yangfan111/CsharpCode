using Core.EntityComponent;
using Core.Prediction;
using Core.Prediction.VehiclePrediction;
using Core.Replicaton;

namespace App.Client.StartUp
{
    public class VehiclePredictionProvider : AbstractPredictionProvider
    {
        private bool _serverAuthorative;
        private VehicleContext _vehicleContext;

        public VehiclePredictionProvider(ISnapshotSelector snapshotSelector, IGameContexts gameContexts,
                                         VehicleContext vehicleContext, bool serverAuthorative) : base(snapshotSelector,
            gameContexts)
        {
            _vehicleContext    = vehicleContext;
            _serverAuthorative = serverAuthorative;
        }

        public override int LastSelfUserCmdSeqId
        {
            get { return LatestSnapshot.VehicleSimulationTime; }
        }


        public override void AfterPredictionInit(bool isRewinded)
        {
            if (isRewinded && _serverAuthorative)
            {
                _vehicleContext.simulationTime.SimulationTime = LatestSnapshot.VehicleSimulationTime;
            }
        }
    }
}