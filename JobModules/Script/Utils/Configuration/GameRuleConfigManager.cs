using System.Collections.Generic;
using XmlConfig;

namespace Utils.Configuration
{
    public interface IGameRuleConfigManager
    {
        string GetCoditionDescriptionByIdAndType(int ruleId, int conditionType);
    }
    public class GameRuleConfigManager : AbstractConfigManager<GameRuleConfigManager>, IGameRuleConfigManager
    {
        private GameRuleConfig _config;
        private Dictionary<int, GameRule> _configCache = new Dictionary<int, GameRule>();

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<GameRuleConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach(var item in _config.Items)
            {
                _configCache[item.Id] = item;
            }
        }

        public GameRule GetConfigById(int ruleId)
        {
            if(!_configCache.ContainsKey(ruleId))
            {
                Logger.ErrorFormat("gamerule id {0} doesn't exist in config ", ruleId);
                return null;
            }
            return _configCache[ruleId];
        }
        public string GetCoditionDescriptionByIdAndType(int ruleId, int conditionType)
        {
            var rule = GetConfigById(ruleId);
            var index = rule.ConditionType.FindIndex((it) => it == conditionType);
            if (index == -1 || rule.ConditionDescription.Count <= index) return string.Empty;
            return rule.ConditionDescription[index];
        }
    }
}
