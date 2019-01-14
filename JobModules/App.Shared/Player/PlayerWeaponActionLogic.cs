using Core.Bag;
using Core.Utils;
using App.Shared.Components;
using Core;
using Utils.Appearance;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.CharacterState;
using App.Shared.WeaponLogic;
using Core.GameModeLogic;
using System;
using Utils.Singleton;

namespace App.Shared.Player
{

    /// <summary>
    /// 进行一些需要在system之间共享的人物相关逻辑
    /// </summary>
    public partial class PlayerWeaponActionLogic : IPlayerWeaponActionLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponActionLogic));

        PlayerEntity _playerEntity;
        //武器背包数据操作
        WeaponBagLogic _weaponBagLogic;
        WeaponSlotAuxiliary _slotAuxiliary;
        IWeaponSlotController _weaponSlotController;
        IWeaponActionListener _weaponActionListener;
        public PlayerWeaponActionLogic(
            PlayerEntity player,
            WeaponBagLogic bagLogic,
            ISceneObjectEntityFactory sceneObjectEntityFactory,
            IReservedBulletLogic reservedBulletLogic,
            IGrenadeBagCacheAgent grenadeInventory,
            IWeaponSlotController weaponSlotController,
            IWeaponActionListener weaponActionListener)
        {
            _playerEntity = player;
            _weaponBagLogic = bagLogic;
            _weaponSlotController = weaponSlotController;
            _slotAuxiliary = new WeaponSlotAuxiliary(player,
                weaponSlotController,
                this,
                reservedBulletLogic,
                grenadeInventory);
            _weaponActionListener = weaponActionListener;
        }

        /// <summary>
        /// 相比UseWeapon多了动作,需经由UserCmd触发
        /// </summary>
        /// <param name="slot"></param>
        public void DrawWeapon(EWeaponSlotType slot)
        {
            Logger.DebugFormat("DrawWeapon {0}", slot);
            if (IsCurrentSlot(slot))
            {
                return;
            }
            var bag = _weaponBagLogic;
            var lastWeapon = bag.GetCurrentWeaponInfo();
            var weapon = bag.GetWeaponInfo(slot);
            if (weapon.Id < 1)
            {
                return;
            }
            var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weapon.Id);
            if (null == weaponCfg)
            {
                return;
            }

            bool armOnLeft = slot == EWeaponSlotType.PrimeWeapon2;
            float holsterParam = (IsCurrentSlot(EWeaponSlotType.PrimeWeapon2)) ?
                                    AnimatorParametersHash.Instance.HolsterFromLeftValue :
                                    AnimatorParametersHash.Instance.HolsterFromRightValue;
            float drawParam = armOnLeft ?
                                    AnimatorParametersHash.Instance.DrawLeftValue :
                                    AnimatorParametersHash.Instance.DrawRightValue;
            float switchParam = holsterParam * 10 + drawParam;

            if (bag.GetCurrentWeaponSlot() == EWeaponSlotType.PrimeWeapon2)
                _playerEntity.appearanceInterface.Appearance.MountP3WeaponOnAlternativeLocator();

            DoDrawInterrupt();
            if (lastWeapon.Id > 0)
            {
                _playerEntity.stateInterface.State.SwitchWeapon(
                       () =>
                       {
                           WeaponToHand(weapon.Id, lastWeapon.Id, slot, armOnLeft);
                       },
                       () =>
                       {
                           OnMountFinish(_weaponBagLogic, weapon, slot);
                           _playerEntity.appearanceInterface.Appearance.RemountP3WeaponOnRightHand();
                       },
                       switchParam
                       );
            }
            else
            {
                WeaponToHand(weapon.Id, lastWeapon.Id, slot, armOnLeft);
                OnMountFinish(bag, weapon, slot);
                _playerEntity.stateInterface.State.Draw(() =>
                {
                    _playerEntity.appearanceInterface.Appearance.RemountP3WeaponOnRightHand();
                }, drawParam);
            }
        }

  

        private void DoDrawInterrupt()
        {
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, _playerEntity.gamePlay);
            _playerEntity.stateInterface.State.InterruptAction();
            _playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
        }

        public void TryMountWeapon(EWeaponSlotType slot)
        {
            WeaponInfo currSlotWeapon = _weaponBagLogic.GetCurrentWeaponInfo();
            WeaponInfo weaponInfo;
            if (_weaponBagLogic.TryGetWeaponSlotInfo(slot, out weaponInfo))
            {
                WeaponToHand(weaponInfo.Id, currSlotWeapon.Id, slot);
                OnMountFinish(_weaponBagLogic, weaponInfo, slot);
            }
      
        }

        public void UnmountWeapon(Action onfinish)
        {
            var weaponInfo = _weaponBagLogic.GetCurrentWeaponInfo();

            if (_weaponBagLogic.GetCurrentWeaponSlot() == EWeaponSlotType.PrimeWeapon2)
                _playerEntity.appearanceInterface.Appearance.MountP3WeaponOnAlternativeLocator();

            float holsterParam = (_weaponBagLogic.GetCurrentWeaponSlot() == EWeaponSlotType.PrimeWeapon2) ?
                AnimatorParametersHash.Instance.HolsterFromLeftValue :
                AnimatorParametersHash.Instance.HolsterFromRightValue;

            _playerEntity.stateInterface.State.InterruptAction();
            _playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
            _playerEntity.stateInterface.State.Unarm(() =>
            {
                _playerEntity.appearanceInterface.Appearance.UnmountWeaponFromHand();
                _weaponBagLogic.SetCurrentWeapon(EWeaponSlotType.None);
                if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponInfo.Id))
                {
                    _playerEntity.UnmoutC4();
                }
                if (null != onfinish)
                {
                    onfinish();
                }
            },
            holsterParam);
        }

        public void UnmountWeapon()//float holsterParam)
        {
            UnmountWeapon(null);
        }

        public void ForceUnmountWeapon()
        {
            _playerEntity.appearanceInterface.Appearance.UnmountWeaponFromHand();
            _weaponBagLogic.SetCurrentWeapon(EWeaponSlotType.None);

            if (_playerEntity.hasThrowingAction)
            {
                if (_playerEntity.throwingAction.ActionInfo.IsReady && _playerEntity.throwingAction.ActionInfo.IsPull)
                {
                    //若已拉栓，销毁ThrowingEntity
                    _playerEntity.throwingAction.ActionInfo.IsInterrupt = true;
                }
                //打断投掷动作
                _playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
                //清理手雷状态
                _playerEntity.throwingAction.ActionInfo.ClearState();
            }
        }

        private bool IsCurrentSlot(EWeaponSlotType slot)
        {
            var curSlot = _weaponBagLogic.GetCurrentWeaponSlot();
            return curSlot == slot;
        }

        private void WeaponToHand(int weaponId, int lastWeaponId, EWeaponSlotType slot, bool armOnLeft = false)
        {
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeaponId))
            {
                _playerEntity.UnmoutC4();
            }
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                _playerEntity.MountC4(weaponId);
            }
            var pos = slot.ToWeaponInPackage();
            _playerEntity.appearanceInterface.Appearance.MountWeaponToHand(pos);
            if (armOnLeft)
                _playerEntity.appearanceInterface.Appearance.MountP3WeaponOnAlternativeLocator();
        }

        private void OnMountFinish(IWeaponBagLogic bag, WeaponInfo weapon, EWeaponSlotType slot)
        {
            _weaponBagLogic.SetCurrentWeapon(slot);
            if (weapon.Bullet <= 0)
            {
                if (SharedConfig.CurrentGameMode == GameMode.Normal)
                {
                    //TODO 判断弹药数量是否足够，如果弹药不足，弹提示框
                    _playerEntity.stateInterface.State.ReloadEmpty(() =>
                    {

                    });
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

        public void DropWeapon(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None || slot == EWeaponSlotType.GrenadeWeapon)
            {
                Logger.Error("drop weapon in slot none");
                return;
            }
            var lastWeapon = _weaponBagLogic.GetWeaponInfo(slot);
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(lastWeapon.Id))
            {
                _playerEntity.RemoveC4();
            }
            Logger.DebugFormat("DropWeapon {0}", slot);
            _weaponActionListener.OnDrop(_playerEntity, slot);
            _slotAuxiliary.OnWpDrop(slot);

        }
   


        public void RemoveWeapon(EWeaponSlotType slot)
        {
            if (_weaponBagLogic.RemoveWeapon(slot))
            {
                OnInterrupt();
            }
        }

        public void RemoveWeaponNoInterrupt(EWeaponSlotType slot)
        {
            _weaponBagLogic.RemoveWeapon(slot);
        }




        public int GetWeaponBulletCount(EWeaponSlotType slot)
        {
            return _weaponBagLogic.GetReservedBullet(slot);
        }

        public void SetWeaponBulletCount(EWeaponSlotType slot, int count)
        {
            _weaponBagLogic.SetReservedBullet(slot, count);
        }

        public bool AutoPickUpWeapon(WeaponInfo weaponInfo)
        {
            Logger.DebugFormat("AutoPickUpWeapon {0}", weaponInfo.Id);
            var weapon = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponInfo.Id);
            if (null != weapon)
            {
                var weaponType = SingletonManager.Get<WeaponConfigManager>().GetWeaponType(weaponInfo.Id);
                if (!weaponType.CanAutoPick())
                {
                    return false;
                }
                var slotType = GetMatchSlot((EWeaponType)weapon.Type);
                if (slotType != EWeaponSlotType.None)
                {
                    if (_weaponBagLogic.HasWeaponInSlot(slotType))
                    {
                        return false;
                    }
                    var noWeaponInHand = _weaponBagLogic.GetCurrentWeaponSlot() == EWeaponSlotType.None;
                    _weaponActionListener.OnPickup(_playerEntity, slotType);
                    var last = ReplaceWeaponToSlot(slotType, weaponInfo, false);
                    //var last = _weaponBagLogic.AddWeaponToSlot(slotType, weaponInfo);
                    if (noWeaponInHand)
                    {
                        TryMountWeapon(slotType);
                    }
                    return true;
                }
                else
                {
                    Logger.Error("Illegal Slot type for weapon ");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private EWeaponSlotType GetMatchSlot(EWeaponType weaponType)
        {
            switch (weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    var hasPrime = _weaponBagLogic.HasWeaponInSlot(EWeaponSlotType.PrimeWeapon1);
                    if (hasPrime && _weaponSlotController.IsSlotValid(EWeaponSlotType.PrimeWeapon2))
                    {
                        return EWeaponSlotType.PrimeWeapon2;
                    }
                    else
                    {
                        return EWeaponSlotType.PrimeWeapon1;
                    }
                default:
                    return weaponType.ToWeaponSlot();
            }
        }

        public void OnInterrupt()
        {
            _playerEntity.stateInterface.State.InterruptAction();
            _playerEntity.stateInterface.State.InterruptSwitchWeapon();
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, _playerEntity.gamePlay);
            _playerEntity.stateInterface.State.ForceBreakSpecialReload(null);
            _playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
            if (_playerEntity.hasThrowingAction)
            {
                _playerEntity.throwingAction.ActionInfo.ClearState();
            }
        }
    }
}
