using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace Utils.Configuration
{
    public class KillFeedBackConfigManager : AbstractConfigManager<KillFeedBackConfigManager>
    {
        
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(KillFeedBackConfigManager));
        private Dictionary<int, KillFeedBack> killfeedBackInfos = new Dictionary<int, KillFeedBack>();

        public KillFeedBackConfigManager()
        {
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("RoleAsset config xml is empty !");
                return;
            }
            killfeedBackInfos.Clear();
            var cfg = XmlConfigParser<KillFeedBackConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("RoleAsset config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                killfeedBackInfos.Add(item.Id, item);
            }
        }

        public string GetIconNameById(int id)
        {
            KillFeedBack ret;
            if (!killfeedBackInfos.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist killFill item id: {0}", id);
            }
            return ret.Icon;
        }

        public string GetEffectNameById(int id)
        {
            KillFeedBack ret;
            if (!killfeedBackInfos.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist killFill item id: {0}", id);
            }
            return ret.Effect;
        }      

        public Dictionary<int, KillFeedBack> GetKillFeedBackInfos()
        {
            return killfeedBackInfos;
        }
    }
}
