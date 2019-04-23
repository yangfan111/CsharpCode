using System.Collections.Generic;
using Utils.AssetManager;
using Core.Utils;
using Shared.Scripts;
using UnityEngine;
using Utils.CharacterState;
using XmlConfig;

namespace Utils.Configuration
{
    public class MeleeAttackCDConfigManager :  AbstractConfigManager<MeleeAttackCDConfigManager>
    {
       
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeAttackCDConfigManager));
        private Dictionary<int, MeleeAttackCDItem> _meleeAttackCDConfigs = new Dictionary<int, MeleeAttackCDItem>();

        public MeleeAttackCDConfigManager()
        {
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("meleeAttack config xml is empty !");
                return;
            }
            _meleeAttackCDConfigs.Clear();
            var cfg = XmlConfigParser<MeleeAttackCDConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("meleeAttack config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                _meleeAttackCDConfigs.Add(item.Id, item);
            }
        }

        public bool IsConfigExist(int id)
        {
            return _meleeAttackCDConfigs.ContainsKey(id);
        }

        public float GetAttackOneCDById(int id)
        {
            var config = _meleeAttackCDConfigs[id];
            if (null == config) return 0;
            return config.AttackOneCD;
        }

        public float GetAttackTwoCDById(int id)
        {
            var config = _meleeAttackCDConfigs[id];
            if (null == config) return 0;
            return config.AttackTwoCD;
        }
    }
}
