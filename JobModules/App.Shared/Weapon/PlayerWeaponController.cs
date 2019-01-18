using Core.Bag;
using Core.Utils;
using App.Shared.Components;
using Utils.Appearance;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.CharacterState;
using System;
using Utils.Singleton;
using App.Shared.Util;
using WeaponConfigNs;
using Core;

namespace App.Shared.WeaponLogic
{
    public partial class PlayerWeaponController: IPlayerWeaponController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponController));

        WeaponEntityFacade entityFacde;
        WeaponBagSlotsAux slotsAux;
        PlayerWeaponComponentAgent componentAgent;
        public PlayerWeaponController(
            PlayerWeaponComponentAgent weaponAgent)
        {
            componentAgent = weaponAgent;
            slotsAux = new WeaponBagSlotsAux();
        }
        public void AttachFacade(WeaponEntityFacade in_facade)
        {
            entityFacde = in_facade;
        }
        void OnDrawWeaponCallback(WeaponInfo weapon, EWeaponSlotType slot)
        {
            OnSlotMountFinish(weapon, slot);
            entityFacde.Appearance_RemountP3WeaponOnRightHand();
        }
        void OnUnmountWeaponCallback(int weaponId, Action onfinish)
        {
            entityFacde.Appearance_UnmountWeaponFromHand();
            componentAgent.SetHeldSlot__TypeDetail(EWeaponSlotType.None);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                entityFacde.UnmountC4();
            }
            if (null != onfinish)
            {
                onfinish();
            }
        }
        /// <summary>
        /// 相比UseWeapon多了动作,需经由UserCmd触发
        /// </summary>
        /// <param name="slot"></param>
        public void DrawSlotWeapon(EWeaponSlotType slot)
        {
            Logger.DebugFormat("DrawWeapon {0}", slot);
            if (componentAgent.IsHeldSlotType(slot))
                return;
            WeaponInfo lastWeapon = componentAgent.HeldSlotWeaponInfo;
            WeaponInfo destWeapon = componentAgent.GetSlot__WeaponInfo(slot);
            if (destWeapon.Id < 1)
                return;

            bool armOnLeft = slot == EWeaponSlotType.PrimeWeapon2;
            float holsterParam = (componentAgent.IsHeldSlotType(EWeaponSlotType.PrimeWeapon2)) ?
                                    AnimatorParametersHash.Instance.HolsterFromLeftValue :
                                    AnimatorParametersHash.Instance.HolsterFromRightValue;
            float drawParam = armOnLeft ?
                                    AnimatorParametersHash.Instance.DrawLeftValue :
                                    AnimatorParametersHash.Instance.DrawRightValue;
            float switchParam = holsterParam * 10 + drawParam;
            AppearanceSpecific();
            DoDrawInterrupt();
            if (lastWeapon.Id > 0)
            {
                entityFacde.CharacterState_SwitchWeapon(() => WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft),
                    () => OnDrawWeaponCallback(destWeapon, slot), switchParam);
            }
            else
            {
                WeaponToHand(destWeapon.Id, lastWeapon.Id, slot, armOnLeft);
                OnSlotMountFinish(destWeapon, slot);
                entityFacde.CharacterState_Draw(entityFacde.Appearance_RemountP3WeaponOnRightHand, drawParam);
            }
        }
        private void AppearanceSpecific()
        {
            if (componentAgent.HeldSlotType == EWeaponSlotType.PrimeWeapon2)
                entityFacde.Appearance_MountP3WeaponOnAlternativeLocator();
        }
        private void DoDrawInterrupt()
        {
            entityFacde.CharacterState_DrawInterrupt();
        }

        public void TryMountSlotWeapon(EWeaponSlotType slot)
        {
            WeaponInfo heldWeapon = componentAgent.HeldSlotWeaponInfo;
            WeaponInfo weaponInfo;
            if (componentAgent.TryGetSlot__WeaponInfo(slot, out weaponInfo))
            {
                WeaponToHand(weaponInfo.Id, heldWeapon.Id, slot);
                OnSlotMountFinish(weaponInfo, slot);
            }

        }
        public void UnmountHeldWeapon(Action onfinish)
        {
            WeaponInfo weaponInfo = componentAgent.HeldSlotWeaponInfo;
            AppearanceSpecific();
            float holsterParam = (componentAgent.HeldSlotType == EWeaponSlotType.PrimeWeapon2) ?
                AnimatorParametersHash.Instance.HolsterFromLeftValue :
                AnimatorParametersHash.Instance.HolsterFromRightValue;
            entityFacde.CharacterState_Unmount(() => OnUnmountWeaponCallback(weaponInfo.Id, onfinish), holsterParam);

        }

        public void UnmountHeldWeapon()//float holsterParam)
        {
            UnmountHeldWeapon(null);
        }

        public void ForceUnmountHeldWeapon()
        {
            entityFacde.Appearance_UnmountWeaponFromHand();
            componentAgent.SetHeldSlot__TypeDetail(EWeaponSlotType.None);
            entityFacde.ThrowAction_Execute();
        }


        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeaponId))
            {
                entityFacde.UnmountC4();
            }
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                entityFacde.MountC4(weaponId);
            }
            WeaponInPackage pos = slot.ToWeaponInPackage();
            entityFacde.Appearance_MountWeaponToHand(pos);
            if (armOnLeft)
                entityFacde.Appearance_MountP3WeaponOnAlternativeLocator();
        }

        private void OnSlotMountFinish(WeaponInfo weapon, EWeaponSlotType slot)
        {
            componentAgent.SetHeldSlot__TypeDetail(slot);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == GameMode.Normal)
                {
                    //TODO 判断弹药数量是否足够，如果弹药不足，弹提示框
                    entityFacde.CharacterState_ReloadEmpty(() => { });
                }
            }
            else
            {
                //if (!bag.CurBolted)
                //{
                //    //TODO 拉栓动作
                //}
            }
        }

        public void DropSlotWeapon(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None || slot == EWeaponSlotType.GrenadeWeapon)
            {
                Logger.Error("drop weapon in slot none");
                return;
            }
            var lastWeapon = componentAgent.GetSlot__WeaponInfo(slot);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeapon.Id))
            {
                entityFacde.RemoveC4();
            }
            Logger.DebugFormat("DropWeapon {0}", slot);
            //=>TODO: _weaponActionListener.OnDrop(_playerEntity, slot);
            RemoveSlotWeapon(slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnDrop();

        }
        public void RemoveSlotWeapon(EWeaponSlotType slot)
        {
            if (componentAgent.RemoveSlotWeapon(slot))
            {
                Interrupt();
            }
        }
        public void RemoveSlotWeaponNoInterrupt(EWeaponSlotType slot)
        {
            componentAgent.RemoveSlotWeapon(slot);
        }
        public int GetSlotWeaponBulletCount(EWeaponSlotType slot)
        {
            return componentAgent.GetReservedBullet(slot);
        }

        public void SetSlotWeaponBulletCount(EWeaponSlotType slot, int count)
        {
            componentAgent.SetReservedBullet(slot, count);
        }

        public bool AutoPickUpWeapon(WeaponInfo weaponInfo)
        {
            NewWeaponConfigItem itemConfig;
            if (!WeaponUtil.VertifyWeaponInfo(weaponInfo, out itemConfig))
                return false;

            var weaponType = SingletonManager.Get<WeaponConfigManager>().GetWeaponType(weaponInfo.Id);
            if (!weaponType.CanAutoPick())
            {
                return false;
            }
            var slotType = GetMatchSlot((EWeaponType)itemConfig.Type);
            if (slotType != EWeaponSlotType.None)
            {
                if (componentAgent.IsWeaponSlotStuffed(slotType))
                {
                    return false;
                }
                var noWeaponInHand = componentAgent.HeldSlotType == EWeaponSlotType.None;
                //=>TODO:
                //_weaponActionListener.OnPickup(_playerEntity, slotType);
                ReplaceWeaponToSlot(slotType, weaponInfo, false);
                if (noWeaponInHand)
                {
                    TryMountSlotWeapon(slotType);
                }
                return true;
            }
            return false;
        }

        private EWeaponSlotType GetMatchSlot(EWeaponType weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    var hasPrime = componentAgent.IsWeaponSlotStuffed(EWeaponSlotType.PrimeWeapon1);
                    if (hasPrime && WeaponUtil.IsSlotValid(EWeaponSlotType.PrimeWeapon2))
                        return EWeaponSlotType.PrimeWeapon2;
                    return EWeaponSlotType.PrimeWeapon1;
                default:
                    return weaponType.ToWeaponSlot();
            }
        }

        public void Interrupt()
        {
            entityFacde.CharacterState_Interrupt();
        }
        public WeaponInfo PickUpWeapon(WeaponInfo weaponInfo)
        {
            NewWeaponConfigItem weaponCfg;
            if (!WeaponUtil.VertifyWeaponInfo(weaponInfo, out weaponCfg))
                return WeaponInfo.Empty;
            var slotType = GetMatchSlot((EWeaponType)weaponCfg.Type);
            if (slotType != EWeaponSlotType.None)
            {
                //=>TODO:    _weaponActionListener.OnPickup(_playerEntity, slotType);
                //除去手雷已经填充到当前槽位的情况
                if (FilterGrenadeStuffedCond(slotType))
                {
                    WeaponInfo last;
                    if (ReplaceWeaponToSlot(slotType, weaponInfo, out last))
                    {
                        TryMountSlotWeapon(slotType);
                        return last;
                    }
                }
            }
            return WeaponInfo.Empty;
        }

        public void OnSpend(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None)
                return;
            //=>TODO:   _weaponActionListener.OnCost(_playerEntity, slot);
            var handler = slotsAux.FindHandler(slot);
            handler.OnSpend(componentAgent, OnWeaponAutoRestuff);
        }
        /// <summary>
        /// 武器消耗完完自动填充逻辑
        /// </summary>
        /// <param name="slot"></param>
        private void OnWeaponAutoRestuff(WeaponSpendCallbackData cbData)
        {
            //消耗掉当前武器
            if (cbData.needRemoveCurrent)
                RemoveSlotWeaponNoInterrupt(cbData.slotType);
            //自动填充下一项武器
            if (componentAgent.IsWeaponSlotStuffed(cbData.slotType) || !cbData.needAutoRestuff) return;
            var handler = slotsAux.FindHandler(cbData.slotType);
            int nextId = handler.PickNextId(false);
            if (nextId > 0)
            {
                WeaponInfo last;
                if (ReplaceWeaponToSlot(cbData.slotType, new WeaponInfo() { Id = nextId }, out last))
                {
                    TryMountSlotWeapon(cbData.slotType);
                }
            }
        }
        /// <summary>
        /// 当前槽位同种武器切换逻辑
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="nextWeaponId"></param>
        private void OnSameSpeciesSwitch(EWeaponSlotType slotType, int nextWeaponId)
        {
            if (ReplaceWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo
            {
                Id = nextWeaponId,
            }))
            {
                TryMountSlotWeapon(EWeaponSlotType.GrenadeWeapon);

            }
        }
        /// <summary>
        /// 不处理手雷已装备情况
        /// </summary>
        /// <param name="slotType"></param>
        /// <returns></returns>
        private bool FilterGrenadeStuffedCond(EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.GrenadeWeapon ||
               !componentAgent.IsWeaponSlotStuffed(slotType);

        }

        public void SwitchIn(EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
                return;
            var from_slot = componentAgent.HeldSlotType;
            var handler = slotsAux.FindHandler(in_slot);
            WeaponInfo wpInfo = componentAgent.GetSlot__WeaponInfo(in_slot);
            if (!WeaponUtil.VertifyWeaponInfo(wpInfo)) return;
           handler.SwitchIn(from_slot, OnSwitchInExecute);
          

            //   _playerEntity.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
        }
        private void OnSwitchInExecute(EWeaponSlotType slot,int nextId)
        {
            //_playerEntity.weaponLogic.State.OnSwitchWeapon();
            if (nextId <1)
                DrawSlotWeapon(slot);
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify = true)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(slotType, weaponInfo, vertify, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, out WeaponInfo lastWp)
        {
            return ReplaceWeaponToSlot(slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify, out WeaponInfo lastWp)
        {
            lastWp = WeaponInfo.Empty;
            if (vertify)
            {
                if (slotType == EWeaponSlotType.None) return false;
                var weaonCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponInfo.Id);
                if (weaonCfg == null)
                    return false;
            }
            if (!WeaponUtil.IsSlotValid(slotType)) return false;
            var errCode = componentAgent.AddWeaponToSlot(slotType, weaponInfo);
            if (errCode == Err_WeaponLogicErrCode.Sucess)
            {
                var handler = slotsAux.FindHandler(slotType);
                handler.StuffEnd();
                return true;


            }
            else
            {
                throw new System.Exception("Add weapon to slot failed");


            }
            return false;
        }

    }

}
