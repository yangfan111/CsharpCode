using System;
using System.Collections.Generic;
using Utils.Configuration;
using XmlConfig;

namespace Core.Configuration
{
    public class LadderRankConfigManager : AbstractConfigManager<LadderRankConfigManager>
    {
        private Dictionary<int, LadderRankConfigItem> _dict = new Dictionary<int, LadderRankConfigItem>();
        private int _maxId;

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("ladder rank config xml is empty !");
                return;
            }
            _dict.Clear();
            var cfg = XmlConfigParser<LadderRankConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("ladder rank config is illegal content : {0}", xml);
                return;
            }
            foreach (var item in cfg.Items)
            {
                _dict[item.Id] = item;
                _maxId = Math.Max(_maxId, item.Id);
            }
        }

        public LadderRankConfigItem GetLadderRank(int id)
        {
            if (!_dict.ContainsKey(id))
            {
                return null;
            }
            return _dict[id];
        }

        public int GetK(int rankScore)
        {
            LadderRankConfigItem item;
            for (int id = _maxId; id > 0; id--)
            {
                item = GetLadderRank(id);
                if (null != item && rankScore >= item.Rank)
                {
                    return item.CoefficientK;
                }
            }
            return 0;
        }
    }
}
