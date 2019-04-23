using XmlConfig;

namespace Utils.Configuration
{
    public class RaycastActionConfigManager : AbstractConfigManager<RaycastActionConfigManager>
    {
        private RaycastActionConfig _config;
        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<RaycastActionConfig>.Load(xml);
            if(null == _config)
            {
                Logger.Error("parse raycastAction config failed !!");
            }
        }

        public float Distance
        {
            get
            {
                if(null != _config)
                {
                    return _config.Distance;
                }
                else
                {
                    Logger.Error("config is null !!");
                    return 2;
                }
            }
        }

        public float Interval
        {
            get
            {
                if(null != _config)
                {
                    return _config.Interval;
                }
                else
                {
                    Logger.Error("config is null !!");
                    return 0.3f;
                }
            }
        }
    }
}
