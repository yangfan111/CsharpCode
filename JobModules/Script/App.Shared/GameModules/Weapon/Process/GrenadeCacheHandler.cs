using App.Shared.Components.Player;
using Assets.Utils.Configuration;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.EntityComponent;
using Core.Free;
using Core.Utils;
using System;
using System.Collections.Generic;
using App.Shared.Components.Weapon;
using Utils.Singleton;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="GrenadeCacheHandler" />
    /// </summary>
    public class GrenadeCacheHandler : IGrenadeCacheHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GrenadeCacheHandler));

        private Dictionary<int, int> heldGrenades;

        public Dictionary<int, int> HeldGrenades
        {
            get { return heldGrenades; }
        }

        private Func<GrenadeCacheDataComponent> grenadeDataExtractor;

        private Func<WeaponEntity> grenadeEntityExtractor;

        private IFreeArgs FreeArgs;

        private GrenadeCacheDataComponent GrandeCache
        {
            get { return grenadeDataExtractor(); }
        }

        /// <summary>
        /// 手雷config id集合
        /// </summary>
        private readonly List<int> grenadeConfigIds;

        private readonly List<int> grenadeValuedIds;

        //   , Func<WeaponEntity> grenadeEntiyExtractor
        public GrenadeCacheHandler(Func<GrenadeCacheDataComponent> extractor,
                                  Func<WeaponEntity>              in_grenadeEntiyExtractor, List<int> grenadeIds,
                                  IFreeArgs                       freeArgs)
        {
            grenadeDataExtractor   = extractor;
            grenadeConfigIds       = grenadeIds;
            grenadeEntityExtractor = in_grenadeEntiyExtractor;
            grenadeValuedIds       = new List<int>();
            heldGrenades           = new Dictionary<int, int>();
            FreeArgs               = freeArgs;
        }

        public bool AddCache(int id)
        {
            if (!grenadeConfigIds.Contains(id)) return false;
            int value = 0;
            heldGrenades.TryGetValue(id, out value);
            value            += 1;
            heldGrenades[id] =  value;
            Sync();
            return true;
        }

        public void SetCurr(int curr)
        {
            GrandeCache.LastId                       = curr;
            GrenadeEntity.weaponBasicData.ConfigId   = curr;
        }


        public int RemoveCache(int id)
        {
            int value;
            if (heldGrenades.TryGetValue(id, out value))
            {
                value -= 1;
                if (value < 1) heldGrenades.Remove(id);
                else heldGrenades[id] = value;
                Sync();
                return value;
            }

            return -1;
        }

        public void SendFreeTrigger(int expendId)
        {
            (FreeArgs as IEventArgs).Trigger(FreeTriggerConstant.GRENADE_ROMOVE, new IntPara("Id", expendId));
        }

        public void ClearCache(bool includeCurrent = true)
        {
            if (includeCurrent)
                grenadeEntityExtractor().weaponBasicData.Reset();
            grenadeEntityExtractor().weaponRuntimeData.Reset();
            heldGrenades.Clear();
            Sync();
        }

        public void ClearEntityCache()
        {
            grenadeEntityExtractor().weaponBasicData.Reset();
        }

        public int GetCount(int id)
        {
            return heldGrenades.ContainsKey(id) ? heldGrenades[id] : 0;
        }

        public List<int> GetOwnedIds()
        {
            grenadeValuedIds.Clear();
            for (int i = 0; i < grenadeConfigIds.Count; i++)
            {
                if (GetCount(grenadeConfigIds[i]) > 0)
                    grenadeValuedIds.Add(grenadeConfigIds[i]);
            }

            return grenadeValuedIds;
        }

        public int GetHoldGrenadeIndex()
        {
            return GetOwnedIds().IndexOf(LastGrenadeId);
        }

        public int FindUsable(bool autoStuff)
        {
            if (heldGrenades.Count == 0) return -1;
            int lastIndex = GrandeCache.LastId > 0 ? grenadeConfigIds.IndexOf(GrandeCache.LastId) : 0;
            int beginIndx = autoStuff ? lastIndex : lastIndex + 1;
            for (int i = beginIndx; i < grenadeConfigIds.Count + beginIndx; i++)
            {
                int id = grenadeConfigIds[i % grenadeConfigIds.Count];
                if (GetCount(id) > 0)
                    return id;
            }

            return -1;
        }

        public int LastGrenadeId
        {
            get { return GrandeCache.LastId; }
        }


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

        public WeaponEntity GrenadeEntity
        {
            get { return grenadeEntityExtractor(); }
        }
    }
}