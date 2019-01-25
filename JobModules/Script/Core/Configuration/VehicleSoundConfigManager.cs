using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Configuration;
using XmlConfig;
using Random = System.Random;

namespace Core.Configuration
{
    public enum EVehicleSoundId
    {
        WaitingForPlay = -1,
        Invalid = 0,
    }

    public class VehicleSoundConfigManager : AbstractConfigManager<VehicleSoundConfigManager>
    {
        private Dictionary<int, List<VehicleSoundConfigItem>> _channelSoundDic =
            new Dictionary<int, List<VehicleSoundConfigItem>>();

        private Dictionary<int, bool> _isSoundMusicDic = new Dictionary<int, bool>();

        public int GetRandomSoundId(int channel)
        {
            int id = (int)EVehicleSoundId.Invalid;
            List<VehicleSoundConfigItem> soundList;
            if (_channelSoundDic.TryGetValue(channel, out soundList))
            {
                id = GetrandomSoundId(soundList);
                //Debug.LogFormat("Channel {0}, id {1}", channel, id);
            }

            return id;
        }

        public EVehicleSoundType GetSoundType(int soundId)
        {
            bool isMusic;
            if (_isSoundMusicDic.TryGetValue(soundId, out isMusic))
            {
                if (isMusic)
                {
                    return EVehicleSoundType.Music;
                }

                return EVehicleSoundType.Record;
            }

            return EVehicleSoundType.Unkown;
        }

        public bool IsLoopSound(int soundId)
        {
            bool isLoop;
            if (_isSoundMusicDic.TryGetValue(soundId, out isLoop))
            {
                return isLoop;
            }

            return false;
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.ErrorFormat("vehicle asset config is Empty");
                return;
            }

            var cfg = XmlConfigParser<VehicleSoundConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("vehicle asset config is illegal content:{0}", xml);
                return;
            }


            foreach (var item in cfg.Items)
            {
                var channel = item.Channel;
                if (!_channelSoundDic.ContainsKey(channel))
                {
                    _channelSoundDic[channel] = new List<VehicleSoundConfigItem>();
                }

                var soudList = _channelSoundDic[channel];
                soudList.Add(item);

                if (item.Type == EVehicleSoundType.Music)
                {
                    _isSoundMusicDic[item.Id] = true;
                }
                else if (item.Type == EVehicleSoundType.Record)
                {
                    _isSoundMusicDic[item.Id] = false;
                }
            }

            foreach (var key in _channelSoundDic.Keys)
            {
                var soundList = _channelSoundDic[key];
                NormalizeWeight(soundList);
            }
        }

        private void NormalizeWeight(List<VehicleSoundConfigItem> soundList)
        {
            var totalWeight = 0.0f;
            foreach (var item in soundList)
            {
                if (item.Weight <= 0)
                {
                    Logger.InfoFormat("vehicle sound's weight for item {0} is non-positive {1}", item.Id, item.Weight);
                    continue;
                }
                totalWeight += item.Weight;
            }

            if (totalWeight <= 0)
            {
                return;
            }

            foreach (var item in soundList)
            {
                item.Weight /= totalWeight;
            }
        }

        private int GetrandomSoundId(List<VehicleSoundConfigItem> soundList)
        {
            int id = (int)EVehicleSoundId.Invalid;
            var weight = (new Random(Guid.NewGuid().GetHashCode())).NextDouble();
            //Debug.LogFormat("weight {0}", weight);
            int count = soundList.Count;
            for (int i = 0; i < count; ++i)
            {
                var item = soundList[i];
                if (weight <= item.Weight)
                {
                    id = item.Id;
                    break;
                }

                if (item.Weight >= 0)
                    weight -= item.Weight;
            }

            return id;
        }

    }
}
