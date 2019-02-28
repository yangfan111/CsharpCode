using App.Shared.Components.Bag;
using Core;
using App.Shared.GameModules.Weapon;
using Core.GameModeLogic;
using App.Shared.WeaponLogic;
using Core.Utils;

namespace App.Shared
{
    public static class PlayerEntityWeaponExt
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityWeaponExt));

        public static WeaponStateComponent GetWeaponState(this PlayerEntity player, bool getAutoState)
        {
            return player.hasWeaponState ? player.weaponState : null;
        }
        public static GrenadeCacheDataComponent GetGrenadeCacheData(this PlayerEntity player)
        {
            return player.grenadeCacheData;
        }

        public static void ClearPlayerWeaponState(this PlayerEntity player, Contexts contexts)
        {
            var wpState = player.GetWeaponRunTimeInfo(contexts);
            if(null == wpState)
            {
                Logger.Error("current weapon data is null");
                return;
            }
            wpState.Accuracy = 0;
            wpState.BurstShootCount = 0;
            wpState.ContinuesShootCount = 0;
            wpState.ContinuesShootDecreaseNeeded = false;
            wpState.ContinuesShootDecreaseTimer = 0;
            wpState.ContinueAttackEndStamp = 0;
            wpState.LastBulletDir = UnityEngine.Vector3.zero;
            wpState.LastFireTime = 0;
            wpState.LastSpreadX = 0;
            wpState.LastSpreadY = 0;

        }

        /// <summary>
        /// 添加武器组件控制器相关 - step2
        /// </summary>
        /// <param name="player"></param>
        public static void AttachWeaponComponentLogic(this PlayerEntity player)
        {
            int cookie = player.entityKey.Value.GetHashCode();
            var newAgent = new PlayerWeaponComponentAgent(player, player.GetWeaponData, player.GetBagState);
            GameModuleLogicManagement.Allocate(cookie, (PlayerWeaponController controller) =>
              {
                  controller.SetAgent(newAgent);
                  var weaponMedium = new PlayerEntityWeaponMedium(controller, player);
                  controller.SetMedium(weaponMedium);
                  controller.SetListener(player.modeLogic.ModeLogic);
                  var helper = new GrenadeBagCacheHelper(player.GetGrenadeCacheData);
                  controller.SetBagCacheHelper(EWeaponSlotType.ThrowingWeapon, helper);
                  newAgent.SetController(controller);
              });
        }

        public static T GetController<T>(this PlayerEntity player) where T : ModuleLogicActivator<T>, new()
        {
            int cookie = player.entityKey.Value.GetHashCode();
            return GameModuleLogicManagement.Get<T>(cookie);
        }
        public static ISharedPlayerWeaponComponentGetter WeaponAPI (this PlayerEntity player)
        {
            int cookie = player.entityKey.Value.GetHashCode();
            return GameModuleLogicManagement.Get<PlayerWeaponController>(cookie).Getter;
        }

    }




}


