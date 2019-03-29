using App.Shared.Util;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.EntityComponent;
using Core.Free;

using System;
using Assets.App.Shared.EntityFactory;
using Core.Utils;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="GrenadeWeaponAgent" />
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.ThrowingWeapon)]
    internal class GrenadeWeaponAgent : WeaponBaseAgent
    {
        private GrenadeCacheHelper bagCacheHelper;

        public GrenadeWeaponAgent(Func<EntityKey> in_holdExtractor, Func<EntityKey> in_emptyExtractor, EWeaponSlotType slot, GrenadeCacheHelper grenadeHelper) : base(in_holdExtractor, in_emptyExtractor, slot, grenadeHelper)
        {
            bagCacheHelper = grenadeHelper;
        }
        protected override WeaponEntity Entity
        {
            get
            {
                
                if(IsValid())
                {
                    entityCache = bagCacheHelper.GetGrenadeEntity();
                }
                else
                {
                    entityCache = WeaponEntityFactory.GetWeaponEntity(emptyKeyExtractor());
                }

                return entityCache;
            }
        }
        internal override EntityKey WeaponKey { get { return IsValid() ? bagCacheHelper.GetGrenadeEntity().entityKey.Value:emptyKeyExtractor(); } }
        public override bool IsValid()
        {
            entityCache = bagCacheHelper.GetGrenadeEntity();
            return entityCache.weaponBasicData.ConfigId > 0;
        }
        ///need auto stuff
        public override bool ExpendWeapon()
        {
            var expendId = ConfigId;
            if (expendId < 1) return false;
            bagCacheHelper.RemoveCache(expendId);
            if (!SharedConfig.IsOffline)
                bagCacheHelper.SendFreeTrigger(expendId);
            ReleaseWeapon();
            return true;
        }
        public  LoggerAdapter logger = new LoggerAdapter(typeof(GrenadeWeaponAgent));
        public override int ConfigId
        {
            get
            {
                var entityCache = bagCacheHelper.GetGrenadeEntity();
                if (entityCache == null)
                {
                    logger.Error("grenadeEntity is null");
                    return WeaponUtil.EmptyHandId;
                }

                if(entityCache.weaponBasicData.ConfigId>0)
                    return entityCache.weaponBasicData.ConfigId;
                return WeaponUtil.EmptyHandId;
            }
        }
        public override int FindNextWeapon(bool autoStuff)
        {
            return bagCacheHelper.FindUsable(autoStuff);
        }

        public override void ReleaseWeapon()
        {
            if (IsValid())
            {
                var grenadeEntity = bagCacheHelper.GetGrenadeEntity();
                grenadeEntity.weaponBasicData.Reset();
                grenadeEntity.weaponRuntimeData.Reset();
            }
        }

        /// <summary>
        /// 手雷武器替换操作：当前ConfigId必须已存在于库存，将手雷ENity替换 为当前configId
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="orient"></param>
        /// <param name="refreshParams"></param>
        /// <returns></returns>
        public override WeaponEntity ReplaceWeapon(EntityKey Owner, WeaponScanStruct orient, ref WeaponPartsRefreshStruct refreshParams)
        {
            if (bagCacheHelper.ShowCount(orient.ConfigId) == 0) return null;
            refreshParams.lastWeaponKey = WeaponKey;
            ReleaseWeapon();
            bagCacheHelper.SetCurr(orient.ConfigId);
            WeaponPartsStruct parts = WeaponPartUtil.CreateParts(orient);
            refreshParams.weaponInfo = orient;
            refreshParams.slot = handledSlot;
            refreshParams.oldParts = new WeaponPartsStruct();
            refreshParams.newParts = parts;
            refreshParams.armInPackage = true;
            return bagCacheHelper.GetGrenadeEntity();
        }
    }
}
