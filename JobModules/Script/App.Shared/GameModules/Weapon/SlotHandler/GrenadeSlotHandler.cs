using App.Shared.Util;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using System;
using Utils.Singleton;

namespace App.Shared.GameModules.Weapon
{
    [WeaponSpecies(EWeaponSlotType.ThrowingWeapon)]
    internal class GrenadeSlotHandler : CommonSlotHandler
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(GrenadeSlotHandler));
        private GrenadeSlotHelper bagCacheHelper;
        
        //private PlayerEntity _playerEntity;
        public override bool HasBagData { get { return true; } }
        internal override void RecordLastWeaponId(int lastId)
        {
            base.RecordLastWeaponId(lastId);
            bagCacheHelper.CacheLastGrenade(lastId);
        }

        //尝试更换手雷或拿出手雷操作
        internal override void Replace()
        {
        }

        internal override int FindNext(bool autoStuff)
        {
            if (autoStuff)
                return bagCacheHelper.FindAutomatic(lastUsageId);
            return bagCacheHelper.FindManually(lastUsageId);

        }
        public override void SetHelper(ISlotHelper in_helper)
        {
            base.SetHelper(in_helper);
            bagCacheHelper = (GrenadeSlotHelper)in_helper;
        }
        internal override void Expend(WeaponComponentsAgent agent, System.Action<WeaponSlotExpendStruct> expendCb)
        {
            bagCacheHelper.RemoveCache(agent.ConfigId);
            if (expendCb != null)
            {
                var paramsData = new WeaponSlotExpendStruct(handledSlot, true, true);
                expendCb(paramsData);
            }
        }
        internal override WeaponEntity ReplaceWeapon(WeaponComponentsAgent weaponSlotAgent, EntityKey Owner, WeaponScanStruct orient,  ref WeaponPartsRefreshStruct refreshParams)
        {
            var grenadeEntity = bagCacheHelper.GetGrenadeEntity();
            AssertUtility.Assert(grenadeEntity != null, "grande entity should initialize first time!");
            grenadeEntity.Recycle();
            grenadeEntity.weaponBasicData.SyncSelf(orient);
            refreshParams.weaponInfo = orient;
            refreshParams.slot = handledSlot;
            refreshParams.armInPackage = true;
            refreshParams.SetLastKey(weaponSlotAgent.WeaponKey);
            return grenadeEntity;
            //var avatarId = orient.AvatarId;
            //if (avatarId < 1)
            //    avatarId = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(orient.ConfigId).AvatorId;
            //refreshParams.oldParts = WeaponPartsStruct.Default;
            //refreshParams.newParts = WeaponPartsStruct.Default;
        }
        /// <summary>
        /// 投掷实例在玩家初始化中统一创建销毁
        /// </summary>
        /// <param name="agent"></param>
        internal override void DestroyWeapon(WeaponComponentsAgent agent)
        {
            agent.Reset();

        }
        internal override void ReleaseWeapon(WeaponComponentsAgent agent)
        {
            agent.Reset();
        }


    }
}
