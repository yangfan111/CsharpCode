using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.item;
using App.Shared;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;
using System;
using Utils.Singleton;
using WeaponConfigNs;

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
            if (para != null)
            {
                var playerEntity = ((SimpleParable)para).GetFieldObject(0) as PlayerEntity;
                var fd = playerEntity.freeData.FreeData as FreeData;
                var weaponId = FreeUtil.ReplaceInt("{state.id}", args);
                if (weaponId > 0)
                {
                    WeaponResConfigItem item = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
                    if (item.Type == (int) EWeaponType_Config.ThrowWeapon)
                    {
                        CarryClipUtil.DeleteGrenade(1, weaponId, fd, args);
                    }
                    else
                    {
                        int ClipType = FreeUtil.ReplaceInt("{state.ClipType}", args);
                        int count = playerEntity.WeaponController().GetReservedBullet((EBulletCaliber)ClipType);
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
            }
            /*var playerEntity = (PlayerEntity)((SimpleParable)para).GetFieldObject(0);
            if (para != null)
            {
                var controller = playerEntity.WeaponController();
                var lastId = controller.GetWeaponAgent(EWeaponSlotType.LastPointer).ConfigId;
                var currentId = controller.HeldWeaponAgent.ConfigId;

                FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit(FreeArgConstant.PlayerCurrent);

                if (currentId > 0)
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
            }*/
        }
    }
}
