using System.Collections.Generic;
using Core.Utils;
using XmlConfig;

namespace Utils.Configuration
{
    public class InputBlockConfigManager:  AbstractConfigManager<InputBlockConfigManager>
    {
        
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(InputBlockConfigManager));

        private Dictionary<PostureInConfig, InputBlockItem> _configs  = new Dictionary<PostureInConfig,InputBlockItem>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("InputBlockConfig config xml is empty !");
                return;
            }
            _configs.Clear();
            var cfg = XmlConfigParser<InputBlockConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("InputBlockConfig config is illegal content : {0}", xml);
                return;
            }

            foreach (InputBlockItem inputBlockItem in cfg.Items)
            {
                if (!_configs.ContainsKey(inputBlockItem.PostureType))
                {
                    _configs.Add(inputBlockItem.PostureType, inputBlockItem);
                }
                else
                {
                    Logger.ErrorFormat("{0} is already in config!!!", inputBlockItem.PostureType);
                }
            }
        }

        public InputBlockItem GetConfig(PostureInConfig postureInConfig)
        {
            InputBlockItem ret = null;
            _configs.TryGetValue(postureInConfig, out ret);
            return ret;
        }
    }
}