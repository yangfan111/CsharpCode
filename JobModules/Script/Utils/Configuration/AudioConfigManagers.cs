using Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Configuration
{
    public class AudioEventManager : AbstractConfigManager<AudioEventManager>
    {
        public readonly Dictionary<int, AudioEventItem> itemsMap = new Dictionary<int, AudioEventItem>();
        private         AudioEventConfig                data;

        public AudioEventItem FindById(int id)
        {
            if (id < 1)
            {
                DebugUtil.LogWarning("audio event {0} not find", DebugUtil.DebugColor.Default, id);
                return null;
            }

            return itemsMap[id];
        }

        public ICollection<AudioEventItem> GetMapCollection()
        {
            return itemsMap.Values;
        }
        public override void ParseConfig(string xml)
        {
            AudioEventConfig data = XmlConfigParser<AudioEventConfig>.Load(xml);
            foreach (var item in data.Items)
            {
                itemsMap[item.Id] = item;
#if UNITY_EDITOR
                nameItemsMap[item.Event] = item;
#endif
            }

        }
#if UNITY_EDITOR
        public readonly Dictionary<string, AudioEventItem> nameItemsMap = new Dictionary<string, AudioEventItem>();

#endif
    }

    public class AudioGroupManager : AbstractConfigManager<AudioGroupManager>
    {
        public readonly Dictionary<int, AudioGroupItem> itemsMap = new Dictionary<int, AudioGroupItem>();
        private         AudioGroupConfig                data;

        public override void ParseConfig(string xml)
        {
            var data = XmlConfigParser<AudioGroupConfig>.Load(xml);
            foreach (var item in data.Items)
            {
                itemsMap[item.Id] = item;
            }
        }

        public AudioGroupItem FindById(int id)
        {
            AudioGroupItem item;
            itemsMap.TryGetValue(id, out item);
            return item;
        }
    }

    public class AudioParamManager : AbstractConfigManager<AudioParamManager>
    {
        public readonly Dictionary<int, AudioParamItem> itemsMap = new Dictionary<int, AudioParamItem>();
        private         AudioParamConfig                data;

        public override void ParseConfig(string xml)
        {
            var data = XmlConfigParser<AudioParamConfig>.Load(xml);
            foreach (var item in data.Items)
            {
                itemsMap[item.Id] = item;
            }
        }

        public AudioParamItem FindById(int id)
        {
            return itemsMap[id];
        }
    }

    public class AudioWeaponManager : AbstractConfigManager<AudioWeaponManager>
    {
        public readonly Dictionary<int, AudioWeaponItem> itemsMap = new Dictionary<int, AudioWeaponItem>();
        private         AudioWeaponConfig                data;

        public override void ParseConfig(string xml)
        {
            var data = XmlConfigParser<AudioWeaponConfig>.Load(xml);
            foreach (var item in data.Items)
            {
                itemsMap[item.Id] = item;
            }
        }

        public AudioWeaponItem FindById(int id)
        {
            return itemsMap[id];
        }

        public LoggerAdapter logger = new LoggerAdapter(typeof(AudioWeaponManager));

        public AudioEventItem FindById(int id, Func<AudioWeaponItem, int> filter)
        {
            if (id < 1)
            {
                //logg("audio weapon {0} not find", DebugUtil.DebugColor.Default,id);
                return null;
            }

            if (!itemsMap.ContainsKey(id))
                return null;
            var item = itemsMap[id];
            if (filter(item) == 0)
            {
                DebugUtil.MyLog("Audio Fire event invalid", DebugUtil.DebugColor.Blue);
                logger.Info("Audio Fire event invalid");
            }

            return SingletonManager.Get<AudioEventManager>().FindById(filter(item));
        }
    }
}