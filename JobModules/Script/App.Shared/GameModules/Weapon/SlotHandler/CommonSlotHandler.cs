using App.Shared.Util;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using System;
using Utils.Singleton;

namespace App.Shared.GameModules.Weapon
{
    public class CommonSlotHelper:ISlotHelper
    {
        Func<WeaponEntity> emptyWeaponExtractor;
        public WeaponEntity GetEmptyWeapon() { return emptyWeaponExtractor(); }

        public CommonSlotHelper(Func<WeaponEntity> in_EmptyWeaponExtractor)
        {
            emptyWeaponExtractor = in_EmptyWeaponExtractor;
        }

    }
    /// <summary>
    /// 4 location : ground body hand pacakge
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon)]
    [WeaponSpecies(EWeaponSlotType.SecondaryWeapon)]
    [WeaponSpecies(EWeaponSlotType.PistolWeapon)]

    public class CommonSlotHandler
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonSlotHandler));
        protected int lastUsageId;
        protected EWeaponSlotType handledSlot;
        public IGrenadeCacheHelper GrenadeHelper { get { return slotHelper  as IGrenadeCacheHelper; } }

        public CommonSlotHelper CommonHelper { get { return slotHelper as CommonSlotHelper; } }

        protected ISlotHelper slotHelper;
        internal void SetSlotTarget(EWeaponSlotType slot)
        {
            handledSlot = slot;
        }
        public virtual void SetHelper(ISlotHelper in_helper)
        {
            slotHelper = in_helper;
        }
        public virtual bool HasBagData { get { return false; } }
        internal virtual void Expend(WeaponComponentsAgent agent, System.Action<WeaponSlotExpendStruct> expendCb)
        {
             agent.BaseComponent.Bullet-=1;
            if (expendCb != null)
            {
                var paramsData = new WeaponSlotExpendStruct(handledSlot,false, false);
                expendCb(paramsData);
            }

        }
  
  
        /// <summary>
        /// 装备槽位填充完成
        /// </summary>
        /// <returns></returns>
        internal virtual void RecordLastWeaponId(int lastId)
        {
            lastUsageId = lastId;
        }
        internal virtual void Recycle() { }
        //选择下一个可装备的武器id
        internal virtual int FindNext(bool autoStuff)
        {
            return -1;
        }
        /// <summary>
        /// weapon from body, hand to ground
        /// </summary>
        internal virtual void Drop()
        {

        }
        internal virtual void Replace()
        {

        }
        ///当前武器不为空手的情况下
        internal virtual void ReleaseWeapon(WeaponComponentsAgent agent)
        {
            agent.SetFlagNoOwner();
        }
        internal virtual void DestroyWeapon(WeaponComponentsAgent agent)
        {
            agent.SetFlagWaitDestroy();
        }
        /// <summary>
        /// release old weapon and create new one
        /// </summary>
        /// <param name="weaponSlotAgent"></param>
        /// <param name="Owner"></param>
        /// <param name="orient"></param>
        /// <param name="refreshParams"></param>
        /// <returns></returns>
        internal virtual WeaponEntity ReplaceWeapon(WeaponComponentsAgent weaponSlotAgent,EntityKey Owner, WeaponScanStruct orient, ref WeaponPartsRefreshStruct refreshParams)
        {
            WeaponEntity orientEntity = null;
            var orientKey = orient.WeaponKey;
            weaponSlotAgent.SetFlagNoOwner();
            ///捡起即创建
            orientEntity = WeaponEntityFactory.GetOrCreateWeaponEntity(Owner, ref orient);

            bool createNewEntity = orientEntity.entityKey.Value != orientKey;
           // playerWeaponAgent.AddSlotWeapon(slot, orientEntity.entityKey.Value);
            if (!createNewEntity)
                orient.CopyToWeaponComponentWithDefaultParts(orientEntity);
            else
                DebugUtil.LogInUnity("add new weapon :{0}", DebugUtil.DebugColor.Blue, orientEntity.entityKey.Value);

            WeaponPartsStruct parts = orient.GetParts();
            var avatarId = orient.AvatarId;
            if (avatarId < 1)
                avatarId = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(orient.ConfigId).AvatorId;
            refreshParams.weaponInfo = orient;
            refreshParams.slot = handledSlot;
            refreshParams.oldParts = new WeaponPartsStruct();
            refreshParams.newParts = parts;
            refreshParams.armInPackage = true;
            refreshParams.SetLastKey(weaponSlotAgent.WeaponKey);
            return orientEntity;
        }
    }
}
