using Core.CameraControl.NewMotor;

namespace Assets.App.Shared.GameModules.Camera
{
    class DummyMotorConfig : IMotorConfig
    {
        public float MinYaw { get; private set; }
        public float MaxYaw { get; private set; }
        public float MinPitch { get; private set; }
        public float MaxPitch { get; private set; }
        public float Smoothing { get; private set; }
        public bool UseYawLimits { get; private set; }
        public bool UsePitchLimits { get; private set; }

        public DummyMotorConfig()
        {
            UsePitchLimits = true;
            UseYawLimits = false;
            MinPitch = -70;
            MaxPitch = 70;
        }
    }
}