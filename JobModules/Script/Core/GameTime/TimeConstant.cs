namespace Core.GameTime
{
    public class TimeConstant
    {
        public const int InterpolateInterval = 50;

        /*允许丢一包*/
        public const int TimeNudge = 50+30;

        public const int ResetTime = 500;
        public static float CompensationDeltaDelta = 0.02f;
    }
}