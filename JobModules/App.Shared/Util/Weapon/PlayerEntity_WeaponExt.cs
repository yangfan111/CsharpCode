
using System;
using System.Reflection;
using App.Shared.Components.Bag;
using Core.Bag;
using App.Shared.WeaponLogic;
namespace App.Shared.Util
{



    public static class PlayerEntity_WeaponExt
    {

        public static WeaponComponent GetWeaponComponentBySlot(this PlayerEntity player, EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon1:
                    return player.hasPrimeWeapon ? player.primeWeapon : null;
                case EWeaponSlotType.PrimeWeapon2:
                    return player.hasSubWeapon ? player.subWeapon : null;
                case EWeaponSlotType.SubWeapon:
                    return player.hasPistol ? player.pistol : null;
                case EWeaponSlotType.MeleeWeapon:
                    return player.hasMelee ? player.melee : null;
                case EWeaponSlotType.GrenadeWeapon:
                    return player.hasGrenade ? player.grenade : null;

                case EWeaponSlotType.TacticWeapon:
                    return player.hasTacticWeapon ? player.tacticWeapon : null;
            }
            return null;
        }
        public static WeaponStateComponent GetWeaponState(this PlayerEntity player, bool getAutoState)
        {
            return player.hasWeaponState ? player.weaponState : null;
        }
        public static GrenadeInventoryDataComponent GetGrenadeCacheData(this PlayerEntity player)
        {
            return player.grenadeInventoryData;
        }
        public static void ClearPlayerWeaponState(this PlayerEntity player)
        {
            var wpState = player.playerWeaponState;
            wpState.Accuracy = 0;
            wpState.BurstShootCount = 0;
            wpState.ContinuesShootCount = 0;
            wpState.ContinuesShootDecreaseNeeded = false;
            wpState.ContinuesShootDecreaseTimer = 0;
            wpState.ContinuousAttackTime = 0;
            wpState.LastBulletDir = UnityEngine.Vector3.zero;
            wpState.LastFireTime = 0;
            wpState.LastSpreadX = 0;
            wpState.LastSpreadY = 0;
            player.weaponLogic.Weapon.Reset();

        }
        /// <summary>
        /// 添加武器组件代理相关 - step2
        /// </summary>
        /// <param name="player"></param>
        public static void AttachWeaponComponentAgent(this PlayerEntity player)
        {
            PlayerWeaponComponentAgent newAgent = new PlayerWeaponComponentAgent(
                          player.GetWeaponComponentBySlot, player.GetWeaponState);

            var controller = newAgent.GetController() as PlayerWeaponController;

            var weaponFacade = new WeaponEntityFacade(controller, player);
            controller.AttachFacade(weaponFacade);
            player.AddAgent(newAgent);

            var helper = new GrenadeBagCacheHelper(player.GetGrenadeCacheData);
            //=>TODO:player.(grenadeInventory);
        }
        /// <summary>
        /// 添加武器组件相关  - step1
        /// </summary>
        /// <param name="player"></param>
        public static void AttachWeaponComponents(this PlayerEntity player)
        {
            for (int i = 0; i < WeaponUtil.AvaliableSlotIndices.Length; i++)
            {
                Action<PlayerEntity> addWeaponComponent;
                if (WeaponUtil.AllAddWeaponComponentDic.TryGetValue(WeaponUtil.AvaliableSlotIndices[i], out addWeaponComponent))
                {
                    addWeaponComponent(player);
                }

            }
        }
        public static IPlayerWeaponComponentArchive GetWeaponAchive(this PlayerEntity player)
        {
            return player.weaponAgent.Content;
        }
        public static PlayerWeaponController GetWeaponController(this PlayerEntity player)
        {
            return player.weaponAgent.Content.GetController() as PlayerWeaponController;

        }

    }




}


