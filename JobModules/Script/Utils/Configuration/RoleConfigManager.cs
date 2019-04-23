using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using XmlConfig;
using System.Collections;

namespace Utils.Configuration
{
    public class RoleConfigManager : AbstractConfigManager<RoleConfigManager>
    {
       
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RoleConfigManager));
        private Dictionary<int, RoleItem> _roleAssetConfigs = new Dictionary<int, RoleItem>();

        public RoleConfigManager()
        {
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("RoleAsset config xml is empty !");
                return;
            }
            _roleAssetConfigs.Clear();
            var cfg = XmlConfigParser<RoleConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("RoleAsset config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                _roleAssetConfigs.Add(item.Id, item);
            }
        }

        public RoleItem GetRoleItemById(int id)
        {
            RoleItem ret;
            if (!_roleAssetConfigs.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist role item id: {0}", id);
            }
            return ret;
        }

       
    }
}
