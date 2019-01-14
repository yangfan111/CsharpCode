namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public interface IClientSimulationTimer : ISimulationTimer
    {

        SimulationTimeSyncInfo SyncInfo { get; set; }

        void Sync();
    }
}
