namespace Core.Prediction.VehiclePrediction.TimeSync
{
    public class PhysicsTimeConfig
    {
//#if UNITY_5 || UNITY_2017
//        public static int StepMs = (int)(Time.fixedDeltaTime * 1000);
//#else
        public static int ServerStepMs = 16;
        public static int ClientStepMs = 10;
//#endif
    }
}