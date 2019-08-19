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
        private GrenadeCacheHandler _bagCacheHandler;

        public GrenadeWeaponAgent(Func<EWeaponSlotType, EntityKey> in_holdExtractor, Func<EntityKey>    in_emptyExtractor,
                                  EWeaponSlotType slot,             GrenadeCacheHandler grenadeHandler) :
            base(in_holdExtractor, in_emptyExtractor, slot, grenadeHandler)
        {
            _bagCacheHandler = grenadeHandler;
        }

        protected override WeaponEntity Entity
        {
            get
            {
                if (IsValid())
                {
                    entityCache = _bagCacheHandler.GrenadeEntity;
                }
                else
                {
                    entityCache = WeaponEntityFactory.GetWeaponEntity(emptyKeyExtractor());
                }

                return entityCache;
            }
        }

        public override EntityKey WeaponKey
        {
            get { return IsValid() ? _bagCacheHandler.GrenadeEntity.entityKey.Value : emptyKeyExtractor(); }
        }

        public override bool IsValid()
        {
            var ret = _bagCacheHandler.GrenadeEntity.weaponBasicData.ConfigId > 0;
            if (ret)
                entityCache = _bagCacheHandler.GrenadeEntity;
            return ret;
        }

        /// need auto stuff
        /// <param name="reservedBullet"></param>
        public override bool ExpendWeapon(int reservedBullet)
        {
            var expendId = ConfigId;
            if (expendId < 1) return false;
            _bagCacheHandler.RemoveCache(expendId);
            if (!SharedConfig.IsOffline)
                _bagCacheHandler.SendFreeTrigger(expendId);
            ReleaseWeapon();
            return true;
        }
        internal override bool CanApplyPart
        {
            get { return false; }
        }
        public LoggerAdapter logger = new LoggerAdapter(typeof(GrenadeWeaponAgent));

        public override int ConfigId
        {
            get
            {
                if (IsValid())
                    return _bagCacheHandler.GrenadeEntity.weaponBasicData.ConfigId;
                return WeaponUtil.EmptyHandId;
            }
        }

        public override int FindNextWeapon(bool autoStuff)
        {
            return _bagCacheHandler.FindUsable(autoStuff);
        }

        public override void ReleaseWeapon()
        {
            if (IsValid())
            {
                entityCache = _bagCacheHandler.GrenadeEntity;
                entityCache.weaponBasicData.Reset();
                entityCache.weaponRuntimeData.Reset();
            }
        }

        /// <summary>
        /// 手雷武器替换操作：当前ConfigId必须已存在于库存，将手雷ENity替换 为当前configId
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="orient"></param>
        /// <param name="refreshParams"></param>
        /// <returns></returns>
        public override WeaponEntity ReplaceWeapon(EntityKey                    Owner, WeaponScanStruct orient,
                                                   ref WeaponPartsRefreshStruct refreshParams)
        {
            if (_bagCacheHandler.ShowCount(orient.ConfigId) == 0) return null;
            refreshParams.lastWeaponKey = WeaponKey;
            ReleaseWeapon();
            _bagCacheHandler.SetCurr(orient.ConfigId);
            WeaponPartsStruct parts = WeaponPartUtil.CreateParts(orient);
            refreshParams.weaponInfo   = orient;
            refreshParams.slot         = handledSlot;
            refreshParams.oldParts     = new WeaponPartsStruct();
            refreshParams.newParts     = parts;
            refreshParams.armInPackage = true;
            return _bagCacheHandler.GrenadeEntity;
        }
    }
}