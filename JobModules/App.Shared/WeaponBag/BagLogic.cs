using Core.Utils;
using Core.Bag;
using XmlConfig;
using WeaponConfigNs;
using Core.Configuration;
using Core.Enums;
using Utils.Configuration;
using Assets.Utils.Configuration;
using App.Shared.Util;
using Utils.Utils;
using App.Shared.Components.Bag;
using Core.WeaponLogic.Attachment;
using App.Shared.EntityFactory;
using Assets.XmlConfig;
using Utils.Appearance;
using App.Shared.Player;
using Utils.Singleton;
using App.Shared.Util;

namespace App.Shared.WeaponLogic
{
    public partial class WeaponBagLogic :IWeaponBagLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBagLogic));
        private INewWeaponConfigManager _newWeaponConfigManager;
        private PlayerEntity _playerEntity;

        public WeaponBagLogic(
            PlayerEntity playerEntity,
            INewWeaponConfigManager newWeaponConfigManager)
        {
            _playerEntity = playerEntity;
            _newWeaponConfigManager = newWeaponConfigManager; 
        }

        public bool HasWeapon()
        {
            return HasWeaponInSlot((EWeaponSlotType)_playerEntity.weaponState.CurrentWeaponSlot);
        }

        public void SetWeaponBullet(int count)
        {
            SetWeaponBullet(CurWeaponSlot, count);
        }

        public EFuncResult SetCurrentWeaponPart(int id)
        {
            return SetWeaponPart(CurWeaponSlot, id);
        }

        public EFuncResult SetWeaponPart(EWeaponSlotType slot, int id)
        {
            if (slot == EWeaponSlotType.None)
            {
                return EFuncResult.Exception;
            }

            var weaponComp = GetWeaponComponentBySlot(slot);
            if (weaponComp.Id < 1)
            {
                Logger.ErrorFormat("set attachment failed : no weapon in slot {0}", slot);
                return EFuncResult.Exception;
            }
            var lastParts = weaponComp.GetParts();
            var weaponCfg = _newWeaponConfigManager.GetConfigById(weaponComp.Id);
            if (null == weaponCfg)
            {
                return EFuncResult.Exception;
            }

            var realAttachId = BagUtility.GetRealAttachmentId(id, weaponCfg.Id);
            var match = SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(realAttachId, weaponCfg.Id);
            if (!match)
            {
                return EFuncResult.Failed;
            }

            if (WeaponValid(weaponComp))
            {
                var attachments = WeaponPartsUtil.ModifyParts(
                    weaponComp.GetParts(),
                    SingletonManager.Get<WeaponPartsConfigManager>().GetPartType(realAttachId),
                    realAttachId);
                weaponComp.ApplyParts(attachments);
            }
            else
            {
                Logger.ErrorFormat("No weapon in slot {0}", slot);
            }

            if (slot == CurWeaponSlot)
            {
                RefreshCurrentWeaponAttachmentLogic();
            }

            RefreshWeaponPartsModel(weaponComp.Id,
                slot,
                lastParts,
                weaponComp.GetParts());
            return EFuncResult.Success;
        }

        public void DeleteWeaponPart(EWeaponSlotType slot, EWeaponPartType part)
        {
            if (slot == EWeaponSlotType.None)
            {
                return;
            }
            var weaponComp = GetWeaponComponentBySlot(slot);
            if(null == weaponComp)
            {
                Logger.ErrorFormat("error : weaponcompent in slot {0} is null", slot);
                return;
            }
            var weapon = GetWeaponComponentBySlot(slot);
            var lastParts = weapon.GetParts();
            if (null != weaponComp)
            {
                var parts = WeaponPartsUtil.ModifyParts(
                    weaponComp.GetParts(),
                    part,
                    UniversalConsts.InvalidIntId);
                weaponComp.ApplyParts(parts);
            }

            if (slot == CurWeaponSlot)
                RefreshCurrentWeaponAttachmentLogic();

            var newParts = WeaponPartsUtil.ModifyParts(lastParts, part, UniversalConsts.InvalidIntId);
            newParts = newParts.ApplyDefaultParts(weapon.Id);
            RefreshWeaponPartsModel(weapon.Id, slot, lastParts, newParts);
        }

        public EWeaponSlotType PopLastWeaponSlot()
        {
            var last = (EWeaponSlotType)LastWeapon;
            if (last != EWeaponSlotType.None && GetWeaponInfo(last).Id > 0)
            {
                return last;
            }
            else
            {
                for (var s = EWeaponSlotType.None + 1; s < EWeaponSlotType.Length; s++)
                {
                    if (GetWeaponInfo(s).Id > 0)
                    {
                        return s;
                    }
                }
                return EWeaponSlotType.None;
            }
        }

        public void SetCurrentWeapon(EWeaponSlotType slot)
        {
            CurWeaponSlot = slot;
            RefreshCurrentWeaponLogic();
            // 需要执行刷新配件逻辑，因为配件会影响相机动作等属性
            RefreshCurrentWeaponAttachmentLogic();

            if (slot != EWeaponSlotType.None)
            {
                LastWeapon = (int)slot;
            }
        }

        public bool RemoveWeapon(EWeaponSlotType slot)
        {
            if (slot == GetCurrentWeaponSlot())
            {
                SetCurrentWeapon(EWeaponSlotType.None);
            }
            var comp = GetWeaponComponentBySlot(slot);
            if (!WeaponValid(comp))
            {
                return false;
            }
            comp.Clear();
            var pos = slot.ToWeaponInPackage();
            _playerEntity.appearanceInterface.Appearance.UnmountWeaponInPackage(pos);
            return true;
        }

        private void RefreshCurrentWeaponAttachmentLogic()
        {
            var curWeaponSlot = CurWeaponSlot;
            if (!curWeaponSlot.MayHasPart())
            {
                return;
            }
            var weapon = GetWeaponInfo(curWeaponSlot);
            if (weapon.Id > 0)
            {
                var attachments = weapon.GetParts();
                //直接影响数值，或者通过数值来体现的表现
                SetCurrentWeaponParts(attachments);
                //影响表现
                ApplyAttachentsEffect();
                // 添加到背包的时候会执行刷新模型逻辑
                //_playerEntity.RefreshWeaponModel(weapon.WeaponId, curWeaponSlot, attachments);
            }
            else
            {
                Logger.Error("weapon is null or item id is illegal ");
            }
        }

        private void RefreshCurrentWeaponLogic()
        {
            var slot = CurWeaponSlot;
            // 清理之前的枪械状态信息
            ClearWeaponState();
            var weapon = GetWeaponInfo(slot);
            if (weapon.Id > 0)
            {
                var config = _newWeaponConfigManager.GetConfigById(weapon.Id);
                if (null != config)
                {
                    _playerEntity.RefreshPlayerWeaponLogic(config.Id);
                    var weaponComp = GetWeaponComponentBySlot(slot);
                    // 重置开火模式
                    if (weaponComp.Id > 0)
                    {
                        if (weaponComp.FireMode == 0)
                        {
                            if(SingletonManager.Get<WeaponDataConfigManager>().HasAutoFireMode(weaponComp.Id))
                            {
                                SetFireMode(slot, EFireMode.Auto);
                            }
                            else
                            {
                                SetFireMode(slot, SingletonManager.Get<WeaponDataConfigManager>().GetFirstAvaliableFireMode(weaponComp.Id));
                            }
                        }
                        
                    }
                    else
                    {
                        Logger.ErrorFormat("weapon with slot {0} is null !!", slot);
                    }
                }
            }
            else
            {
                _playerEntity.RefreshPlayerWeaponLogic(UniversalConsts.InvalidIntId);
            }
        }

        public WeaponInfo GetCurrentWeaponInfo()
        {
            return GetWeaponInfo(CurWeaponSlot);
        }

        public int CurFireMode
        {
            get { return (int)GetFireMode(CurWeaponSlot); }
            set { SetFireMode(CurWeaponSlot, (EFireMode)value); }
        }

        public bool CurBolted
        {
            get { return GetWeaponBolted(CurWeaponSlot); }
            set { SetWeaponBolted(CurWeaponSlot, value); }
        }

        public int GetFireModeCount(EWeaponSlotType slot)
        {
            var weapon = GetWeaponInfo(slot);
            if (weapon.Id < 1)
            {
                return 1;
            }
            var count = SingletonManager.Get<WeaponDataConfigManager>().GetFireModeCountById(weapon.Id);
            return count;
        }
    
      
        private void ProcessMountWeaponInPackage(WeaponInPackage pos, int weaponId, int avatarId)
        {
            if(SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                _playerEntity.AddC4ToBag(weaponId);
            }
            else
            {
                _playerEntity.appearanceInterface.Appearance.MountWeaponInPackage(pos, avatarId);
            }
        }

        /// <summary>
        /// 如果没有当前武器就一定返回None
        /// </summary>
        /// <returns></returns>
        public EWeaponSlotType GetCurrentWeaponSlot()
        {
            if (HasCurWeapon)
            {
                return CurWeaponSlot;
            }
            else
            {
                CurWeaponSlot = EWeaponSlotType.None;
                return EWeaponSlotType.None;
            }
        }

        public int GetReservedBullet(EWeaponSlotType slot)
        {
            if (slot.IsSlotWithBullet())
            {
                return _playerEntity.modeLogic.ModeLogic.GetReservedBullet(_playerEntity, slot);
            }
            else
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("get reserved bullet from slot {0}", slot);
                }
            }
            return 0;
        }

        public void SetReservedBullet(EWeaponSlotType slot, int count)
        {
            if (slot.IsSlotWithBullet())
            {
                _playerEntity.modeLogic.ModeLogic.SetReservedBullet(_playerEntity, slot, count);
            }
            else
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("set reserved bullet to slot {0}", slot);
                }
            }
        }

        public void SetReservedBullet(int count)
        {
            _playerEntity.modeLogic.ModeLogic.SetReservedBullet(_playerEntity, CurWeaponSlot, count);
        }

        public int SetReservedBullet(EBulletCaliber caliber, int count)
        {
            return _playerEntity.modeLogic.ModeLogic.SetReservedBullet(_playerEntity, caliber, count);
        }

        public int GetReservedBullet(EBulletCaliber caliber)
        {
            return _playerEntity.modeLogic.ModeLogic.GetReservedBullet(_playerEntity, caliber);
        }

        public int GetReservedBullet()
        {
            return _playerEntity.modeLogic.ModeLogic.GetReservedBullet(_playerEntity, CurWeaponSlot);
        }

        public int GetWeaponBullet()
        {
            var slot = CurWeaponSlot;
            return GetWeaponBullet(slot);
        }

        public int GetWeaponAvatarId(int weaponId)
        {
            return _newWeaponConfigManager.GetAvatarByWeaponId(weaponId);
        }

        public bool HasCurWeapon
        {
            get { return WeaponValid(GetWeaponComponentBySlot(CurWeaponSlot)); }
        }

        public EWeaponSlotType CurWeaponSlot
        {
            get
            {
                if (_playerEntity.hasWeaponState)
                {
                    return (EWeaponSlotType)_playerEntity.weaponState.CurrentWeaponSlot;
                }
                else
                {
                    return EWeaponSlotType.None;
                }
            }
            set
            {
                if (_playerEntity.hasWeaponState)
                {
                    _playerEntity.weaponState.CurrentWeaponSlot = (int)value;
                }
            }
        }

        public int LastWeapon
        {
            get { return _playerEntity.weaponState.LastWeapon; }
            set { _playerEntity.weaponState.LastWeapon = value; }
        }

        public int CurrentWeaponId
        {
            get
            {
                var curComp = GetWeaponComponentBySlot(CurWeaponSlot);
                if (WeaponValid(curComp))
                {
                    return curComp.Id;
                }
                return 0;
            }
        }

        public bool HasWeaponInSlot(EWeaponSlotType slot)
        {
            return WeaponValid(GetWeaponComponentBySlot(slot));
        }


        public void SetWeaponBullet(EWeaponSlotType slot, int bullet)
        {
            var weaponComp = GetWeaponComponentBySlot(slot);

            if (WeaponValid(weaponComp))
                weaponComp.Bullet = bullet;
            else
                Logger.ErrorFormat("set weapon bullet failed : no weapon in slot {0}", slot);
        }

        public int GetWeaponBullet(EWeaponSlotType slot)
        {
            var weaponComp = GetWeaponComponentBySlot(slot);

            if (WeaponValid(weaponComp))
            {
                return weaponComp.Bullet;
            }

            Logger.ErrorFormat("get weapon bullet failed : no weapon in slot {0}", slot);
            return 0;
        }

        public void SetWeapon(EWeaponSlotType slot, int weaponId)
        {
            var weaponComp = GetWeaponComponentBySlot(slot);

            if (weaponComp != null)
            {
                weaponComp.Clear();
                weaponComp.Id = weaponId;
            }
            else
                Logger.ErrorFormat("set weapon failed : weaponcompent in slot {0} is null", slot);
        }

        public WeaponInfo GetLastWeapon()
        {
            var weapon = GetWeaponComponentBySlot((EWeaponSlotType)_playerEntity.weaponState.LastWeapon);
            if (null == weapon || weapon.Id < 1)
            {
                return new WeaponInfo();
            }
            return weapon.ToWeaponInfo(); 
        }

        public void ClearAttachments(EWeaponSlotType slot)
        {
            var weaponComp = GetWeaponComponentBySlot(slot);

            if (weaponComp != null)
                weaponComp.ResetAttachments();
            else
                Logger.ErrorFormat("set weapon failed : weaponcompent in slot {0} is null", slot);
        }

        public void SetWeaponBolted(EWeaponSlotType slot, bool bolted)
        {
            var weapon = GetWeaponComponentBySlot(slot);

            if (WeaponValid(weapon))
                weapon.IsBoltPulled = bolted;
            else
                Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
        }

        public bool GetWeaponBolted(EWeaponSlotType slot)
        {
            var weapon = GetWeaponComponentBySlot(slot);

            if (WeaponValid(weapon))
                return weapon.IsBoltPulled;

            Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
            return false;
        }

        public int GetFireMode(EWeaponSlotType slot)
        {
            var weapon = GetWeaponComponentBySlot(slot);

            if (WeaponValid(weapon))
                return weapon.FireMode;

            Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
            return (int)EFireMode.Manual;
        }

        public void SetFireMode(EWeaponSlotType slot, EFireMode mode)
        {
            var weapon = GetWeaponComponentBySlot(slot);

            if (WeaponValid(weapon))
                weapon.FireMode = (int)mode;
            else
                Logger.ErrorFormat("get weapon bolted failed : no weapon in slot {0} ", slot);
        }

        //private WeaponComponent GetWeaponComponentBySlot(EWeaponSlotType slot)
        //{
        //    return _playerEntity.GetWeaponComponentBySlot(slot);
        //}

        private static bool WeaponValid(EquipmentComponent weapon)
        {
            return weapon != null && weapon.Id > 0;
        }

        public void ApplyAttachentsEffect()
        {
            RefreshBreath();
        }

        /// <summary>
        /// 影响呼吸晃动幅度
        /// </summary>
        void RefreshBreath()
        {
            var breath = _playerEntity.weaponLogic.Weapon.GetBreathFactor();
            _playerEntity.appearanceInterface.FirstPersonAppearance.SightShift.SetAttachmentFactor(breath);
        }

        public void RefreshWeaponPartsModel(int weaponId, EWeaponSlotType slot, WeaponPartsStruct oldAttachment, WeaponPartsStruct newAttachments)
        {
            WeaponPartsUtil.RefreshWeaponPartModels(_playerEntity.appearanceInterface.Appearance, weaponId, oldAttachment, newAttachments, slot);
        }

        public void SetCurrentWeaponParts(WeaponPartsStruct attachments)
        {
            _playerEntity.weaponLogic.Weapon.SetAttachment(attachments);
        }

        public void ClearWeaponState()
        {
            _playerEntity.playerWeaponState.Accuracy = 0;
            _playerEntity.playerWeaponState.BurstShootCount = 0;
            _playerEntity.playerWeaponState.ContinuesShootCount = 0;
            _playerEntity.playerWeaponState.ContinuesShootDecreaseNeeded = false;
            _playerEntity.playerWeaponState.ContinuesShootDecreaseTimer = 0;
            _playerEntity.playerWeaponState.ContinuousAttackTime = 0;
            _playerEntity.playerWeaponState.LastBulletDir = UnityEngine.Vector3.zero;
            _playerEntity.playerWeaponState.LastFireTime = 0;
            _playerEntity.playerWeaponState.LastSpreadX = 0;
            _playerEntity.playerWeaponState.LastSpreadY = 0;
            _playerEntity.weaponLogic.Weapon.Reset();
        }

        public int GetAvatarById(int weaponId)
        {
            return _newWeaponConfigManager.GetAvatarByWeaponId(weaponId);
        }

        public bool HasWeapon(int weaponId)
        {
            if(weaponId == 0)
            {
                return false;
            }
            var weapon = _newWeaponConfigManager.GetConfigById(weaponId);
            if(null == weapon)
            {
                return false;
            }
            var slot =((EWeaponType)weapon.Type).ToWeaponSlot();
            var weaponComp = GetWeaponComponentBySlot(slot);
            if(null == weaponComp)
            {
                return false;
            }
            return weaponComp.Id == weaponId;
        }
    }
}