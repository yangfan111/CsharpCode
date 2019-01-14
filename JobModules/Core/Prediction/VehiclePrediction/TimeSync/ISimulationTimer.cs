namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public interface ISimulationTimer
    {

        int CurrentTime { get; set; }
        int StepTime { get; }
        void StepCurrentTime();

        float SimulationStepTime { get; }
        int ClampSimulationInterval(int interval); //speed up/down simulation time

    }
}
