using System;
using System.Collections.Generic;
using System.Linq;
using Core.CameraControl.NewMotor;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace Core.Configuration
{
    public class CameraConfigManager : AbstractConfigManager<CameraConfigManager>
    {
        
        private CameraConfig _config;
        public string XMLContent { get; private set; }

        public CameraConfig Config
        {
            get { return _config; }
        }
        
        
        public CameraConfigItem GetRoleConfig(int roleId = 1)
        {
            var item = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId);
            if (item == null) return null;
            if (item.Unique == false) return _config.GetRoleConfig(0);
            return _config.GetRoleConfig(roleId);
        }
        
        public override void ParseConfig(string xml)
        {
            XMLContent = xml;
            _config = XmlConfigParser<CameraConfig>.Load(xml);
            foreach (var config in _config.ConfigItems)
            {
                HandleConfigItem(config);
            }
            _config.RecordConfigs();
        }
        
        private void HandleConfigItem(CameraConfigItem config)
        {
            config.PoseConfigs = config.PoseConfigs.OrderBy(var => var.CameraType).ToArray();
    
            foreach (var cameraConfigItem in config.PoseConfigs)
            {
                if (cameraConfigItem.Far <100)
                {
                    cameraConfigItem.Far = 8000;
//                    switch (cameraConfigItem.CameraType)
//                    {
//                        case ECameraPoseMode.Stand:
//                        case ECameraPoseMode.DriveCar:
//                        case ECameraPoseMode.Prone:
//                        case ECameraPoseMode.Crouch:
//                        case ECameraPoseMode.Swim:
//                        case ECameraPoseMode.Rescue:
//                        case ECameraPoseMode.Dying:
//                        case ECameraPoseMode.Dead:
//                        case ECameraPoseMode.Climb:
//                        case ECameraPoseMode.AirPlane:
//                        case ECameraPoseMode.Parachuting:
//                        case ECameraPoseMode.ParachutingOpen:
//                        case ECameraPoseMode.Gliding:
//                            cameraConfigItem.Far = 8000;
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
                }
                if (Math.Abs(cameraConfigItem.Near) < 0.00001)
                {
                    cameraConfigItem.Near = 0.02f;
                }
            }

            if (config.ObserveConfig == null)
            {
                config.ObserveConfig= new ObserveCameraConfig();
                config.ObserveConfig.Offset = new Vector3(0,1,0);
                config.ObserveConfig.ObserveDistance = 2;
            }
            
            if (config.ViewConfig == null)
            {
                config.ViewConfig = new ViewCameraConfig()
                {
                    DefaltFovTransitionTime = 100,
                    OnHoldBreathTransitionTime = 100,
                    OffHoldBreathTransitionTime = 100
                };
            }
        }
       

        public int GetTransitionTime(SubCameraMotorType type, SubCameraMotorState state, int roleType = 0)
        {
            if (type == SubCameraMotorType.Pose)
                return PoseTransitionTime((ECameraPoseMode) state.LastMode, (ECameraPoseMode) state.NowMode, roleType);
            if (type == SubCameraMotorType.Free)
                return _config.GetRoleConfig(roleType).FreeConfig.TransitionTime;
            return  _config.GetRoleConfig(roleType).DefaultTransitionTime;
        }
        
        private int PoseTransitionTime(ECameraPoseMode lastMode,ECameraPoseMode curMode, int roleType)
        {
            var config =  _config.GetRoleConfig(roleType).PoseConfigs[(int) curMode];
            if (config.PoseTransitionItems == null) return config.DefaultTime;
            foreach (var item in config.PoseTransitionItems)
            {
                if (item.LastState == lastMode)
                    return item.BaseTime;
            }
            return config.DefaultTime;
        }

    }
}