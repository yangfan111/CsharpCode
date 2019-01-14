using UnityEngine;

namespace Core.CameraControl
{
    public interface ICameraState
    {
        Vector3 AnchorPosition { get; }
        Quaternion AnchorRotation { get; }
        int ActiveMotorId { get; set; }
        Vector3 CameraPosition { get; }
        Quaternion CameraRotation { get; }
        float Yaw { get; set; }
        float Pitch { get; set; }
        float FreeYaw { get; set; }
        float FreePitch { get; set; }
        float ViewYaw { get; set; }
        float ViewPitch { get; set; }
        float PunchYaw { get; set; }
        float PunchPitch { get; set; }
    }

    public interface ICameraMotorParam
    {
        float Distance { get; set; }
        Vector3 AnchorOffset { get; set; }
        Vector3 ScreenOffset { get; set; }
        float Fov { get; set; }
        float MinYaw { get; set; }
        float MaxYaw { get; set; }
        float MinPitch { get; set; }
        float MaxPitch { get; set; }
    }

    public interface ICameraOutput { 
        bool IsDirty { set; }
        Quaternion CameraRotation { get; set; }
        Vector3 CameraPosition { get; set; }
        Vector3 FocusPosition { get; set; }
        float Fov { get; set; }
    }
}