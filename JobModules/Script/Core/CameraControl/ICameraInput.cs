namespace Core.CameraControl
{
    public interface ICameraInput
    {
        float DeltaYaw { get; set; }
        float DeltaPitch { get; set; }
        bool IsCameraFree { get; }
        int FrameInterval { get; set; }
    }

    public interface IVariableCameraInput : ICameraInput
    {
        new bool IsCameraFree { get; set; }
    }
}