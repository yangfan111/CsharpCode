using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.entity;
using App.Server.GameModules.GamePlay.Free.item;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Server.GameModules.GamePlay.Free.player;
using App.Server.GameModules.GamePlay.Free.weapon;
using App.Shared;
using App.Shared.GameModules.GamePlay.Free;
using App.Shared.GameModules.GamePlay.Free.Map;
using App.Shared.GameModules.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.item;
using com.wd.free.para;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.rule;
using Sharpen;
using UnityEngine;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.client
{
    /// <summary>
    /// Defines the <see cref="PickupItemUtil" />
    /// </summary>
    public class PickupItemUtil
    {
        private static MyDictionary<string, string> typeInvDic;

        static PickupItemUtil()
        {
            typeInvDic = new MyDictionary<string, string>();
            for (int i = 1; i <= 5; i++)
            {
                typeInvDic["p" + i] = ChickenConstant.BagPrimeWeapon + i;
            }
            typeInvDic[ChickenConstant.ItemCatPistolWeapon] = ChickenConstant.BagPistolWeapon;
        }

        public static bool IsDefault(string key)
        {
            return !string.IsNullOrEmpty(key) && key.StartsWith(ChickenConstant.BagDefault);
        }

        public static bool IsGround(string key)
        {
            return !string.IsNullOrEmpty(key) && key.StartsWith(ChickenConstant.BagGround);
        }

        public static bool isWeapon(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                string w = key.Split(',')[0].Trim();
                if (w.Length == 2 && w.StartsWith("w"))
                {
                    return true;
                }
            }

            return false;
        }

        public static SimpleItemInfo GetGroundItemInfo(ServerRoom room, FreeData fd, string key)
        {
            SimpleItemInfo freeItem = new SimpleItemInfo();

            string[] xy = StringUtil.Split(key, ParaConstant.SPLITER_COMMA);
            if (xy.Length == 3)
            {
                string name = xy[0];
                int x = NumberUtil.GetInt(xy[1]);
                int y = NumberUtil.GetInt(xy[2]);

                if (x == 0)
                {
                    SceneObjectEntity entity = room.RoomContexts.sceneObject.GetEntityWithEntityKey(new EntityKey(y, (short)EEntityType.SceneObject));
                    if (entity != null)
                    {
                        freeItem.entityId = entity.entityKey.Value.EntityId;
                        freeItem.cat = entity.simpleItem.Category;
                        freeItem.id = entity.simpleItem.Id;
                        freeItem.count = entity.simpleItem.Count;
                    }
                    else
                    {
                        FreeMoveEntity moveEntity = room.RoomContexts.freeMove.GetEntityWithEntityKey(new EntityKey(y, (short)EEntityType.FreeMove));
                        if (moveEntity != null)
                        {
                            SimpleItemInfo info = (SimpleItemInfo)SingletonManager.Get<DeadBoxParser>().FromString(moveEntity.freeData.Value);
                            freeItem.entityId = moveEntity.entityKey.Value.EntityId;
                            freeItem.cat = info.cat;
                            freeItem.id = info.id;
                            freeItem.count = info.count;
                        }
                        else
                        {
                            Debug.LogErrorFormat("entity {0} not existed.", key);
                        }
                    }
                }
            }

            return freeItem;
        }

        public static void ShowSplitUI(ServerRoom room, FreeData fd, string key)
        {
            // 分拆道具
            if (key.StartsWith(ChickenConstant.BagDefault))
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, key, fd.GetFreeInventory().GetInventoryManager());
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                var stackable = true;
                if (info.cat == (int)ECategory.Weapon)
                {
                    WeaponResConfigItem item = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(info.id);
                    if (item.Type != (int)EWeaponType_Config.ThrowWeapon)
                    {
                        stackable = false;
                    }
                }
                else
                {
                    if (info.stack <= 1)
                    {
                        stackable = false;
                    }
                }
                SimpleProto data = FreePool.Allocate();
                if (!stackable || ip.count <= 1)
                {
                    data.Key = FreeMessageConstant.ChickenTip;
                    data.Ss.Add("word78");
                    FreeMessageSender.SendMessage(fd.Player, data);
                    return;
                }
                data.Key = FreeMessageConstant.ShowSplitUI;
                data.Ins.Add(info.cat);
                data.Ins.Add(info.id);
                data.Ins.Add(ip.count);
                data.Ss.Add(key);
                data.Ss.Add(ip.GetKey().GetName());
                FreeMessageSender.SendMessage(fd.Player, data);
                PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, fd.Player.gamePlay);
            }
        }

        public static bool AddItemToPlayer(ServerRoom room, PlayerEntity player, int entityId, int cat, int id, int count, string toInv = "")
        {
            SceneObjectEntity entity = room.RoomContexts.sceneObject.GetEntityWithEntityKey(new EntityKey(entityId, (short)EEntityType.SceneObject));
            FreeMoveEntity freeMoveEntity = null;
            if (entity == null || entity.isFlagDestroy)
            {
                freeMoveEntity = room.RoomContexts.freeMove.GetEntityWithEntityKey(new EntityKey(entityId, (short)EEntityType.FreeMove));
                if (freeMoveEntity == null)
                {
                    return false;
                }
            }

            FreeData fd = (FreeData) player.freeData.FreeData;
            room.FreeArgs.TempUse("current", fd);

            if (!FreeItemConfig.Contains(cat, id)) return false;

            FreeItemInfo item = FreeItemConfig.GetItemInfo(cat, id);
            CreateItemToPlayerAction action = new CreateItemToPlayerAction();

            action.name = ChickenConstant.BagDefault;
            switch (item.cat)
            {
                case (int)ECategory.Weapon:
                    if (item.subType == ChickenConstant.ItemCatPrimeWeapon)
                    {
                        action.name = ChickenConstant.BagPrimeWeapon;

                        int c1 = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagPrimeWeapon).posList.Count;
                        int c2 = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagSecondaryWeapon).posList.Count;

                        if (toInv.StartsWith(ChickenConstant.BagPrimeWeapon))
                        {
                            action.name = ChickenConstant.BagPrimeWeapon;
                            if (c1 > 0)
                            {
                                DropItem(action.name, fd, room);
                            }
                        }
                        else if (toInv.StartsWith(ChickenConstant.BagSecondaryWeapon))
                        {
                            action.name = ChickenConstant.BagSecondaryWeapon;
                            if (c2 > 0)
                            {
                                DropItem(action.name, fd, room);
                            }
                        }
                        else
                        {
                            if (c1 > 0 && c2 == 0)
                            {
                                action.name = ChickenConstant.BagSecondaryWeapon;
                            }

                            if (c1 > 0 && c2 > 0)
                            {
                                int currentKey = FreeWeaponUtil.GetWeaponKey(fd.Player.WeaponController().HeldSlotType);
                                if (currentKey == 0)
                                {
                                    currentKey = 1;
                                }
                                if (currentKey == 1 || currentKey == 2)
                                {
                                    action.name = "w" + currentKey;
                                }
                                else
                                {
                                    action.name = ChickenConstant.BagPrimeWeapon;
                                }

                                DropItem(action.name, fd, room);
                            }
                        }
                    }
                    else if (item.subType == ChickenConstant.ItemCatPistolWeapon)
                    {
                        action.name = ChickenConstant.BagPistolWeapon;
                        DropItem(action.name, fd, room);
                    }
                    else if (item.subType == ChickenConstant.ItemCatMeleeWeapon)
                    {
                        action.name = ChickenConstant.BagMeleeWeapon;
                        DropItem(action.name, fd, room);
                    }
                    else if (item.subType == ChickenConstant.ItemCatGrenadeWeapon)
                    {
                        action.name = ChickenConstant.BagDefault;
                    }
                    else if(item.subType == ChickenConstant.ItemCatArmor)
                    {
                        action.name = ChickenConstant.BagArmor;
                        if (entity != null) count = entity.armorDurability.CurDurability;
                        DropItem(action.name, fd, room);
                    }
                    else if(item.subType == ChickenConstant.ItemCatHelmet)
                    {
                        action.name = ChickenConstant.BagHelmet;
                        if (entity != null) count = entity.armorDurability.CurDurability;
                        DropItem(action.name, fd, room);
                    }
                    break;
                case (int)ECategory.Avatar:
                    action.name = item.subType;
                    DropItem(item.subType, fd, room);
                    break;
                case (int)ECategory.WeaponPart:
                    action.name = AutoPutPart(fd, item, toInv, room);
                    break;
                default:
                    break;
            }
            action.key = FreeItemConfig.GetItemKey(item.cat, item.id);

            int canCount = 0;
            if (action.name == ChickenConstant.BagDefault)
            {
                canCount = BagCapacityUtil.CanAddToBagCount(room.FreeArgs, fd, item.cat, item.id, count);
            }
            else if (item.type == ChickenConstant.ItemCatAvatar || action.name == ChickenConstant.BagHelmet || action.name == ChickenConstant.BagArmor)
            {
                canCount = count;
            }
            else
            {
                canCount = 1;
            }

            bool pickupSuccess = false;
            if (canCount > 0 && !string.IsNullOrEmpty(action.name))
            {
                PlayerAnimationAction.DoAnimation(room.RoomContexts, PlayerAnimationAction.Interrupt, fd.Player, true);
                PlayerAnimationAction.DoAnimation(room.RoomContexts, PlayerAnimationAction.PickUp, fd.Player, true);
                PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, fd.Player.gamePlay);

                if (!string.IsNullOrEmpty(action.key))
                {
                    action.count = canCount.ToString();
                    action.SetPlayer("current");
                    room.FreeArgs.TempUse("current", fd);
                    action.Act(room.FreeArgs);
                    room.FreeArgs.Resume("current");

                    if (count > canCount)
                    {
                        SimpleProto msg = FreePool.Allocate();
                        msg.Key = FreeMessageConstant.ChickenTip;
                        msg.Ss.Add("word80," + count + "," + canCount);
                        FreeMessageSender.SendMessage(fd.Player, msg);
                        room.FreeArgs.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity((ECategory)item.cat,
                            item.id, count - canCount, entity == null ? freeMoveEntity.position.Value : entity.position.Value);
                    }
                    pickupSuccess = true;
                }
                else
                {
                    Debug.LogError(item.cat + "-" + item.key + " not existed");
                }

                if (entity != null) entity.isFlagDestroy = true;
                if (freeMoveEntity != null)
                {
                    freeMoveEntity.isFlagDestroy = true;
                    if (freeMoveEntity.freeData.Cat == FreeEntityConstant.DeadBox)
                    {
                        var deadBox = room.FreeArgs.GameContext.freeMove.GetEntityWithEntityKey(new EntityKey(freeMoveEntity.freeData.IntValue, (short) EEntityType.FreeMove));
                        if (deadBox != null && deadBox.freeData.EmptyDelete)
                        {
                            if (deadBox.freeData.IntValue > 1) deadBox.freeData.IntValue--;
                            else deadBox.isFlagDestroy = true;
                        }
                    }
                }
            }
            room.FreeArgs.Resume("current");

            return pickupSuccess;
        }

        public static string AutoPutPart(FreeData fd, FreeItemInfo partInfo, string toInv = "", ServerRoom room = null)
        {
            string inv = null;
            if (toInv.StartsWith(ChickenConstant.BagPrimeWeapon))
            {
                inv = putOnPart(fd, 1, partInfo, toInv, room);
            }
            else if (toInv.StartsWith(ChickenConstant.BagSecondaryWeapon))
            {
                inv = putOnPart(fd, 2, partInfo, toInv, room);
            }
            else if (toInv.StartsWith(ChickenConstant.BagPistolWeapon))
            {
                inv = putOnPart(fd, 3, partInfo, toInv, room);
            }
            else
            {
                inv = putOnPart(fd, 1, partInfo);
                if (inv == null)
                {
                    inv = putOnPart(fd, 2, partInfo);
                    if (inv == null)
                    {
                        inv = putOnPart(fd, 3, partInfo);
                    }
                }
            }
            return inv == null ? ChickenConstant.BagDefault : inv;
        }

        private static string putOnPart(FreeData fd, int weaponType, FreeItemInfo info, string toInv = "", ServerRoom room = null)
        {
            WeaponResConfigItem weapon = GetWeapon(fd, weaponType);
            if (weapon != null)
            {
                int detailId = WeaponPartUtil.GetWeaponFstMatchedPartId(info.id, weapon.Id);
                var weaponconfigMngr = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(weapon.Id);
                foreach (EWeaponPartType part in weaponconfigMngr.ApplyPartsSlot)
                {
                    int p = FreeWeaponUtil.GetWeaponPart(part);
                    if (weaponconfigMngr.IsPartMatchWeapon(detailId))
                    {
                        if ("p" + p == info.subType)
                        {
                            string inv = "w" + weaponType + p;
                            ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory(inv);
                            if (ii != null && (ii.posList.Count == 0
                                || toInv.StartsWith("w" + weaponType)))
                            {
                                if (ii.posList.Count > 0)
                                {
                                    DropPart(inv, fd, room);
                                }
                                return inv;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static void DropPart(string inv, FreeData fd, ServerRoom room)
        {
            int c3 = fd.freeInventory.GetInventoryManager().GetInventory(inv).posList.Count;
            ItemInventory w3 = fd.freeInventory.GetInventoryManager().GetInventory(inv);
            if (c3 > 0)
            {
                ItemPosition ip = w3.posList[0];

                w3.RemoveItem(room.FreeArgs, ip);

                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());

                if (BagCapacityUtil.CanAddToBag(room.FreeArgs, fd, ip))
                {
                    fd.freeInventory.GetInventoryManager().GetDefaultInventory().AddItem(room.FreeArgs, ip.key, true);
                }
                else
                {
                    if (info.cat > 0)
                    {
                        room.RoomContexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity((ECategory)info.cat,
                            info.id, ip.GetCount(), fd.Player.position.Value);
                    }
                }
            }
        }

        private static WeaponResConfigItem GetWeapon(FreeData fd, int type)
        {
            int c = fd.freeInventory.GetInventoryManager().GetInventory("w" + type).posList.Count;
            if (c > 0)
            {
                ItemInventory w = fd.freeInventory.GetInventoryManager().GetInventory("w" + type);
                ItemPosition ip = w.posList[0];
                int id = (int)((IntPara)ip.GetParameters().Get("itemId")).GetValue();

                return SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(id);
            }

            return null;
        }

        private static void handleAddToDefault(string[] types, CreateItemToPlayerAction action, FreeItemInfo item, FreeData fd, ServerRoom room, SceneObjectEntity entity)
        {
            foreach (string type in types)
            {
                if (item.subType == type)
                {
                    action.name = ChickenConstant.BagDefault;
                }
            }
        }

        private static void handleDropOne(string[] types, CreateItemToPlayerAction action, FreeItemInfo item, FreeData fd, ServerRoom room)
        {
            foreach (string type in types)
            {
                if (item.subType == type)
                {
                    action.name = typeInvDic[type];

                    DropItem(typeInvDic[type], fd, room);
                }
            }
        }

        private static void DropItem(string inv, FreeData fd, ServerRoom room)
        {
            Debug.LogFormat("inv {0}", inv);
            int c3 = fd.freeInventory.GetInventoryManager().GetInventory(inv).posList.Count;
            ItemInventory w3 = fd.freeInventory.GetInventoryManager().GetInventory(inv);
            if (c3 > 0)
            {
                ItemPosition ip = w3.posList[0];
                w3.RemoveItem(room.FreeArgs, ip);
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());

                if (info.cat > 0)
                {
                    /*room.RoomContexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity((ECategory)info.cat, 
                        info.id, ip.GetCount(), fd.Player.position.Value, ip.GetCount());*/
                    CreateSceneObjectAction.CreateDropItem(fd.Player, info.cat, info.id, ip.GetCount(), room.RoomContexts.session.entityFactoryObject.SceneObjectEntityFactory);
                }
            }
        }
    }
}
