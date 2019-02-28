using App.Shared.GameModules.Weapon;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.EntityComponent;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModeLogic.WeaponInitLoigc
{
    /// <summary>
    /// Defines the <see cref="ServerWeaponInitHandler" />
    /// </summary>
    public class ServerWeaponInitHandler : IWeaponInitHandler
    {
        /// <summary>
        /// Defines the <see cref="OverrideBagTaticsCacheData" />
        /// </summary>
        private class OverrideBagTaticsCacheData
        {
            private readonly PlayerWeaponBagData _playerWeaponBagData = new PlayerWeaponBagData();

            public PlayerWeaponBagData CombineOverridedBagData(ISharedPlayerWeaponGetter getter, PlayerWeaponBagData playerWeaponBagData)
            {
                playerWeaponBagData.CopyTo(_playerWeaponBagData);
                if (getter.OverrideBagTactic < 1)
                {
                    return _playerWeaponBagData;
                }
                bool replace = false;
                foreach (var weapon in playerWeaponBagData.weaponList)
                {
                    var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                    if (slot == EWeaponSlotType.TacticWeapon)
                    {
                        weapon.WeaponTplId = getter.OverrideBagTactic;
                        replace = true;
                    }
                }
                if (!replace)
                {
                    _playerWeaponBagData.weaponList.Add(new PlayerWeaponData
                    {
                        Index = PlayerWeaponBagData.Slot2Index(EWeaponSlotType.TacticWeapon),
                        WeaponTplId = getter.OverrideBagTactic,
                    });
                }
                return _playerWeaponBagData;
            }
        }

        private readonly OverrideBagTaticsCacheData BagTaticsCache = new OverrideBagTaticsCacheData();

        public int ModeId { get; private set; }


        public ServerWeaponInitHandler(
            int modeId)
        {
            ModeId = modeId;
        }

        public bool CanModeSwitchBag
        {
            get { return SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(ModeId) != XmlConfig.EBagType.Chicken; }
        }

        private EntityKey CreateCustomizeWeapon(EntityKey lastKey, EntityKey owner)
        {
            if (lastKey.IsVailed())
            {
                WeaponEntity currWeapon = WeaponEntityFactory.GetWeaponEntity(lastKey);
                if (currWeapon != null)
                    WeaponEntityFactory.RemoveWeaponEntity(currWeapon);
            }
            WeaponEntity newWeapon = WeaponEntityFactory.CreateEntity(null);
            newWeapon.SetFlagOwner(owner);
            newWeapon.isFlagSyncSelf = true;
            return newWeapon.entityKey.Value;

        }
        private void RebindCustomizeWeapon(PlayerEntity player)
        {
            var customizeCmp = player.AttachPlayerCustomize();
            customizeCmp.EmptyConstWeaponkey = CreateCustomizeWeapon(customizeCmp.EmptyConstWeaponkey, player.entityKey.Value);
            customizeCmp.GrenadeConstWeaponKey = CreateCustomizeWeapon(customizeCmp.GrenadeConstWeaponKey, player.entityKey.Value);
            var bagSetCmp = player.playerWeaponBagSet;
            for (int i = 0; i < bagSetCmp.WeaponBags.Count; i++)
            {
                bagSetCmp[i].BindCustomizeWeaponKey(customizeCmp);
            }
        }
        private void DropOrTrashOldWeapons(PlayerEntity player)
        {
            var controller = player.WeaponController();
            var bagSetCmp = player.playerWeaponBagSet;
            for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
            {
                for (EWeaponSlotType j = EWeaponSlotType.None + 1; j < EWeaponSlotType.Length; j++)
                {
                    if (controller.HeldBagPointer != i)
                        controller.DestroyWeapon(j, i);
                    else
                    {
                        controller.DestroyWeapon(j, i);
                        //EntityKey? lastKey = controller.DropWeapon(j, false, i);
                        //if(lastKey.HasValue)
                        //{
                        //    //TODO:生成掉落的武器

                        //}
                    }
                }
            }
        }

        public void PreparePlayerWeapon(PlayerEntity player)
        {
            //丢弃武器数据
            DropOrTrashOldWeapons(player);

            //重新绑定自定义武器
            RebindCustomizeWeapon(player);
            //重新初始化武器数据
            GenerateInitialWeapons(player);
        }
        public void InitBeforeAllOnce(PlayerEntity player)
        {

        }

        public void Recovery(IPlayerWeaponGetter controller, int index)
        {
            RecoveryBagContainer(index, controller);
           // TrashBagState(controller);
        }

        private void GenerateInitialWeapons(PlayerEntity player)
        {

            var controller = player.WeaponController();
            PlayerWeaponBagData[] bags = controller.RelatedPlayerInfo.WeaponBags;
            List<int> valuableBagIndices = new List<int>();
            int defaultBagIndex = 0;
            for (int i = 0; i < bags.Length; i++)
            {

                if (bags[i] == null || bags[i].weaponList.Count < 1 ||
                    bags[i].BagIndex >= GameGlobalConst.WeaponBagMaxCount)
                    continue;
                valuableBagIndices.Add(i);
                defaultBagIndex = Math.Min(defaultBagIndex, bags[i].BagIndex);
            }

            controller.InitBag(defaultBagIndex, SingletonManager.Get<GameModeConfigManager>().GetBagLimitTime(ModeId));
            if (valuableBagIndices.Count == 0)
                return;
            foreach (int index in valuableBagIndices)
            {
                GenerateInitialWeapons(bags[index], controller, index == defaultBagIndex);

            }

        }

        private void GenerateInitialWeapons(PlayerWeaponBagData srcBagData, PlayerWeaponController controller, bool isDefaultBag)
        {


            PlayerWeaponBagData bagData = BagTaticsCache.CombineOverridedBagData(controller, srcBagData);
            var helper = controller.GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon);
            helper.ClearCache();
            var firstSlot = EWeaponSlotType.Length;
#if UNITY_EDITOR
            var s = "";
            foreach (var weapon in bagData.weaponList)
            {
                s += weapon.ToString() + "\n";
            }
            DebugUtil.LogInUnity(s, DebugUtil.DebugColor.Green);
#endif
            foreach (var weapon in bagData.weaponList)
            {
                DebugUtil.LogInUnity("|In:" + weapon.ToString(), DebugUtil.DebugColor.Blue);

                var slot = PlayerWeaponBagData.Index2Slot(weapon.Index);
                if (slot < firstSlot)
                {
                    firstSlot = slot;
                }
                var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.WeaponTplId);
                var weaponType = (EWeaponType_Config)weaponAllConfig.WeaponResCfg().Type;
                var weaponId = weapon.WeaponTplId;
                var orient = WeaponUtil.CreateScan(weapon);

                if (weaponType == EWeaponType_Config.ThrowWeapon)
                {
                    controller.GetBagCacheHelper(EWeaponSlotType.ThrowingWeapon).AddCache(weaponId);

                    //if (!grenadeMounted)
                    //{
                    //    controller.PickUpWeapon(slot, false);
                    //    grenadeMounted = true;
                    //}
                    //helper.AddCache(weaponInfo.ConfigId);
                }
                else
                {
                    orient.Bullet = weaponAllConfig.WeaponPropertyCfg().Bullet;
                    orient.ReservedBullet = weaponAllConfig.WeaponPropertyCfg().Bulletmax;

                    if (orient.Magazine > 0)
                    {
                        orient.Bullet += weaponAllConfig.GetWeaponPartCfg(orient.Magazine).Bullet;
                    }
                    controller.ReplaceWeaponToSlot(slot, srcBagData.BagIndex, orient);
                    //if(isdefaultBag)
                    //    controller.PickUpWeapon(weaponInfo);
                    //else
                    //    controller.ReplaceWeaponToSlot(slot)
                }
            }
            if (isDefaultBag)
            {
            if (firstSlot == EWeaponSlotType.ThrowingWeapon)
                controller.ArmGreande();
            else
                controller.TryArmWeapon(firstSlot);
            //DebugUtil.LogInUnity(controller.ToString());
            }
        }

        public void TrashBagState(PlayerEntity player)
        {
            player.WeaponController().ResetBagLockState(SingletonManager.Get<GameModeConfigManager>().GetBagLimitTime(ModeId));
        }

        public void ResetPlayerWeaponData(IPlayerWeaponGetter controller)
        {
            throw new NotImplementedException();
        }

        public void RecoveryBagContainer(int index, IPlayerWeaponGetter controller)
        {
            throw new NotImplementedException();
        }

        public void TrashPlayerBagState(IPlayerWeaponGetter controller)
        {
            throw new NotImplementedException();
        }
    }
}
