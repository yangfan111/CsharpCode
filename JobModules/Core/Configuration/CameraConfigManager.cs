using System;
using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;

namespace Core.Configuration
{
    public class CameraConfigManager : AbstractConfigManager<CameraConfigManager>
    {
        private Dictionary<ECameraConfigType, CameraConfigItem> _cameraConfigItems =
            new Dictionary<ECameraConfigType, CameraConfigItem>(CommonEnumEqualityComparer<ECameraConfigType>.Instance);

        private CameraConfig _config;
        public string XMLContent { get; private set; }

        public CameraConfig Config
        {
            get { return _config; }
        }

        public int DeadTranstiionTime { get; private set; }
        public int DefaultTranstionTime { get; private set; }

        public override void ParseConfig(string xml)
        {
            XMLContent = xml;
            _config = XmlConfigParser<CameraConfig>.Load(xml);
            foreach (var cameraConfigItem in _config.PoseConfigs)
            {
                _cameraConfigItems[cameraConfigItem.CameraType] = cameraConfigItem;
                if (cameraConfigItem.Far <100)
                {
                    switch (cameraConfigItem.CameraType)
                    {
                        case ECameraConfigType.ThirdPerson:
                        case ECameraConfigType.FirstPerson:
                        case ECameraConfigType.DriveCar:
                        case ECameraConfigType.Prone:
                        case ECameraConfigType.Crouch:
                        case ECameraConfigType.Swim:
                        case ECameraConfigType.Rescue:
                        case ECameraConfigType.Dying:
                        case ECameraConfigType.Dead:
                            cameraConfigItem.Far = 1500;
                            break;
                        case ECameraConfigType.AirPlane:
                        case ECameraConfigType.Parachuting:
                        case ECameraConfigType.ParachutingOpen:
                        case ECameraConfigType.Gliding:
                            cameraConfigItem.Far = 8000;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                if (Math.Abs(cameraConfigItem.Near) < 0.00001)
                {
                    cameraConfigItem.Near = 0.03f;
                }
            }

            DefaultTranstionTime = _config.DefaultTransitionTime;
        }

        public CameraConfigItem GetConfigByType(ECameraConfigType type)
        {
            if (_cameraConfigItems.ContainsKey(type))
            {
                return _cameraConfigItems[type];
            }

            Logger.WarnFormat("Config does not exsit for {0}", type);
            return null;
        }

        public CameraConfigItem[] GetConfigItems()
        {
            if (null != _config)
            {
                return _config.PoseConfigs;
            }
            else
            {
                Logger.Error("camera config is null !");
                return null;
            }
        }
    }
}