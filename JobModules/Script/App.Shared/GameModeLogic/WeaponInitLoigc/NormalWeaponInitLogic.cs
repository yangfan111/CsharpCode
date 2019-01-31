using App.Shared.GameModules.Weapon;
using App.Shared.WeaponLogic;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.GameModeLogic;
using Core.Room;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using Utils.Configuration;

namespace App.Shared.GameModeLogic.WeaponInitLoigc
{
    public class NormalWeaponInitLogic : IWeaponInitLogic
    {
        private const int DefaultBagIndex = 0;
        private LinkedList<EWeaponSlotType> _removeSlotList = new LinkedList<EWeaponSlotType>();
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NormalWeaponInitLogic));
        private readonly int _gameModeId;
        private IWeaponConfigManager _weaponConfigManager;
        private IWeaponPropertyConfigManager _weaponPropertyConfigManager;
        private IGameModeConfigManager _gameModeConfigManager;
        private IWeaponPartsConfigManager _weaponPartsConfigManager;
        private OverrideWeaponController _overrideWeaponController = new OverrideWeaponController();
        private Contexts _contexts;

        public NormalWeaponInitLogic(
            Contexts contexts,
            int modeId, 
            IGameModeConfigManager gameModeConfigManager,
            IWeaponConfigManager newWeaponConfigManager,
            IWeaponPropertyConfigManager weaponPropertyConfigManager,
            IWeaponPartsConfigManager weaponPartsConfigManager)
        {
            Logger.Info("init NormalWeaponInitLogic ");
            _contexts = contexts;
            _gameModeId = modeId;
            _weaponConfigManager = newWeaponConfigManager;
            _weaponPropertyConfigManager = weaponPropertyConfigManager;
            _gameModeConfigManager = gameModeConfigManager;
            _weaponPartsConfigManager = weaponPartsConfigManager;
        }

        public bool IsBagSwithEnabled(Entity playerEntity)
        {
            var player = playerEntity as PlayerEntity;
            if(null == player)
            {
                Logger.Error("PlayerEntity is null");
                return false;
            }
            return player.gamePlay.IsLifeState(Components.Player.EPlayerLifeState.Alive) && !player.weaponState.BagLocked && player.weaponState.BagOpenLimitTime > player.time.ClientTime;
        }

        public void InitDefaultWeapon(Entity playerEntity)
        {
            Logger.Info("InitDefaultWeapon");
            var player = playerEntity as PlayerEntity;
            if(null == player)
            {
                Logger.Error("PlayerEntity is null");
                return;
            }
            ResetBagState(player);
            var bags = player.playerInfo.WeaponBags;
            var defaultBag = GetDefaultBag(bags);
            if (null != defaultBag)
            {
                MountWeaponAndFillBullet(player, defaultBag);
            }
            else
            {
                Logger.Error("all bag is empty");
            }
        }

        private PlayerWeaponBagData GetDefaultBag(PlayerWeaponBagData[] bags)
        {
            foreach(var bag in bags)
            {
                if(null == bag)
                {
                    continue;
                }
                if(bag.weaponList.Count < 1)
                {
                    continue;
                }
                foreach(var weapon in bag.weaponList)
                {
                    if(weapon.WeaponTplId > 0)
                    {
                        return bag;
                    }
                }
            }
            return null;
        }

        private void ResetBagState(PlayerEntity player)
        {
            if(null == player)
            {
                Logger.Error("PlayerEntity is null");
                return;
            }
            player.weaponState.BagLocked = false;
            player.weaponState.BagOpenLimitTime = player.time.ClientTime + _gameModeConfigManager.GetBagLimitTime(_gameModeId);
        }

        public void ResetWeaponWithBagIndex(int index, Entity playerEntity)
        {
            var player = playerEntity as PlayerEntity;
            if(null == player)
            {
                Logger.Error("PlayerEntity is null");
                return;
            }
            var bags = player.playerInfo.WeaponBags;
            if(index > -1 && index < bags.Length)
            {
                var bag = bags[index];
                if(null == bag)
                {
                    return;
                }
                MountWeaponAndFillBullet(player, bag);
            }
        }

        private void MountWeaponAndFillBullet(PlayerEntity playerEntity, PlayerWeaponBagData srcBagData)
        {
            if(null == playerEntity)
            {
                Logger.Error("PlayerEntity is null");
                return;
            }
            var bagData = _overrideWeaponController.GetOverridedBagData(playerEntity, srcBagData);
            _removeSlotList.Clear();
            for(var slot = EWeaponSlotType.None + 1; slot < EWeaponSlotType.Length; slot++)
            {
                _removeSlotList.AddLast(slot);
            }
            var helper = playerEntity.GetController<PlayerWeaponController>().GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
            helper.ClearCache();
            var firstSlot = EWeaponSlotType.Length;
            bool grenadeMounted = false;
            if(null != bagData)
            {
                foreach(var weapon in bagData.weaponList)
                {
                    var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                    _removeSlotList.Remove(slot);
                    if(slot < firstSlot)
                    {
                        firstSlot = slot; 
                    }
                    var weaponType = _weaponConfigManager.GetWeaponType(weapon.WeaponTplId);
                    var weaponId = weapon.WeaponTplId;
                    var weaponInfo = weapon.ToWeaponInfo();

                    if (weaponType == EWeaponType.ThrowWeapon)
                    {
                        if (!grenadeMounted)
                        {
                            playerEntity.GetController<PlayerWeaponController>().ReplaceWeaponToSlot(_contexts, slot, weaponInfo);
                            grenadeMounted = true;
                        }
                        helper.AddCache(weaponInfo.Id);
                    }
                    else
                    {
                        weaponInfo.Bullet = _weaponPropertyConfigManager.GetBullet(weaponId);
                        weaponInfo.ReservedBullet = _weaponPropertyConfigManager.GetBulletMax(weaponId);
                        if(weaponInfo.Magazine > 0)
                        {
                            var magazineCfg = _weaponPartsConfigManager.GetConfigById(weaponInfo.Magazine);
                            weaponInfo.Bullet += magazineCfg.Bullet;
                        }
                        playerEntity.GetController<PlayerWeaponController>().ReplaceWeaponToSlot(_contexts, slot, weaponInfo);
                    }
                }
                playerEntity.weaponState.BagIndex = bagData.BagIndex;
            }
            if(firstSlot < EWeaponSlotType.Length)
            {
                playerEntity.GetController<PlayerWeaponController>().TryMountSlotWeapon(_contexts, firstSlot);
            }
            foreach(var slot in _removeSlotList)
            {
                if(playerEntity.HasWeaponInSlot(_contexts, slot))
                {
                    playerEntity.GetController<PlayerWeaponController>().RemoveSlotWeapon(_contexts, slot);
                }
            }
        }

        public void InitDefaultWeapon(Entity playerEntity, int index)
        {
            var player= playerEntity as PlayerEntity;
            if(null == playerEntity)
            {
                Logger.Error("PlayerEntity is null");
                return;
            }
            ResetBagState(player);
            ResetWeaponWithBagIndex(index, player);
        }
    }
}
