using App.Shared.Components.Player;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Utils;
using System;
using System.Collections.Generic;
using Utils.Singleton;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="GrenadeSlotHelper" />
    /// </summary>
    public class GrenadeSlotHelper : IGrenadeCacheHelper
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GrenadeSlotHelper));

        private readonly Dictionary<int, int> heldGrenades;

        private Func<GrenadeCacheDataComponent> grenadeDataExtractor;

        private Func<WeaponEntity> grenadeEntityExtractor;

        private GrenadeCacheDataComponent GrandeCache { get { return grenadeDataExtractor(); } }

        /// <summary>
        /// 手雷config id集合
        /// </summary>
        private readonly List<int> grenadeConfigIds;

        private readonly List<int> grenadeValuedIds;

        public GrenadeSlotHelper(Func<GrenadeCacheDataComponent> extractor, Func<WeaponEntity> grenadeEntiyExtractor, List<int> grenadeIds)
        {
            grenadeDataExtractor = extractor;
            var configs = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigs();
            grenadeConfigIds = grenadeIds;
            grenadeValuedIds = new List<int>();
            heldGrenades = new Dictionary<int, int>();
        }

        public bool AddCache(int id)
        {
            if (!grenadeConfigIds.Contains(id)) return false;
            int value;
            if (heldGrenades.TryGetValue(id, out value))
            {
                heldGrenades[id] += 1;
            }
            else
            {
                heldGrenades.Add(id, 1);
            }
            Sync();
            return true;
        }

        public bool RemoveCache(int id)
        {
            if (!grenadeConfigIds.Contains(id)) return false;
            int value;
            if (heldGrenades.TryGetValue(id, out value))
            {
                value -= 1;
                if (value < 1) heldGrenades.Remove(id);
                else heldGrenades[id] = value;
                Sync();
                return true;
            }
            return false;
        }

        public void ClearCache()
        {
            lastGrenadeId = 0;
            heldGrenades.Clear();
            Sync();
        }

        public int GetCount(int id)
        {
            return heldGrenades.ContainsKey(id) ?
                heldGrenades[id] : 0;
        }

        public List<int> GetOwnedIds()
        {
            grenadeValuedIds.Clear();
            foreach (KeyValuePair<int, int> pair in heldGrenades)
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
        public int FindAutomatic(int lastId)
        {
            if (heldGrenades.Count == 0) return -1;
            int lastIndex = lastId > 0 ? grenadeConfigIds.IndexOf(lastId) : 0;
            for(int i= lastIndex; i< grenadeConfigIds.Count+ lastIndex; i++)
            {
                int id = grenadeConfigIds[ i % grenadeConfigIds.Count];
                if (GetCount(id) > 0)
                    return id;
            }
            return -1;
        }

        /// <summary>
        /// 手动切换手雷
        /// </summary>
        public int FindManually(int lastId)
        {
            if (heldGrenades.Count == 0) return -1;
            int lastIndex = lastId > 0 ? grenadeConfigIds.IndexOf(lastId) : 0;
            for (int i = lastIndex+1; i < grenadeConfigIds.Count + lastIndex+1; i++)
            {
                int id = grenadeConfigIds[i % grenadeConfigIds.Count];
                if (GetCount(id) > 0)
                    return id;
            }
            return -1;
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

        private void SetCache(int id, int count)
        {
            if (!grenadeConfigIds.Contains(id)) return;
            heldGrenades[id] = count;
        }

        private void Sync()
        {
            for (int i = 0; i < grenadeConfigIds.Count; i++)
                GrandeCache.GrenadeArr[i].grenadeCount = GetCount(grenadeConfigIds[i]);
        }

        public void Rewind()
        {
            for (int i = 0; i < grenadeConfigIds.Count; i++)
                SetCache(grenadeConfigIds[i], GrandeCache.GrenadeArr[i].grenadeCount);
        }

        public int ShowCount(int id)
        {
            return GetCount(id);
        }

        public WeaponEntity GetGrenadeEntity (){ return grenadeEntityExtractor(); }


    }
}
