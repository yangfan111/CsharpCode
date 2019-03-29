using com.wd.free.action;
using System;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;
using Assets.Utils.Configuration;
using WeaponConfigNs;
using Assets.XmlConfig;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.Free.item;
using Core.WeaponLogic;
using App.Server.GameModules.GamePlay.Free;
using com.wd.free.item;
using App.Server.GameModules.GamePlay.Free.chicken;
using com.wd.free.skill;
using Utils.Singleton;
using App.Shared;
using App.Shared.GameModules.Weapon;
using Core;

namespace Assets.App.Server.GameModules.GamePlay.Free.chicken
{
    /// <summary>
    /// 处理武器的特殊逻辑
    /// </summary>
    public class ChickenWeaponAction : AbstractGameAction
    {
        /// <summary>
        /// 处理武器相关的特殊逻辑
        /// </summary>
        /// <param name="args"></param>
        public override void DoAction(IEventArgs args)
        {
            IParable para = args.GetUnit("state");
            var contexts = args.GameContext;
            var playerEntity = (PlayerEntity)((SimpleParable)para).GetFieldObject(0);
            if (para != null)
            {
                var controller = playerEntity.WeaponController();
                //TODO 确认逻辑 
                var lastId = controller.GetWeaponAgent(EWeaponSlotType.LastPointer).ConfigId;
                var currentId = controller.HeldWeaponAgent.ConfigId;

                //if(null != weaponData)
                //{
                //    currentId = weaponData.WeaponId;
                //}
                //if(null != lastWeaponData)
                //{
                //    lastId = lastWeaponData.WeaponId;
                //}

                FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit(FreeArgConstant.PlayerCurrent);

                if (currentId>0)
                {
                    WeaponResConfigItem item = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(currentId);
                    if (item.Type == (int)EWeaponType_Config.ThrowWeapon)
                    {
                        CarryClipUtil.DeleteGrenade(1, lastId, fd, args);
                        if(lastId != currentId)
                        {
                            ItemInventory grenade = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagGrenadeWeapon);
                        }
                    }
                    else
                    {
                        int ClipType = FreeUtil.ReplaceInt("{state.ClipType}", args);
                        int count = fd.Player.WeaponController().GetReservedBullet((EBulletCaliber)ClipType);
                        int itemCount = CarryClipUtil.GetClipCount(ClipType, fd, args);
                        int delta = count - itemCount;
                        if (delta > 0)
                        {
                            CarryClipUtil.AddClip(Math.Abs(delta), ClipType, fd, args);
                        }
                        else
                        {
                            CarryClipUtil.DeleteClip(Math.Abs(delta), ClipType, fd, args);
                        }
                    }
                }
                else
                {
                    if (lastId != 0)
                    {
                        CarryClipUtil.DeleteGrenade(1, lastId, fd, args);
                        ItemInventory grenade = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagGrenadeWeapon);
                        grenade.RemoveItem((ISkillArgs)args, grenade.posList[0]);
                    }
                }
            }
        }
    }
}
