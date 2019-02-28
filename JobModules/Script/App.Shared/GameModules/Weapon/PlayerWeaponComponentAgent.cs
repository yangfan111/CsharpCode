using Core.Utils;
using XmlConfig;
using WeaponConfigNs;
using Core.Configuration;
using Core.Enums;
using App.Shared.Util;
using Utils.Appearance;
using Utils.Singleton;
using System;
using Core;
using Assets.Utils.Configuration;
using App.Shared.Components.Bag;
using Core.WeaponLogic.Attachment;
using App.Shared.Audio;
using App.Shared.WeaponLogic;
using Core.EntityComponent;
using App.Shared.Components.Weapon;

namespace App.Shared.GameModules.Weapon
{
    public partial class PlayerWeaponComponentAgent 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponComponentAgent));
        /// <summary>
        /// WeaponSlotComponent
        /// </summary>
        private readonly WeaponSlotComponenExtractor slotExtractor;
        /// <summary>
        /// WeaponStateComponent
        /// </summary>
        private readonly BagStateComponentExtractor stateExtractor;
        /// <summary>
        /// weaponController，正常状态下不应对Controlller持有
        /// </summary>
        private  PlayerWeaponController controller;

        private PlayerEntity _playerEntity;

        public PlayerWeaponComponentAgent(
            PlayerEntity playerEntity,
            WeaponSlotComponenExtractor in_slotExtractor, BagStateComponentExtractor in_stateExtractor)
        {
            _playerEntity = playerEntity; 
            slotExtractor = in_slotExtractor;
            stateExtractor = in_stateExtractor;
          //  controller = new PlayerWeaponController(this);
        }
    
        internal bool RemoveSlotWeapon(Contexts contexts, EWeaponSlotType slot,System.Action<Contexts> onSetProcess)
        {
            var weaponEntityId = _playerEntity.GetWeaponInSlot(slot);
            if(!weaponEntityId.HasValue)
            {
                return false;
            }
            var weaponEntity = contexts.weapon.GetEntityWithEntityKey(new EntityKey(weaponEntityId.Value, (short)EEntityType.Weapon));
            if(null != weaponEntity)
            {
                if(SharedConfig.IsServer)
                {
                    weaponEntity.RemoveOwner();
                }
                _playerEntity.SetWeaponInSlot(slot, 0);
            }
            else
            {
                Logger.ErrorFormat("weaponEntity with id {0} in slot {1} doesn't exist ", weaponEntityId.Value, slot);
            }
            if (slot == CurrSlotType)
            {
                SetCurrSlotTypeProcess(contexts, EWeaponSlotType.None, onSetProcess);
            }
            WeaponInPackage pos = slot.ToWeaponInPackage();
            return true;
        }
     
        internal void SetSlotWeaponBullet(Contexts contexts, int bullet)
        {
            SetSlotWeaponBullet(contexts, CurrSlotType,bullet);
        }
        internal void SetSlotWeaponBullet(Contexts contexts, EWeaponSlotType slot, int bullet)
        {
            var weaponComp = slotExtractor(contexts, slot);
            if(null != weaponComp)
            {
                weaponComp.Bullet = bullet;
            }
            else
            {
                Logger.ErrorFormat("weapon data in slot {0} doesn't exist", slot);
            }
            //if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
        }

        internal void SetSlotFireMode(Contexts contexts, EWeaponSlotType slot, EFireMode mode)
        {
            var weapon = slotExtractor(contexts, slot);
            NewWeaponConfigItem cfg;
            if (WeaponUtil.VertifyWeaponComponent(weapon,out cfg) == EFuncResult.Success)
            {
                weapon.FireMode = (int)mode;
                //GameAudioMedium.PerformOnFireModelSwitch(cfg);
            }
        }

        internal void SetCurrSlotType(EWeaponSlotType slot)
        {
            _playerEntity.bagState.CurSlot = (int)slot;
        }
        internal void SetLastWeaponSlot(int slot)
        {
            var comp = stateExtractor();
            AssertUtility.Assert(comp != null);
            comp.LastSlot = slot;
        }
        internal void SetCurrSlotTypeProcess(Contexts contexts, EWeaponSlotType slot, System.Action<Contexts> onSetProcess)
        {
            SetCurrSlotType(slot);
            onSetProcess(contexts);
          
            if (slot != EWeaponSlotType.None)
            {
                SetLastWeaponSlot((int)slot);
            }
        }
        /// <summary>
        /// 自动查找当前可用手雷,no vertify
        /// </summary>
        /// <param name="grenadeComp"></param>
        private void TryStuffEmptyGrenadeSlot()
        {
            //=>TODO:
            ////var grenadeBagAgent = _playerEntity.grenadeInventoryHolder.Inventory;
            ////int nextId = grenadeBagAgent.PickNextGrenadeInstance();
            ////if (nextId < 1) return;
            ////WeaponInfo wpInfo;
            ////Err_WeaponLogicErrCode errCode = AddWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo() { Id = nextId }, out wpInfo);
            ////if (errCode != Err_WeaponLogicErrCode.Sucess)
            ////{
            ////    throw new System.Exception("Stuff empty grenade slot failed");
            ////}
        }



        public void TryStuffGrenadeToSlot()
        {
            //var cmp = slotExtractor(EWeaponSlotType.GrenadeWeapon);
            //AssertUtility.Assert(cmp != null);
            //if (!WeaponUtil.VertifyWeaponComponentStuffed(cmp))
            //{
            //    TryStuffEmptyGrenadeSlot(cmp);
            //}

        }
        /// <summary>
        /// 手雷物品自动填充
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="autoStuffSlot"></param>
        /// <returns></returns>
        private WeaponBasicInfoComponent TryStuffEmptyGrenadeSlot(EWeaponSlotType slot, bool autoStuffSlot = false)
        {
            return null; 
            //var comp = slotExtractor(slot);
            //if (autoStuffSlot && comp != null && slot == EWeaponSlotType.GrenadeWeapon && comp.Id < 1)
            //    TryStuffEmptyGrenadeSlot(comp);
            //return comp;
        }

        internal Err_WeaponLogicErrCode AddWeaponToSlot(Contexts contexts, EWeaponSlotType slot, WeaponInfo weapon, WeaponPartsModelRefresh onModelWeaponPartsRefresh, out WeaponInfo lastWeapon)
        {
            lastWeapon = new WeaponInfo();
            if (slot == EWeaponSlotType.None)
                return Err_WeaponLogicErrCode.Err_SlotNone;
            int? lastWeaponId = null;
            int? lastWeaponKey = null;
            if (_playerEntity.HasWeaponInSlot(contexts, slot))
            {
                lastWeapon = GetSlotWeaponInfo(contexts, slot);
                lastWeaponId = lastWeapon.Id;
                lastWeaponKey = lastWeapon.weaponKey;
            }
            WeaponPartsStruct parts = weapon.GetParts();
            ProcessWeaponEntityForPickUp(contexts, weapon, slot, lastWeaponKey);
            var avatarId = weapon.AvatarId;
            if (avatarId < 1)
            {
                avatarId = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weapon.Id).AvatorId;
            }
            WeaponPartsRefreshData refreshData = new WeaponPartsRefreshData();
            refreshData.weaponInfo = weapon;
            refreshData.slot = slot;
            refreshData.oldParts = new WeaponPartsStruct();
            refreshData.newParts = parts;
            refreshData.mountInPackage = true;
            if(lastWeaponId.HasValue)
            {
                refreshData.SetRefreshLogic(lastWeaponId.Value);
            }
            onModelWeaponPartsRefresh(contexts, refreshData);
            return Err_WeaponLogicErrCode.Sucess;
        }

        private void ProcessWeaponEntityForPickUp(Contexts contexts, WeaponInfo weapon, EWeaponSlotType slot, int? lastWeaponKey)
        {
            if(weapon.weaponKey == 0)
            {
                var weaponEntity = _playerEntity.AddWeaponEntity(contexts, weapon);
                _playerEntity.SetWeaponInSlot(slot, weaponEntity.entityKey.Value.EntityId);

                weapon.CopyToWeaponComponentWithDefaultParts(weaponEntity.weaponBasicInfo);
            }
            else
            {
                _playerEntity.SetWeaponInSlot(slot, weapon.weaponKey);
                var weaponEntity = contexts.weapon.GetEntityWithEntityKey(new EntityKey(weapon.weaponKey, (short)EEntityType.Weapon));
                if(null != weaponEntity)
                {
                    weaponEntity.AddOwner(_playerEntity.entityKey.Value);
                }
            }
            if(lastWeaponKey.HasValue)
            {
                var lastWeaponEntity = contexts.weapon.GetEntityWithEntityKey(new EntityKey(lastWeaponKey.Value, (short)EEntityType.Weapon));
                if(null != lastWeaponEntity)
                {
                    //TODO 客户端和服务器逻辑分离
                    if(SharedConfig.IsServer)
                    {
                        lastWeaponEntity.RemoveOwner();
                    }
                }
                else
                {
                    Logger.ErrorFormat("GetLastWeaponEntity {0} Failed", lastWeaponKey.Value);
                }
            }
        }

        /// <summary>
        /// 重置开火模式
        /// </summary>
        /// <param name="slot"></param>
        internal void ResetSlotFireModel(Contexts contexts, EWeaponSlotType slot)
        {
            var weaponComp = slotExtractor(contexts, slot);
            // 重置开火模式
            if (WeaponUtil.VertifyWeaponComponent(weaponComp) == EFuncResult.Success)
            {
                if (weaponComp.FireMode == 0)
                {
                    bool hasAutoFireModel = SingletonManager.Get<WeaponDataConfigManager>().HasAutoFireMode(weaponComp.WeaponId);
                    if (hasAutoFireModel)
                        SetSlotFireMode(contexts, slot, EFireMode.Auto);
                    else
                        SetSlotFireMode(contexts, slot, SingletonManager.Get<WeaponDataConfigManager>().GetFirstAvaliableFireMode(weaponComp.WeaponId));
                }
            }
        }




    }
}