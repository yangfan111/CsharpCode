
using System;
using System.Reflection;
using App.Shared.Components.Bag;
using Core;
using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Core.GameModeLogic;
using Utils.Singleton;
using System.Collections.Generic;

namespace App.Shared
{



    public static class PlayerEntityWeaponExt
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
        public static GrenadeCacheDataComponent GetGrenadeCacheData(this PlayerEntity player)
        {
            return player.grenadeCacheData;
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
        /// 添加武器组件相关  - step1
        /// </summary>
        /// <param name="player"></param>
        public static void AttachWeaponComponents(this PlayerEntity player, IWeaponSlotsLibrary libary)
        {
            libary.AllocateWeaponComponents(player);
        }
        /// <summary>
        /// 添加武器组件相关 - step2
        /// </summary>
        /// <param name="player"></param>
        public static void AttachWeaponComponentLogic(this PlayerEntity player)
        {
            int cookie = player.entityKey.Value.EntityId;
            var newAgent = new PlayerWeaponComponentAgent(player.GetWeaponComponentBySlot, player.GetWeaponState);
            GameModuleLogicManagent.Allocate(cookie, (PlayerWeaponController controller) =>
              {
                  controller.SetAgent(newAgent);
                  var weaponMedium = new PlayerEntityWeaponMedium(controller, player);
                  controller.SetMedium(weaponMedium);
                  controller.SetListener(player.modeLogic.ModeLogic);
                  var helper = new GrenadeBagCacheHelper(player.GetGrenadeCacheData);
                  controller.SetBagCacheHelper(EWeaponSlotType.GrenadeWeapon, helper);
                  newAgent.SetController(controller);
              });
        }

        public static T GetController<T>(this PlayerEntity player) where T : ModuleLogicActivator<T>, new()
        {
            int cookie = player.entityKey.Value.EntityId;
            return GameModuleLogicManagent.Get<T>(cookie);
        }


    }




}


