using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Bag;
using Core.Utils;
using System.Collections.Generic;
using System.Linq;
using Utils.Singleton;
using App.Shared.Components.Bag;
using System;

namespace App.Shared.WeaponLogic
{
    public class GrenadeBagCacheAgent : IGrenadeBagCacheAgent
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GrenadeBagCacheAgent));
        private readonly Dictionary<int, int> acquiredGrenades;
        private GrenadeInventoryDataComponent sharedComponentData;
        private readonly List<int> grenadeConfigIds;
        private readonly List<int> grenadeValuedIds;

        public GrenadeBagCacheAgent(GrenadeInventoryDataComponent componentData)
        {
            sharedComponentData = componentData;
            var configs = SingletonManager.Get<WeaponConfigManager>().GetConfigs();
            grenadeConfigIds = new List<int>();
            grenadeValuedIds = new List<int>();
            acquiredGrenades = new Dictionary<int, int>();
            foreach (var config in configs)
            {
                switch ((EWeaponType)config.Value.Type)
                {
                    case EWeaponType.ThrowWeapon:
                        var subType = (EWeaponSubType)config.Value.SubType;
                        switch (subType)
                        {
                            case EWeaponSubType.BurnBomb:
                            case EWeaponSubType.FlashBomb:
                            case EWeaponSubType.FogBomb:
                            case EWeaponSubType.Grenade:
                                break;
                            default:
                                Logger.ErrorFormat("new subtype {0} in tactic weapon", subType);
                                break;
                        }
                        grenadeConfigIds.Add(config.Value.Id);
                        break;
                }
            }

        }
        public bool AddCache(int id)
        {
            if (!grenadeConfigIds.Contains(id)) return false;
            int value;
            if (acquiredGrenades.TryGetValue(id, out value))
            {
                acquiredGrenades[id] += 1;
            }
            else
            {
                acquiredGrenades.Add(id, 1);
            }
            Sync();
            return true;
        }

        public bool RemoveCache(int id)
        {
            if (!grenadeConfigIds.Contains(id)) return false;
            int value;
            if (acquiredGrenades.TryGetValue(id, out value))
            {
                value -= 1;
                if (value < 1) acquiredGrenades.Remove(id);
                else acquiredGrenades[id] = value;
                Sync();
                return true;
            }
            return false;
        }

        public void Recycle()
        {
            lastGrenadeId = 0;
            acquiredGrenades.Clear();
            Sync();
        }
        public int GetCount(int id)
        {
            return acquiredGrenades.ContainsKey(id) ?
                acquiredGrenades[id] : 0;
        }
        public List<int> GetOwnedIds()
        {
            grenadeValuedIds.Clear();
            foreach (KeyValuePair<int,int> pair in acquiredGrenades)
            {
                if (pair.Value > 0)
                    grenadeValuedIds.Add(pair.Key);
            }
            grenadeValuedIds.Sort();
            return grenadeValuedIds;
        }
        /// <summary>
        /// 扔出手雷实现自动填充
        /// </summary>
        /// <returns></returns>
        public int PickNextGrenadeInstance()
        {
            if (acquiredGrenades.Count < 1)
                return -1;
            var list = GetOwnedIds();
            if (lastGrenadeId < 1)
                return list[0];
            int finalIndx = list.FindIndex((data) => lastGrenadeId <= data);
            return Math.Max(finalIndx, 0);
        }
        /// <summary>
        /// 手动切换手雷
        /// </summary>
        public int PickNextGrenadeSpecies()
        {
            if (acquiredGrenades.Count < 1)
                return -1;
            int finalIndx = 0;
            var list = GetOwnedIds();
            if (lastGrenadeId > 0)
            {
                finalIndx = list.FindIndex((data) => lastGrenadeId < data);
                finalIndx = Math.Max(finalIndx, 0);
            }
            return list[finalIndx];
        }


        public void CacheLastGrenade(int lastId)
        {
            lastGrenadeId = lastId;
        }
        public void ClearLastCache()
        {
            lastGrenadeId = 0;
        }
        private int lastGrenadeId = 0;

        public void SetCache(int id, int count)
        {
            if (!grenadeConfigIds.Contains(id)) return;
            acquiredGrenades[id] = count;
            Sync();

        }
        private void Sync()
        {
            sharedComponentData.GrenadeCount1 = GetCount(grenadeConfigIds[0]);
            sharedComponentData.GrenadeCount2 = GetCount(grenadeConfigIds[1]);
            sharedComponentData.GrenadeCount3 = GetCount(grenadeConfigIds[2]);
            sharedComponentData.GrenadeCount4 = GetCount(grenadeConfigIds[3]);


        }

        public void Rewind()
        {
            SetCache(grenadeConfigIds[0], sharedComponentData.GrenadeCount1);
            SetCache(grenadeConfigIds[1], sharedComponentData.GrenadeCount2);
            SetCache(grenadeConfigIds[2], sharedComponentData.GrenadeCount3);
            SetCache(grenadeConfigIds[3], sharedComponentData.GrenadeCount4);
        }

        public int ShowCount(int id)
        {
            return GetCount(id);
        }



    }
}