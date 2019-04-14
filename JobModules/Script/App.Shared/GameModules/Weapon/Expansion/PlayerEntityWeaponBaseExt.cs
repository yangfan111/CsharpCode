using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using Assets.App.Shared.EntityFactory;
using Core;
using Core.EntityComponent;
using Core.Utils;
using System.Collections.Generic;
using App.Shared.Components.Weapon;
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
            
            //    GameModuleManagement.Dispose();
            GameModuleManagement.ForceAllocate(player.entityKey.Value.EntityId, (PlayerWeaponController controller) =>
              {
                  var greandeHelper     = new GrenadeCacheHandler(() => { return FindGrenadeCacheData(player); }, () => { return FindGrenadeWeaponEntity(player); }, greandeIds, contexts.session.commonSession.FreeArgs);
                  controller.Initialize(player, greandeHelper);
                  
                  //controller.SetWeaponContext(contexts.weapon);

                  // controller.SetConfigManager(contexts.session.commonSession.PlayerWeaponResourceConfigManager);
                  controller.SetProcessListener(player.ModeController().ProcessListener)
                  ;
                
                  controller.ResetAllComponents();
//#if UNITY_EDITOR
//                  if (!player.hasPlayerDebug)
//                      player.AddPlayerDebug(controller.Bag1DebugInfo,controller.Bag2DebugInfo,controller.Bag3DebugInfo,controller.Bag4DebugInfo);
//#endif

              });


        }

        public static void AttachGrenadeCacheData(this PlayerEntity player, List<int> ids)
        {
            if (!player.hasGrenadeCacheData)
            {
                var arrs = WeaponUtil.CreateEmptyGrenadeCacheArrs(ids);
                player.AddGrenadeCacheData(arrs);
            }


        }

        public static PlayerWeaponController WeaponController(this PlayerEntity player)
        {
            return GameModuleManagement.Get<PlayerWeaponController>(player.entityKey.Value.EntityId);
        }
        public static IPlayerWeaponSharedGetter WeaponGetter(this PlayerEntity player)
        {
            return GameModuleManagement.Get<PlayerWeaponController>(player.entityKey.Value.EntityId);
        }

        /// <summary>
        /// 玩家武器背包添加
        /// </summary>
        /// <param name="player"></param>
        public static void AttachPlayerWeaponBags(this PlayerEntity player)
        {
            if (!player.hasPlayerWeaponBagSet)
            {
                var modeController = player.ModeController();
                player.AddPlayerWeaponBagSet(WeaponUtil.CreateEmptyBagContainers());

            }
          
        }
        public static void AttachPlayerAmmu(this PlayerEntity player)
        {
            if (!player.hasPlayerWeaponAmmunition)
            {
                player.AddPlayerWeaponAmmunition();
            }
            else
            {
                player.ReplacePlayerWeaponAmmunition();
            }
        }

        public static void AttachPlayerAux(this PlayerEntity player)
        {
            if (!player.hasPlayerWeaponAuxiliary)
            {
                player.AddPlayerWeaponAuxiliary();

            }
            else
            {
                player.ReplacePlayerWeaponAuxiliary();
            }
            player.playerWeaponAuxiliary.BulletList = new List<PlayerBulletData>();

        }
        /// <summary>
        /// 在背包初始化之后进行
        /// </summary>
        /// <param name="player"></param>
        public static void AttachPlayerCustomize(this PlayerEntity player)
        {
            var emptyKey = CreateCustomizeWeapon(EntityKey.Default, player.entityKey.Value, WeaponUtil.EmptyHandId);
            var grenenadeKey = CreateCustomizeWeapon(EntityKey.Default, player.entityKey.Value, 0);
            if (!player.hasPlayerWeaponCustomize)
            {
                player.AddPlayerWeaponCustomize(grenenadeKey, emptyKey);
            }
            else
            {
                player.ReplacePlayerWeaponCustomize(grenenadeKey, emptyKey);
            }
            var bagSetCmp = player.playerWeaponBagSet;
            for (int i = 0; i < bagSetCmp.WeaponBags.Count; i++)
            {
                bagSetCmp[i].BindCustomizeWeaponKey(player.playerWeaponCustomize.EmptyConstWeaponkey);
            }
       
        }
        public static EntityKey CreateCustomizeWeapon(EntityKey lastKey, EntityKey owner, int configId)
        {
            if (lastKey.IsValid())
            {
                WeaponEntity currWeapon = WeaponEntityFactory.GetWeaponEntity(lastKey);
                if (currWeapon != null)
                    WeaponEntityFactory.RemoveWeaponEntity(currWeapon);
            }
            WeaponEntity newWeapon = WeaponEntityFactory.CreateEntity(WeaponUtil.CreateScan(configId));
            newWeapon.SetRetain(owner);
            return newWeapon.entityKey.Value;
        }
        /// <summary>
        /// 获取角色背包
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static PlayerWeaponBagSetComponent FindBagSetComponent(this PlayerEntity player)
        {
          
            return player.playerWeaponBagSet;
        }


        /// <summary>
        /// 获取手雷信息
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static GrenadeCacheDataComponent FindGrenadeCacheData(this PlayerEntity player)
        {
            if (!player.hasGrenadeCacheData)
            {
                var greandeIds = WeaponUtil.ForeachFilterGreandeIds();
                var arrs = WeaponUtil.CreateEmptyGrenadeCacheArrs(greandeIds);
                player.AddGrenadeCacheData(arrs);

            }
            return player.grenadeCacheData;
        }

        private static WeaponEntity FindGrenadeWeaponEntity(this PlayerEntity player)
        {
            return WeaponEntityFactory.GetWeaponEntity(player.playerWeaponCustomize.GrenadeConstWeaponKey);
        }
        private static WeaponEntity FindEmptyWeaponEntity(this PlayerEntity player)
        {
            return WeaponEntityFactory.GetWeaponEntity(player.playerWeaponCustomize.EmptyConstWeaponkey);
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
