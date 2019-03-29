namespace Core.CameraControl
{
    public interface ICameraInput
    {
        float DeltaYaw { get; set; }
        float DeltaPitch { get; }
        bool IsCameraFree { get; }
        int FrameInterval { get; }
    }

    public interface IVariableCameraInput : ICameraInput
    {
        new bool IsCameraFree { get; set; }
    }
}