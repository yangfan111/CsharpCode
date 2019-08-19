using System.Collections.Generic;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Configuration
{
    public class ExpData
    {
        public int totalExp;    //总经验
        public int level;       //等级
        public int showExp;     //当前等级经验
        public int upgradeExp;  //当前等级升级所需经验
        public float ratio;     //经验比例
        public bool isMax;      //是否最大等级

        public override string ToString()
        {
            return "total: " + totalExp + " level: " + level + " showExp: " + showExp + " upgradeExp: " + upgradeExp + " ratio: " + ratio + " isMax: " + isMax;
        }
    }
    public class WeaponUpdateConfigManager : AbstractConfigManager<WeaponUpdateConfigManager>
    {
        private Dictionary<int, WeaponUpdate> _configs = new Dictionary<int, WeaponUpdate>();
        private WeaponUpdateConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<WeaponUpdateConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach (var item in _config.Items)
            {
                _configs[item.Tid] = item;
            }
        }

        private WeaponUpdate GetConfigById(int id)
        {
            if (!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in  config ", id);
                return null;
            }
            return _configs[id];
        }

        public int GetMaxLv(int id)
        {
            var config = GetConfigById(id);
            if (config == null) return 0;
            return config.MaxLv;
        }

        public int GetMaxLvAndLevel(int id,int exp,out int level,out float ratio)
        {
            GetExpCfg();
            var maxLv = GetMaxLv(id);
            var data = GetWeaponExpData(exp, maxLv);
            level = data.level;
            ratio = data.ratio;
            return maxLv;
        }
        List<int> UpgradeTotlaExp;  //每级总共的经验//

        public static float decomposeRate = 0;
        void GetExpCfg()
        {
            if (UpgradeTotlaExp == null)
            {
                UpgradeTotlaExp = SingletonManager.Get<GlobalDataConfigManager>().GetIntListValue(11);
            }

            if (decomposeRate <= 0)
            {
                decomposeRate = SingletonManager.Get<GlobalDataConfigManager>().GetFloatValue(31);
            }
        }

        public ExpData GetWeaponExpData(int totalExp, int maxLevel)
        {
            maxLevel = maxLevel <= UpgradeTotlaExp.Count ? maxLevel : UpgradeTotlaExp.Count;

            ExpData expData = new ExpData();
            expData.totalExp = totalExp;

            int level = 0;
            if (totalExp >= UpgradeTotlaExp[0])
            {
                for (int i = 0; i < UpgradeTotlaExp.Count; i++)
                {
                    if (i + 1 < UpgradeTotlaExp.Count)
                    {
                        if (totalExp >= UpgradeTotlaExp[i] && totalExp < UpgradeTotlaExp[i + 1])
                        {
                            level = i + 1;
                            break;
                        }
                    }
                    else
                    {
                        level = maxLevel;
                    }
                }
            }
            if (level > maxLevel)
            {
                level = maxLevel;
            }
            expData.level = level;
            expData.isMax = level >= maxLevel ? true : false;
            if (!expData.isMax)
            {
                if (level == 0)
                {
                    expData.upgradeExp = UpgradeTotlaExp[0];
                    expData.showExp = totalExp;
                }
                else
                {
                    expData.upgradeExp = UpgradeTotlaExp[level] - UpgradeTotlaExp[level - 1];
                    expData.showExp = totalExp - UpgradeTotlaExp[level - 1];
                }
                expData.ratio = (float)expData.showExp / expData.upgradeExp;
            }
            else
            {
                expData.upgradeExp = 0;
                expData.showExp = 0;
                expData.ratio = 0;
            }
            return expData;
        }


    }
}
