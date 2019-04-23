using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public enum ChatChannel
    {
        None,
        Comprehensive,
        Hall,
        Room,
        Team,
        PrivateChat,
        Corps,
        GameTeam,
        Near,
        Camp,
        All
    }

    public enum EUIChatListState
    {
        None,
        Receive,
        Send
    }
    public class ChatConfigManager : AbstractConfigManager<ChatConfigManager>
    {
        private Dictionary<int, Channel> _configs = new Dictionary<int, Channel>();
        private ChatConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<ChatConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach (var item in _config.Items)
            {
                _configs[item.Id] = item;
            }
        }

        public Channel GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in config ", id);
                return null;
            }
            return _configs[id];
        }

        public ChatConfig GetConfig()
        {
            return _config;
        }

        public string GetChannelName(ChatChannel channel)
        {
            var config = GetConfigById((int) channel);
            if (config != null)
            {
                return config.ChannelName;
            }

            return string.Empty;
        }
    }
}
