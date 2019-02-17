using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using Core;
using Core.Utils;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="PlayerEntityWeaponBaseExt" />
    /// </summary>
    public static class PlayerEntityWeaponBaseExt
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityWeaponBaseExt));

        /// <summary>
        /// 添加武器组件控制器相关 
        /// </summary>
        /// <param name="player"></param>
        public static void AttachWeaponComponentBehavior(this PlayerEntity player, Contexts contexts, List<int> greandeIds)
        {
            int cookie = player.entityKey.Value.GetHashCode();
            GameModuleManagement.Dispose();
            GameModuleManagement.Allocate(cookie, (PlayerWeaponController controller) =>
              {

                  var playerWeaponAgent = new PlayerWeaponComponentsAgent(player.FindWeaponBagSetComponent,
                      player.FindOverrideBagComponent, player.FindWeaponAuxiliaryComponent);

                  var weaponInteract = new PlayerEntityWeaponInteract(controller, player);
                  controller.SetOwner(player.entityKey.Value);
                  controller.SetPlayerWeaponAgent(playerWeaponAgent);
                  controller.SetInteract(weaponInteract);
                  controller.SetWeaponContext(contexts.weapon);
                  controller.SetConfigManager(contexts.session.commonSession.PlayerWeaponConfigManager);
                  controller.SetProcessListener(player.modeLogic.ModeLogic);
                  var helper = new GrenadeBagCacheHelper(() => { return FindGrenadeCacheData(player); }, greandeIds);
                  controller.SetBagCacheHelper(EWeaponSlotType.ThrowingWeapon, helper);
                  playerWeaponAgent.SetController(controller);
                  controller.ResetAllComponents();
              });
        }

        public static void AttachGrenadeCacheData(this PlayerEntity player, List<int> ids)
        {
            var arrs = WeaponUtil.CreateEmptyGrenadeCacheArrs(ids);
            player.AddGrenadeCacheData(arrs);
        }

        public static T GetController<T>(this PlayerEntity player) where T : ModuleLogicActivator<T>, new()
        {
            return GameModuleManagement.Get<T>(player.entityKey.Value.EntityId);
        }

        public static PlayerWeaponController WeaponController(this PlayerEntity player)
        {
            return GameModuleManagement.Get<PlayerWeaponController>(player.entityKey.Value.EntityId);
        }

        public static ISharedPlayerWeaponComponentGetter WeaponAPI(this PlayerEntity player)
        {
            return GameModuleManagement.Get<PlayerWeaponController>(player.entityKey.Value.EntityId).Getter;
        }

        /// <summary>
        /// 玩家武器背包添加
        /// </summary>
        /// <param name="player"></param>
        public static void AttachPlayerWeaponBags(this PlayerEntity player, Contexts context)
        {
            var containers = WeaponUtil.CreateEmptyBagContainers();
            player.AddPlayerWeaponBagSet(containers);
        }

        public static void AttachPlayerAux(this PlayerEntity player)
        {
            player.AddPlayerWeaponAuxiliary(new List<PlayerBulletData>(), new List<EClientEffectType>());
        }

        /// <summary>
        /// 获取角色背包
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static PlayerWeaponBagSetComponent FindWeaponBagSetComponent(this PlayerEntity player)
        {
            return player.playerWeaponBagSet;
        }

        /// <summary>
        /// 获取后坐力
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static OverrideBagComponent FindOverrideBagComponent(this PlayerEntity player)
        {
            return player.overrideBag;
        }

        /// <summary>
        /// 获取本地缓存
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static PlayerWeaponAuxiliaryComponent FindWeaponAuxiliaryComponent(this PlayerEntity player)
        {
            return player.playerWeaponAuxiliary;
        }

        /// <summary>
        /// 获取手雷信息
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static GrenadeCacheDataComponent FindGrenadeCacheData(this PlayerEntity player)
        {
            return player.grenadeCacheData;
        }

        [System.Obsolete]
        public static void PlayWeaponSound(this PlayerEntity playerEntity, EWeaponSoundType sound)
        {
            if (playerEntity.hasWeaponSound)
            {
                playerEntity.weaponSound.PlayList.Add(sound);
            }
        }
    }
}
