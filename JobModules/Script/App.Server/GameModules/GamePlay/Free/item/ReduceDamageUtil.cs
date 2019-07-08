using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Attack;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using com.wd.free.@event;
using com.wd.free.item;
using Core.Enums;
using Core.Free;
using Free.framework;
using System;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.item
{
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

            PlayerEntity playerEntity = fd.Player;
            if (playerEntity.gamePlay.CurHelmet > 0)
            {
                var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(playerEntity.gamePlay.HelmetLv);
                if (config != null)
                {
                    if (config.NewWeaponCfg.ProtectivePartsList.Contains(damage.part))
                    {
                        float reduce = readDamage * config.NewWeaponCfg.DamageReduction / 100;
                        reduce = Math.Min(playerEntity.gamePlay.CurHelmet, reduce);
                        playerEntity.gamePlay.CurHelmet = Math.Max(0, playerEntity.gamePlay.CurHelmet - (int) readDamage);
                        if (reduce > 0 && playerEntity.gamePlay.CurHelmet == 0)
                        {
                            playerEntity.gamePlay.HelmetLv = playerEntity.gamePlay.MaxHelmet = 0;
                            SimpleProto msg = FreePool.Allocate();
                            msg.Key = FreeMessageConstant.ChickenTip;
                            msg.Ss.Add("word75," + config.NewWeaponCfg.Name);
                            FreeMessageSender.SendMessage(playerEntity, msg);
                        }
                        ItemPosition ip = GetHelmet(fd);
                        if (ip != null)
                        {
                            args.TempUse("current", (FreeData) playerEntity.freeData.FreeData);
                            if (playerEntity.gamePlay.CurHelmet == 0)
                            {
                                ip.GetInventory().RemoveItem((FreeRuleEventArgs)args, ip);
                            }
                            else
                            {
                                ip.SetCount(playerEntity.gamePlay.CurHelmet);
                                ip.GetInventory().GetInventoryUI().UpdateItem((FreeRuleEventArgs)args, ip.GetInventory(), ip);
                            }
                            args.Resume("current");
                        }
                        damage.damage -= reduce;
                        readDamage = damage.damage;
                        playerEntity.statisticsData.Statistics.DefenseDamage += reduce;
                    }
                }
            }

            if (playerEntity.gamePlay.CurArmor > 0)
            {
                var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(playerEntity.gamePlay.ArmorLv);
                if (config != null)
                {
                    if (config.NewWeaponCfg.ProtectivePartsList.Contains(damage.part))
                    {
                        float reduce = readDamage * config.NewWeaponCfg.DamageReduction / 100;
                        reduce = Math.Min(playerEntity.gamePlay.CurArmor, reduce);
                        playerEntity.gamePlay.CurArmor = Math.Max(0, playerEntity.gamePlay.CurArmor - (int) readDamage);
                        if (reduce > 0 && playerEntity.gamePlay.CurArmor == 0)
                        {
                            playerEntity.gamePlay.ArmorLv = playerEntity.gamePlay.MaxArmor = 0;
                            SimpleProto msg = FreePool.Allocate();
                            msg.Key = FreeMessageConstant.ChickenTip;
                            msg.Ss.Add("word75," + config.NewWeaponCfg.Name);
                            FreeMessageSender.SendMessage(playerEntity, msg);
                        }
                        ItemPosition ip = GetArmor(fd);
                        if (ip != null)
                        {
                            args.TempUse("current", (FreeData) playerEntity.freeData.FreeData);
                            if (playerEntity.gamePlay.CurArmor == 0)
                            {
                                ip.GetInventory().RemoveItem((FreeRuleEventArgs)args, ip);
                            }
                            else
                            {
                                ip.SetCount(playerEntity.gamePlay.CurArmor);
                                ip.GetInventory().GetInventoryUI().UpdateItem((FreeRuleEventArgs)args, ip.GetInventory(), ip);
                            }
                            args.Resume("current");
                        }
                        damage.damage -= reduce;
                        readDamage = damage.damage;
                        playerEntity.statisticsData.Statistics.DefenseDamage += reduce;
                    }
                }
            }

            return readDamage;
        }
    }
}
