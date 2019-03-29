using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;
using UnityEngine;

namespace Core.Configuration
{
    public interface IPlayerSoundConfigManager
    {
        int GetSoundIdByType(EPlayerSoundType soundType);
    }

    public class PlayerSoundConfigManager : AbstractConfigManager<PlayerSoundConfigManager>, IPlayerSoundConfigManager
    {
        Dictionary<EPlayerSoundType, List<int>> _soundTypeDic = new Dictionary<EPlayerSoundType, List<int>>(CommonIntEnumEqualityComparer<EPlayerSoundType>.Instance);
        public override void ParseConfig(string xml)
        {
            var config = XmlConfigParser<PlayerSoundConfig>.Load(xml);
            foreach(var item in config.Items)
            {
                var sounds = item.Id.Split(',');
                foreach(var sound in sounds)
                {
                    int id;
                    if (int.TryParse(sound, out id))
                    {
                        if(!_soundTypeDic.ContainsKey(item.SoundType))
                        {
                            _soundTypeDic[item.SoundType] = new List<int>();
                        }
                        _soundTypeDic[item.SoundType].Add(id);
                    }
                }
            }
        }

        public int GetSoundIdByType(EPlayerSoundType soundType)
        {
            if(!_soundTypeDic.ContainsKey(soundType))
            {
                return UniversalConsts.InvalidIntId;
            }
            if(_soundTypeDic.Count == 1)
            {
                return _soundTypeDic[soundType][0];
            }
            var list = _soundTypeDic[soundType]; 
            return list[Random.Range(0, list.Count)];
        }
    }
}
