using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Server.GameModules.GamePlay.Free.weapon;
using App.Shared;
using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.skill;
using Core;
using gameplay.gamerule.free.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Server.GameModules.GamePlay.Free.item
{
    public class CarryClipUtil
    {
        public static int GetClipCount(int clipType, FreeData fd, IEventArgs args)
        {
            int count = 0;
            ItemPosition[] ips = fd.freeInventory.GetInventoryManager().GetDefaultInventory().GetItems();
            foreach (ItemPosition ip in ips)
            {
                if (ip.GetParameters().HasPara("ClipType") && (int)ip.GetParameters().Get("ClipType").GetValue() == clipType)
                {
                    count += ip.GetCount();
                }
            }

            return count;
        }

        public static void AddCurrentClipToBag(int key, FreeData fd, IEventArgs args)
        {
            int remainClip = GetCurrentWeaponClip(key, fd, args);
            int clipType = GetClipType(args.GameContext, key, fd);
            AddClip(remainClip, clipType, fd, args);
            fd.Player.WeaponController().SetReservedBullet((EBulletCaliber)clipType, CarryClipUtil.GetClipCount(clipType, fd, args));
        }

        public static void AddClip(int count, int clipType, FreeData fd, IEventArgs args)
        {
            if (BagCapacityUtil.CanAddToBag(args, fd, (int)ECategory.GameItem, clipType, count))
            {
                int leftCount = count;
                ItemPosition[] ips = fd.freeInventory.GetInventoryManager().GetDefaultInventory().GetItems();
                foreach (ItemPosition ip in ips)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                    if (ip.GetParameters().HasPara("ClipType") && (int)ip.GetParameters().Get("ClipType").GetValue() == clipType)
                    {
                        int added = Math.Min(leftCount, info.stack - ip.count);
                        leftCount -= added;

                        ip.SetCount(ip.GetCount() + added);
                        ip.GetInventory().GetInventoryUI().UpdateItem((FreeRuleEventArgs)args, ip.GetInventory(), ip);
                    }
                }

                if (leftCount > 0)
                {
                    FreeItem item = FreeItemManager.GetItem((FreeRuleEventArgs)args, FreeItemConfig.GetItemKey((int)ECategory.GameItem, clipType), leftCount);
                    if (item != null)
                    {
                        fd.freeInventory.GetInventoryManager().GetDefaultInventory().AddItem((ISkillArgs)args, item, true);
                    }
                }
            }
            else
            {
                FreeItem item = FreeItemManager.GetItem((FreeRuleEventArgs)args, FreeItemConfig.GetItemKey((int)ECategory.GameItem, clipType), count);
                if (item != null)
                {
                    fd.freeInventory.GetInventoryManager().GetInventory("ground").AddItem((ISkillArgs)args, item, true);
                }
            }
        }

        public static void DeleteClip(int count, int clipType, FreeData fd, IEventArgs args)
        {
            int leftCount = count;
            ItemPosition[] ips = fd.freeInventory.GetInventoryManager().GetDefaultInventory().GetItems();
            List<ItemPosition> remove = new List<ItemPosition>();
            foreach (ItemPosition ip in ips)
            {
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                if (ip.GetParameters().HasPara("ClipType") && (int)ip.GetParameters().Get("ClipType").GetValue() == clipType)
                {
                    if(leftCount >= ip.GetCount())
                    {
                        leftCount -= ip.GetCount();

                        remove.Add(ip);
                    }
                    else
                    {
                        ip.SetCount(ip.GetCount() - leftCount);
                        ip.GetInventory().GetInventoryUI().UpdateItem((FreeRuleEventArgs)args, ip.GetInventory(), ip);
                        break;
                    }
                }
            }

            foreach (ItemPosition ip in remove)
            {
                fd.freeInventory.GetInventoryManager().GetDefaultInventory().RemoveItem((FreeRuleEventArgs)args, ip);
            }
        }

        public static void DeleteGrenade(int count, int id, FreeData fd, IEventArgs args)
        {
            int leftCount = count;
            ItemPosition[] ips = fd.freeInventory.GetInventoryManager().GetDefaultInventory().GetItems();
            List<ItemPosition> remove = new List<ItemPosition>();
            foreach (ItemPosition ip in ips)
            {
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                if ((int)ip.GetParameters().Get("itemId").GetValue() == id)
                {
                    if (leftCount >= ip.GetCount())
                    {
                        leftCount -= ip.GetCount();

                        remove.Add(ip);
                    }
                    else
                    {
                        ip.SetCount(ip.GetCount() - leftCount);
                        ip.GetInventory().GetInventoryUI().UpdateItem((FreeRuleEventArgs)args, ip.GetInventory(), ip);
                        break;
                    }
                }
            }

            foreach (ItemPosition ip in remove)
            {
                fd.freeInventory.GetInventoryManager().GetDefaultInventory().RemoveItem((FreeRuleEventArgs)args, ip);
            }
        }


        public static int GetClipType(Contexts contexts, int weaponKey, FreeData fd)
        {
            var weaponAgent = fd.Player.WeaponController().GetWeaponAgent(FreeWeaponUtil.GetSlotType(weaponKey));
            WeaponResConfigItem weapon = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponAgent.ConfigId);
            return weapon.Caliber;
        }

        public static int GetWeaponKey(string invName, FreeData fd)
        {
            if (invName.StartsWith("w"))
            {
                return NumberUtil.GetInt(invName);
            }

            return 0;
        }

        public static int GetWeaponClip(Contexts contexts, EWeaponSlotType type, FreeData fd)
        {
            var weaponAgent = fd.Player.WeaponController().GetWeaponAgent(type);
            return weaponAgent.IsValid() ? weaponAgent.BaseComponent.Bullet : 0;

        }

        public static int GetCurrentWeaponClip(int key, FreeData fd, IEventArgs args)
        {

            var weaponAgent = fd.Player.WeaponController().HeldWeaponAgent;
            return weaponAgent.IsValid() ? weaponAgent.BaseComponent.Bullet : 0;

        }
    }
}
