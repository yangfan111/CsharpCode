using Core.EntityComponent;
using Core.Replicaton;

namespace App.Server
{
    public class CompensationManager
    {
        public void CompensationSnapshot(ContextsServerWrapper contextsServerWrapper,SnapshotFactory snapshotFactory)
        {
            int       snapshotSeq           = contextsServerWrapper.GetNextSeq();
            int       vehicleSimulationTime = contextsServerWrapper.SimulationTimer.CurrentTime;
            int       serverTime            = contextsServerWrapper.CurrentTime.CurrentTime;
            ISnapshot compensationSnapshot  = snapshotFactory.GenerateCompensationSnapshot();
            compensationSnapshot.ServerTime            = serverTime;
            compensationSnapshot.SnapshotSeq           = snapshotSeq;
            compensationSnapshot.VehicleSimulationTime = vehicleSimulationTime;
            contextsServerWrapper.SnapshotPool.AddSnapshot(compensationSnapshot);
            compensationSnapshot.ReleaseReference();
        }
    }
}