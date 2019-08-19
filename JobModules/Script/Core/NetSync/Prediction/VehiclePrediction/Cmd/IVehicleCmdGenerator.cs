namespace Core.Prediction.VehiclePrediction.Cmd
{
    public interface IVehicleCmdGenerator
    {
        IVehicleCmd GeneratorVehicleCmd(int currentSimulationTime);
    }
}
