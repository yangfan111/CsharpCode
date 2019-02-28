using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Shared.FreeFramework.framework.util;
using App.Shared.GameModules.Bullet;
using com.wd.free.action.function;
using com.wd.free.@event;
using com.wd.free.item;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using WeaponConfigNs;

namespace App.Server.GameModules.GamePlay.Free.item
{
    public enum ItemType
    {
        Armor,
        Helmet
    }

    public class ReduceDamageUtil
    {
        public static ItemPosition GetArmor(FreeData fd)
        {
            ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory("armor");
            if (ii != null && ii.posList.Count > 0)
            {
                ItemPosition ip = ii.posList[0];
                return ip;
            }
            return null;
        }

        public static ItemPosition GetHelmet(FreeData fd)
        {
            ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory("hel");
            if (ii != null && ii.posList.Count > 0)
            {
                ItemPosition ip = ii.posList[0];
                return ip;
            }
            return null;
        }

        public static ItemPosition GetBag(FreeData fd)
        {
            ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory("bag");
            if (ii != null && ii.posList.Count > 0)
            {
                ItemPosition ip = ii.posList[0];
                return ip;
            }
            return null;
        }

        public static float HandleDamage(IEventArgs args, FreeData fd, PlayerDamageInfo damage)
        {
            float readDamage = damage.damage;
            if (damage.type != (int)EUIDeadType.Weapon && damage.type != (int)EUIDeadType.Unarmed)
            {
                return readDamage;
            }
            if (damage.part == (int)EBodyPart.Head)
            {
                ItemPosition ip = GetHelmet(fd);
                if (ip != null)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());

                    int percent = 0;
                    if (info.id == 8)
                    {
                        percent = 30;
                    }
                    if (info.id == 9)
                    {
                        percent = 40;
                    }
                    else if (info.id == 10)
                    {
                        percent = 55;
                    }

                    readDamage = ReduceDamage(args, fd, damage, ip, percent, ItemType.Helmet);
                }
            }
            else if (damage.part == (int)EBodyPart.Chest || damage.part == (int)EBodyPart.Stomach || damage.part == (int)EBodyPart.Pelvis)
            {
                ItemPosition ip = GetArmor(fd);
                if (ip != null)
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                    int percent = 0;
                    if (info.id == 1)
                    {
                        percent = 30;
                    }
                    if (info.id == 2)
                    {
                        percent = 40;
                    }
                    else if (info.id == 3)
                    {
                        percent = 55;
                    }

                    readDamage = ReduceDamage(args, fd, damage, ip, percent, ItemType.Armor);
                }
            }

            return readDamage;
        }

        private static float ReduceDamage(IEventArgs args, FreeData fd, PlayerDamageInfo damage, ItemPosition ip, int percent, ItemType itemType)
        {
            if (ip != null)
            {
                float realDamage = damage.damage;
                float reduce = damage.damage * percent / 100;
                float realReduce = reduce;

                damage.damage -= realReduce;
                fd.Player.statisticsData.Statistics.DefenseDamage += reduce;

                // 普通帽子不减少
                if (reduce > 0)
                {
                    ip.SetCount(ip.GetCount() - (int)realDamage);
                    UpdateGamePlayData(fd, ip, itemType);

                    args.TempUse("current", fd);

                    if (ip.GetCount() <= 0)
                    {
                        ip.GetInventory().RemoveItem((FreeRuleEventArgs)args, ip);
                        FuntionUtil.Call(args, "showBottomTip", "msg", "{desc:10075," + ip.key.GetName() + "}");
                    }
                    else
                    {
                        ip.GetInventory().GetInventoryUI().UpdateItem((FreeRuleEventArgs)args, ip.GetInventory(), ip);
                    }

                    args.Resume("current");
                }
            }

            return damage.damage;
        }

        public static void UpdateArmorAndHelmet(FreeData fd)
        {
            UpdateGamePlayData(fd, GetArmor(fd), ItemType.Armor);
            UpdateGamePlayData(fd, GetHelmet(fd), ItemType.Helmet);
            if (FreeItemConfig.GetItemInfo(GetArmor(fd).key.GetKey()).id == 3 &&
                FreeItemConfig.GetItemInfo(GetHelmet(fd).key.GetKey()).id == 10 &&
                FreeItemConfig.GetItemInfo(GetBag(fd).key.GetKey()).id == 6)
            {
                fd.Player.statisticsData.Statistics.IsFullArmed = true;
            }
        }

        public static void UpdateGamePlayData(FreeData fd, ItemPosition ip, ItemType itemType)
        {
            GamePlayComponent gamePlay = fd.Player.gamePlay;

            if (ip == null)
            {
                if(itemType == ItemType.Armor)
                {
                    gamePlay.MaxArmor = 0;
                    gamePlay.CurArmor = 0;
                }
                if (itemType == ItemType.Helmet)
                {
                    gamePlay.MaxHelmet = 0;
                    gamePlay.CurHelmet = 0;
                }
            }
            else
            {
                FreeItemInfo itemInfo = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                switch (itemType)
                {
                    case ItemType.Armor:
                        gamePlay.CurArmor = Math.Max(0, ip.GetCount());
                        if (itemInfo.id == 1)
                        {
                            gamePlay.MaxArmor = 200;
                        }
                        else if (itemInfo.id == 2)
                        {
                            gamePlay.MaxArmor = 220;
                        }
                        else if (itemInfo.id == 3)
                        {
                            gamePlay.MaxArmor = 250;
                        }
                        break;
                    case ItemType.Helmet:
                        gamePlay.CurHelmet = Math.Max(0, ip.GetCount());
                        if (itemInfo.id == 8)
                        {
                            gamePlay.MaxHelmet = 80;
                        }
                        else if (itemInfo.id == 9)
                        {
                            gamePlay.MaxHelmet = 150;
                        }
                        else if (itemInfo.id == 10)
                        {
                            gamePlay.MaxHelmet = 230;
                        }
                        break;
                }
            }

        }
    }
}
