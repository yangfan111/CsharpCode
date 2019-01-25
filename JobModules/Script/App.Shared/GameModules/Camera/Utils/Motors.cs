using System;
using System.Collections.Generic;
using Core.CameraControl.NewMotor;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera
{
    public class Motors
    {
        private Dictionary<int, Dictionary<int, ICameraNewMotor>> Dict =
            new Dictionary<int, Dictionary<int, ICameraNewMotor>>();

        public Motors()
        {
            foreach (int i in Enum.GetValues(typeof(SubCameraMotorType)))
            {
                var type = (SubCameraMotorType) i;
                Dict[(int)type] = new Dictionary<int, ICameraNewMotor>();
            }
        }

        public Dictionary<int, ICameraNewMotor> GetDict(SubCameraMotorType type)
        {
            return Dict[(int)type];
        }
        
    }
}