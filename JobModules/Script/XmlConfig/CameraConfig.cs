using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace XmlConfig
{
    public enum SubCameraMotorType
    {
        Pose,
        Free,
        Peek,
        View,
        End
    }

    public enum ECameraPoseMode
    {
        Stand,
        DriveCar,
        Prone,
        Crouch,
        Swim,
        Rescue,
        Dying,
        AirPlane,
        Dead,
        Parachuting,
        ParachutingOpen,
        Gliding,
        Climb,
        BuriedBomb,
        DismantleBomb,
        End
    }
    
    public enum ECameraFreeMode
    {
        Off,
        On,
        End
    }

    public enum ECameraViewMode
    {
        ThirdPerson,
        FirstPerson,
        GunSight,
        End
    }

    public enum ECameraPeekMode
    {
        Off,
        Right,
        Left,
        End
    }


    public class FreeCameraConfig
    {
        public int TransitionTime;
    }

    [Serializable]
    public class DeadCameraConfig
    {
        public Vector3 Roatation;
    }

    [Serializable]
    public class PeekCameraConfig
    {
        public Vector3 ThirdOffset;
        public Vector3 GunSightOffset;
        public Vector3 FirstOffset;
        public int TransitionTime;
    }

    [Serializable]
    public class ViewCameraConfig
    {
        public int DefaltFovTransitionTime;
        public int OnHoldBreathTransitionTime;
        public int OffHoldBreathTransitionTime;
    }

    [Serializable]
    public class ObserveCameraConfig
    {
        public Vector3 Offset;
        public float ObserveDistance;
    }

    [Serializable]
    public class CameraConfigItem
    {
        public int RoleType;
        
        public PoseCameraConfig[] PoseConfigs;
        public PeekCameraConfig PeekConfig;
        public FreeCameraConfig FreeConfig;
        public DeadCameraConfig DeadConfig;
        public ViewCameraConfig ViewConfig;
        public ObserveCameraConfig ObserveConfig;
        public int DefaultTransitionTime;
        public int PostTransitionTime;
        
        public PoseCameraConfig GetCameraConfigItem(ECameraPoseMode Type)
        {
            foreach (var cameraConfigItem in PoseConfigs)
            {
                if (cameraConfigItem.CameraType.Equals(Type))
                {
                    return cameraConfigItem;
                }
            }
            return null;
        }
    }
    
    [Serializable]
    public class CameraConfig
    {
        public CameraConfigItem[] ConfigItems;

        private Dictionary<int, CameraConfigItem> _configDict = new Dictionary<int, CameraConfigItem>();

        private int previousRoleType = -1;

        private CameraConfigItem cameraConfigItem;

        public CameraConfigItem GetRoleConfig(int roleType = 0)
        {
            if (roleType == previousRoleType)
            {
                return cameraConfigItem;
            }
            previousRoleType = roleType;

            cameraConfigItem = _configDict[roleType];
            return cameraConfigItem;
        }

        public void RecordConfigs()
        {
            previousRoleType = -1;
            foreach (var item in ConfigItems)
            {
                _configDict.Add(item.RoleType, item);
            }
        }
    }

    [Serializable]
    public class ViewLimit
    {
        public bool Flag;
        public float Min;
        public float Max;

        public ViewLimit Clone()
        {
            var r = new ViewLimit();
            r.Flag = Flag;
            r.Min = Min;
            r.Max = Max;
            return r;
        }
    }

    public struct PoseTransitionConfig
    {
        public ECameraPoseMode LastState;
        public int BaseTime;
    }
    
    [Serializable]
    public class PoseCameraConfig
    {
        public int Id;
        public int Order;
        public bool CanFire;
        public bool CanSwitchView;

        public ECameraPoseMode CameraType;
        public Vector3 AnchorOffset;
        public Vector3 ScreenOffset;
        public float Distance;
        public ViewLimit YawLimit;
        public ViewLimit PitchLimit;

        public float Smoothing;
        public int Fov;
        public float Far;
        public float Near;
        public bool ForbidDetect;

        public int DefaultTime;
        public PoseTransitionConfig[] PoseTransitionItems;
    }

}
