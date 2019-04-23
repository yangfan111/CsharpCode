using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace Utils.Configuration
{
    public interface IEnvironmentTypeConfigManager
    {
        float GetDamageDecayFactorByEnvironmentType(EEnvironmentType environmentType);
        float GetEnergyDecayFactorByEnvironmentType(EEnvironmentType environmentType);
        EEnvironmentType GetEnvironmentTypeByMatName(string matName);
    }

    public class EnvironmentTypeConfigManager : AbstractConfigManager<EnvironmentTypeConfigManager>, IEnvironmentTypeConfigManager
    {
        Dictionary<string, EEnvironmentType> _configs = new Dictionary<string, EEnvironmentType>();
        Dictionary<EEnvironmentType, EnvironmentTypeFactorItem> _factors = new Dictionary<EEnvironmentType, EnvironmentTypeFactorItem>(CommonIntEnumEqualityComparer<EEnvironmentType>.Instance);

        public override void ParseConfig(string xml)
        {
            _configs.Clear();
            _factors.Clear();
            var cfg = XmlConfigParser<EnvironmentTypeConfig>.Load(xml);
            foreach(var item in cfg.Items)
            {
                if(_configs.ContainsKey(item.Name))
                {
                    Logger.Error("deplicated config item");
                }
                _configs[item.Name] = item.Type;
            }
            foreach(var factor in cfg.Factors)
            {
                if(_factors.ContainsKey(factor.Type))
                {
                    Logger.ErrorFormat("deplicated factor item {0}", factor.Type);
                }
                _factors[factor.Type] = factor;
            }
        }

        public float GetDamageDecayFactorByEnvironmentType(EEnvironmentType environmentType)
        {
            if(!_factors.ContainsKey(environmentType))
            {
                return 0;
            }
            return _factors[environmentType].DamageDecay;
        }

        public float GetEnergyDecayFactorByEnvironmentType(EEnvironmentType environmentType)
        {
            if(!_factors.ContainsKey(environmentType))
            {
                return 0;
            }
            return _factors[environmentType].EnergyDecay;
        }

        public EEnvironmentType GetEnvironmentTypeByMatName(string matName)
        {
            var lowerName = matName.ToLower();
            if(lowerName.IndexOf("door") > 0)
            {
                UnityEngine.Debug.LogFormat("hit wood");
                return EEnvironmentType.Wood;
            }
            if (lowerName.IndexOf("win") > 0)
            {
                UnityEngine.Debug.LogFormat("hit window");
                return EEnvironmentType.Glass;
            }
 
            if(_configs.ContainsKey(matName))
            {
                return _configs[matName];
            }
            return EEnvironmentType.Length;
        }
    }
}
