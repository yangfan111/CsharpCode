using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.item.config;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.action.function;
using com.wd.free.@event;
using com.wd.free.item;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    UseCommonAction use = new UseCommonAction();
                    use.key = "showBottomTip";
                    use.values = new List<ArgValue>();
                    use.values.Add(new ArgValue("msg", "{desc:10074}"));

                    args.TempUse("current", fd);
                    use.Act(args);
                    args.Resume("current");

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
                UseCommonAction use = new UseCommonAction();
                use.key = "showBottomTip";
                use.values = new List<ArgValue>();
                use.values.Add(new ArgValue("msg", "{desc:10074}"));

                args.TempUse("current", fd);
                use.Act(args);
                args.Resume("current");
            }

            return can;
        }

        public static bool CanAddToBag(IEventArgs args, FreeData fd, int cat, int id, int count)
        {
            bool can = true;
            float capacity = GetCapacity(fd);
            float weight = GetWeight(fd);
            FreeItemInfo info = FreeItemConfig.GetItemInfo(cat, id);
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
                UseCommonAction use = new UseCommonAction();
                use.key = "showBottomTip";
                use.values = new List<ArgValue>();
                use.values.Add(new ArgValue("msg", "{desc:10073}"));

                args.TempUse("current", fd);
                use.Act(args);
                args.Resume("current");
            }

            return can;
        }

        // 获取给定物品在玩家身上相同类型的物品的容量
        public static float GetCapacity(FreeData fd, int cat, int id)
        {
            foreach (string name in fd.freeInventory.GetInventoryManager().GetInventoryNames())
            {
                if (name != "ground")
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
            float w = 20;
            foreach (string name in fd.freeInventory.GetInventoryManager().GetInventoryNames())
            {
                if (name != "ground")
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
    }
}
