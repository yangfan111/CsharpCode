using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Shared;
using App.Shared.GameModules.Weapon;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.@event;
using com.wd.free.item;
using Core;
using Core.Free;
using Core.Utils;
using Free.framework;
using System;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.item
{
    public class BagCapacityUtil
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BagCapacityUtil));

        public static bool CanAddToBag(IEventArgs args, FreeData fd, ItemPosition ip)
        {
            FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
            return CanAddToBag(args, fd, info.cat, info.id, ip.count);
        }

        public static bool CanTakeOff(IEventArgs args, FreeData fd, int cat, int id)
        {
            FreeItemInfo info = FreeItemConfig.GetItemInfo(cat, id);
            if (info.cat == (int)ECategory.Avatar)
            {
                if (GetCapacity(fd) - GetWeight(fd) < info.capacity)
                {
                    SimpleProto msg = FreePool.Allocate();
                    msg.Key = FreeMessageConstant.ChickenTip;
                    msg.Ss.Add("word74");
                    FreeMessageSender.SendMessage(fd.Player, msg);
                    return false;
                }
            }
            return true;
        }

        public static bool CanChangeBag(IEventArgs args, FreeData fd, int cat, int id)
        {
            bool can = true;
            float capacity = GetCapacity(fd);
            float weight = GetWeight(fd);
            FreeItemInfo info = FreeItemConfig.GetItemInfo(cat, id);
            if (cat == (int)ECategory.Avatar)
            {
                float oldCap = GetCapacity(fd, cat, id);
                can = Math.Round(capacity - weight, 3) >= Math.Round(oldCap - info.capacity, 3);
            }

            if (!can)
            {
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ChickenTip;
                msg.Ss.Add("word74");
                FreeMessageSender.SendMessage(fd.Player, msg);
            }
            return can;
        }

        public static int CanAddToBagCount(IEventArgs args, FreeData fd, int cat, int id, int count)
        {
            if (count > 1 && cat != (int)ECategory.Avatar)
            {
                int canCount = 0;
                float capacity = GetCapacity(fd);
                float weight = GetWeight(fd);
                FreeItemInfo info = FreeItemConfig.GetItemInfo(cat, id);

                if (cat == (int)ECategory.GameItem || cat == (int)ECategory.WeaponPart)
                {
                    canCount = (int)(Math.Round(capacity - weight, 3) / info.weight);
                }

                if (cat == (int)ECategory.Weapon)
                {
                    WeaponResConfigItem item = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(id);
                    if (item.Type == (int)EWeaponType_Config.ThrowWeapon)
                    {
                        canCount = (int)(Math.Round(capacity - weight, 3) / info.weight);
                    }
                    if (item.Type == (int) EWeaponType_Config.Armor || item.Type == (int) EWeaponType_Config.Helmet)
                    {
                        canCount = count;
                    }
                }

                int realCount = Math.Min(count, canCount);
                if (realCount == 0)
                {
                    SimpleProto msg = FreePool.Allocate();
                    msg.Key = FreeMessageConstant.ChickenTip;
                    msg.Ss.Add("word73");
                    FreeMessageSender.SendMessage(fd.Player, msg);
                }
                return realCount;
            }
            return CanAddToBag(args, fd, cat, id, count) ? 1 : 0;
        }

        public static bool CanAddToBag(IEventArgs args, FreeData fd, int cat, int id, int count)
        {
            bool can = true;
            float capacity = GetCapacity(fd);
            float weight = GetWeight(fd);
            FreeItemInfo info = FreeItemConfig.GetItemInfo(cat, id);
            if (info == null)
            {
                return false; // 无效道具
            }
            if (cat == (int)ECategory.Avatar)
            {
                float oldCap = GetCapacity(fd, cat, id);

                can = Math.Round(capacity - weight, 3) >= oldCap - info.capacity;
            }
            if (cat == (int)ECategory.GameItem || cat == (int)ECategory.WeaponPart)
            {
                can = Math.Round(capacity - weight, 3) >= info.weight * count;
            }
            if (cat == (int)ECategory.Weapon)
            {
                WeaponResConfigItem item = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(id);
                if (item.Type == (int)EWeaponType_Config.ThrowWeapon)
                {
                    can = Math.Round(capacity - weight, 3) >= info.weight * count;
                }
            }

            if (!can)
            {
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ChickenTip;
                msg.Ss.Add("word73");
                FreeMessageSender.SendMessage(fd.Player, msg);
            }
            return can;
        }

        // 获取给定物品在玩家身上相同类型的物品的容量
        public static float GetCapacity(FreeData fd, int cat, int id)
        {
            foreach (string name in fd.freeInventory.GetInventoryManager().GetInventoryNames())
            {
                if (name != ChickenConstant.BagGround)
                {
                    ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(name);
                    if (ii != null)
                    {
                        foreach (ItemPosition ip in ii.GetItems())
                        {
                            FreeItemInfo currentInfo = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());
                            if (currentInfo.cat == (int)ECategory.Avatar && cat == (int)ECategory.Avatar)
                            {
                                RoleAvatarConfigItem avatar1 = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(currentInfo.id);
                                RoleAvatarConfigItem avatar2 = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(id);

                                if (avatar1.Type == avatar2.Type)
                                {
                                    return avatar1.Capacity;
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

        // 获取玩家总容量
        public static float GetCapacity(FreeData fd)
        {
            float w = 70;
            /*foreach (string name in fd.freeInventory.GetInventoryManager().GetInventoryNames())
            {
                if (name != ChickenConstant.BagGround)
                {
                    ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(name);
                    if (ii != null)
                    {
                        foreach (ItemPosition ip in ii.GetItems())
                        {
                            FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());
                            if (info.cat == 9)
                            {
                                RoleAvatarConfigItem avatar = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(info.id);
                                w += avatar.Capacity;
                            }
                        }
                    }
                }
            }*/
            ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagBag);
            if (ii != null)
            {
                foreach (ItemPosition ip in ii.GetItems())
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());
                    if (info.cat == 9)
                    {
                        RoleAvatarConfigItem avatar = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(info.id);
                        w += avatar.Capacity;
                    }
                }
            }
            return w;
        }

        // 获取玩家物品总重量
        public static float GetWeight(FreeData fd)
        {
            float w = 0;
            ItemInventory ii = fd.GetFreeInventory().GetInventoryManager().GetDefaultInventory();
            if (ii != null)
            {
                foreach (ItemPosition ip in ii.GetItems())
                {
                    FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());

                    w += ip.GetCount() * info.weight;
                }
            }
            return w;
        }

        public static bool CanDemountAttachment(ServerRoom room, FreeData fd, FreeItemInfo part, string ipStr, bool toGround)
        {
            double capacity = Math.Round(GetCapacity(fd) - GetWeight(fd), 3);
            float bulletWeight = 0f;
            WeaponBaseAgent agent = fd.Player.WeaponController().GetWeaponAgent((EWeaponSlotType) short.Parse(ipStr.Substring(1, 1)));
            int overBullet = 0;

            if (SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(WeaponPartUtil.GetWeaponFstMatchedPartId(part.id, agent.ConfigId)).Bullet > 0)
            {
                overBullet = agent.BaseComponent.Bullet - agent.WeaponConfigAssy.PropertyCfg.Bullet;
                if (overBullet > 0)
                {
                    bulletWeight = SingletonManager.Get<GameItemConfigManager>().GetConfigById((int) agent.Caliber).Weight * overBullet;
                }
            }

            var partWeight = 0f;
            if (!toGround) partWeight = part.weight;

            if (capacity < bulletWeight + partWeight)
            {
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ChickenTip;
                msg.Ss.Add("word79");
                FreeMessageSender.SendMessage(fd.Player, msg);
                return false;
            }

            if (overBullet > 0)
            {
                agent.BaseComponent.Bullet = agent.WeaponConfigAssy.PropertyCfg.Bullet;
                CarryClipUtil.AddClip(overBullet, (int) agent.Caliber, fd, room.ContextsWrapper.FreeArgs);
                fd.Player.WeaponController().SetReservedBullet(agent.Caliber, CarryClipUtil.GetClipCount((int) agent.Caliber, fd, room.ContextsWrapper.FreeArgs));
            }

            return true;
        }

        public static bool CanExchangeAttachment(ServerRoom room, FreeData fd, FreeItemInfo fromPart, FreeItemInfo toPart, WeaponBaseAgent fromAgent, WeaponBaseAgent toAgent)
        {
            double capacity = Math.Round(GetCapacity(fd) - GetWeight(fd), 3);

            int toBullet = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(WeaponPartUtil.GetWeaponFstMatchedPartId(toPart.id, fromAgent.ConfigId)).Bullet;
            int fromBullet = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(WeaponPartUtil.GetWeaponFstMatchedPartId(fromPart.id, toAgent.ConfigId)).Bullet;

            if (toBullet == fromBullet)
            {
                return true;
            }

            int overBulletFrom = fromAgent.BaseComponent.Bullet - fromAgent.WeaponConfigAssy.PropertyCfg.Bullet - toBullet;
            int overBulletTo = toAgent.BaseComponent.Bullet - toAgent.WeaponConfigAssy.PropertyCfg.Bullet - fromBullet;

            float bulletWeight = 0f;
            if (overBulletFrom > 0)
            {
                bulletWeight += SingletonManager.Get<GameItemConfigManager>().GetConfigById((int) fromAgent.Caliber).Weight * overBulletFrom;
            }

            if (overBulletTo > 0)
            {
                bulletWeight += SingletonManager.Get<GameItemConfigManager>().GetConfigById((int) toAgent.Caliber).Weight * overBulletTo;
            }

            if (capacity < bulletWeight)
            {
                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ChickenTip;
                msg.Ss.Add("word79");
                FreeMessageSender.SendMessage(fd.Player, msg);
                return false;
            }

            if (overBulletFrom > 0)
            {
                fromAgent.BaseComponent.Bullet = fromAgent.WeaponConfigAssy.PropertyCfg.Bullet + toBullet;
                CarryClipUtil.AddClip(overBulletFrom, (int) fromAgent.Caliber, fd, room.ContextsWrapper.FreeArgs);
                fd.Player.WeaponController().SetReservedBullet(fromAgent.Caliber, CarryClipUtil.GetClipCount((int) fromAgent.Caliber, fd, room.ContextsWrapper.FreeArgs));
            }

            if (overBulletTo > 0)
            {
                toAgent.BaseComponent.Bullet = toAgent.WeaponConfigAssy.PropertyCfg.Bullet + fromBullet;
                CarryClipUtil.AddClip(overBulletTo, (int) toAgent.Caliber, fd, room.ContextsWrapper.FreeArgs);
                fd.Player.WeaponController().SetReservedBullet(toAgent.Caliber, CarryClipUtil.GetClipCount((int) toAgent.Caliber, fd, room.ContextsWrapper.FreeArgs));
            }

            return true;
        }
    }
}
