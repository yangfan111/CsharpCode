using System.Collections.Generic;
using System.Runtime.CompilerServices;
using App.Shared.Components.Player;
using Core.CameraControl.NewMotor;
using UnityEngine;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera
{
    class DummyCameraMotorState : ICameraMotorState
    {
        private Dictionary<int, SubCameraMotorState> _dict =
            new Dictionary<int, SubCameraMotorState>();

        private Motors _motors;

        public DummyCameraMotorState(Motors motors)
        {
            _motors = motors;
            Dict[(int)SubCameraMotorType.Free] =
                new SubCameraMotorState();
            Dict[(int)SubCameraMotorType.Pose] =
                new SubCameraMotorState();
            Dict[(int)SubCameraMotorType.Peek] =
                new SubCameraMotorState();
            Dict[(int)SubCameraMotorType.View] = 
                new SubCameraMotorState();
        }

        public Dictionary<int, SubCameraMotorState> Dict
        {
            get { return _dict; }
            set { _dict = value; }
        }

        public SubCameraMotorState Get(SubCameraMotorType type)
        {
            return Dict[(int) type];
        }

        public float FreeYaw { get; set; }
        public float FreePitch { get; set; }
        public float LastFreeYaw { get; set; }
        public float LastFreePitch { get; set; }
        public float LastPeekPercent { get; set; }

        public SubCameraMotorState GetMainMotor()
        {
            return Get(SubCameraMotorType.Pose);
        }

        public CameraConfigItem GetMainConfig()
        {
            return ((ICameraMainMotor) _motors.GetDict(SubCameraMotorType.Pose)[GetMainMotor().NowMode]).Config;
        }

        public bool IsFree()
        {
            return Get(SubCameraMotorType.Free).NowMode == (short)ECameraFreeMode.On ||  Mathf.Abs(FreeYaw) > 0.1f ||  Mathf.Abs(FreePitch) > 0.1f;
        }

        public bool IsFristPersion()
        {
            return Get(SubCameraMotorType.View).NowMode != (short) ECameraViewMode.ThirdPerson;
        }

        public ECameraViewMode ViewMode
        {
            get { return (ECameraViewMode) Get(SubCameraMotorType.View).NowMode; }
        }
        public ECameraViewMode LastViewMode
        {
            get { return (ECameraViewMode) Get(SubCameraMotorType.View).LastMode; }
        }

        public ECameraFreeMode FreeMode
        {
            get { return (ECameraFreeMode) Get(SubCameraMotorType.Free).NowMode; }
        }
        public ECameraPeekMode PeekMode
        {
            get { return (ECameraPeekMode) Get(SubCameraMotorType.Peek).NowMode; }
        }


        public static void Convert(CameraStateNewComponent inData, DummyCameraMotorState outData)
        {
            outData.FreePitch = inData.FreePitch;
            outData.FreeYaw = inData.FreeYaw;
            outData.LastFreeYaw = inData.LastFreeYaw;
            outData.LastFreePitch = inData.LastFreePitch;
            outData.LastPeekPercent = inData.LastPeekPercent;
            outData.Dict[(int)SubCameraMotorType.Free].Set(inData.FreeNowMode, inData.FreeLastMode, inData.FreeModeTime);
            outData.Dict[(int)SubCameraMotorType.Pose].Set(inData.MainNowMode, inData.MainLastMode, inData.MainModeTime);
            outData.Dict[(int)SubCameraMotorType.Peek].Set(inData.PeekNowMode, inData.PeekLastMode, inData.PeekModeTime);
            outData.Dict[(int)SubCameraMotorType.View].Set(inData.ViewNowMode,inData.ViewLastMode,inData.ViewModeTime);
        
        }

        public static void Convert(DummyCameraMotorState inData, CameraStateNewComponent outData)
        {
            outData.FreePitch = inData.FreePitch;
            outData.FreeYaw = inData.FreeYaw;
            outData.LastFreeYaw = inData.LastFreeYaw;
            outData.LastFreePitch = inData.LastFreePitch;
            outData.LastPeekPercent = inData.LastPeekPercent;
            outData.FreeNowMode = inData.Dict[(int)SubCameraMotorType.Free].NowMode;
            outData.FreeLastMode = inData.Dict[(int)SubCameraMotorType.Free].LastMode;
            outData.FreeModeTime = inData.Dict[(int)SubCameraMotorType.Free].ModeTime;

            outData.MainNowMode = inData.Dict[(int)SubCameraMotorType.Pose].NowMode;
            outData.MainLastMode = inData.Dict[(int)SubCameraMotorType.Pose].LastMode;
            outData.MainModeTime = inData.Dict[(int)SubCameraMotorType.Pose].ModeTime;
       
            outData.ViewNowMode  = inData.Dict[(int)SubCameraMotorType.View].NowMode;
            outData.ViewLastMode = inData.Dict[(int)SubCameraMotorType.View].LastMode;
            outData.ViewModeTime = inData.Dict[(int)SubCameraMotorType.View].ModeTime;
            
            outData.PeekNowMode = inData.Dict[(int)SubCameraMotorType.Peek].NowMode;
            outData.PeekLastMode = inData.Dict[(int)SubCameraMotorType.Peek].LastMode;
            outData.PeekModeTime = inData.Dict[(int)SubCameraMotorType.Peek].ModeTime;
            outData.CanFire = !inData.IsFree() && inData.GetMainConfig().CanFire;

        }
    }
}