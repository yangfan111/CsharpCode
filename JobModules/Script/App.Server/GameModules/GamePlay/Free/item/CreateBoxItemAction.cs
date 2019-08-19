using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Server.GameModules.GamePlay.Free.map.position;
using App.Shared;
using App.Shared.GameModules.GamePlay.Free;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.map.position;
using com.wd.free.util;
using Core.EntityComponent;
using Core.Free;
using Sharpen;
using System;
using System.Collections.Generic;
using Core.EntityComponent;
using UnityEngine;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class CreateBoxItemAction : AbstractGameAction, IRule
    {
        private string name;
        private string id;
        private string cat;
        private string count;
        private IPosSelector pos;
        private int type;
        private bool delete;

        public override void DoAction(IEventArgs args)
        {
            Vector3 p = UnityPositionUtil.ToVector3(pos.Select(args));
            string realName = FreeUtil.ReplaceVar(name, args);
            var groupEntity = args.GameContext.freeMove.CreateEntity();
            groupEntity.AddEntityKey(new EntityKey(args.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
            groupEntity.AddPosition();
            groupEntity.position.Value = p;
            groupEntity.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, 1000);
            
            groupEntity.AddFreeData("", null);
            groupEntity.freeData.Value = "空投箱";
            groupEntity.freeData.IntValue = 1;
            groupEntity.freeData.EmptyDelete = false;

            switch (type)
            {
                case 1:
                    groupEntity.freeData.Cat = FreeEntityConstant.DeadBoxGroup;
                    break;
                case 2:
                    groupEntity.freeData.Cat = FreeEntityConstant.DropBoxGroup;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(id))
            {
                ItemDrop[] list = SingletonManager.Get<FreeItemDrop>().GetDropItems(FreeUtil.ReplaceVar(cat, args), FreeUtil.ReplaceInt(count, args), args.GameContext.session.commonSession.RoomInfo.MapId);
                foreach (ItemDrop drop in list)
                {
                    CreateItemFromItemDrop(args, p, drop, realName);
                    List<ItemDrop> extra = SingletonManager.Get<FreeItemDrop>().GetExtraItems(drop);
                    foreach (ItemDrop e in extra)
                    {
                        CreateItemFromItemDrop(args, p, e, realName);
                    }
                }
            }
            else
            {
                int itemCount = 0;
                PlayerEntity player = args.GameContext.player.GetEntityWithEntityKey(new EntityKey(FreeUtil.ReplaceInt(id, args), (short)EEntityType.Player));
                if (player != null)
                {
                    realName = player.playerInfo.PlayerName;
                    FreeData fd = ((FreeData)player.freeData.FreeData);
                    foreach (string inv in fd.GetFreeInventory().GetInventoryManager().GetInventoryNames())
                    {
                        ItemInventory ii = fd.GetFreeInventory().GetInventoryManager().GetInventory(inv);
                        if (ii.name != ChickenConstant.BagDefault)
                        {
                            itemCount += CreateItemFromInventory(args, fd, ii, p, realName, groupEntity.entityKey.Value.EntityId);
                        }
                    }
                    itemCount += CreateItemFromInventory(args, fd, fd.GetFreeInventory().GetInventoryManager().GetDefaultInventory(), p, realName, groupEntity.entityKey.Value.EntityId);

                    groupEntity.freeData.Value = realName;
                    groupEntity.freeData.IntValue = itemCount;
                    groupEntity.freeData.EmptyDelete = delete;
                    if (groupEntity.freeData.IntValue == 0 && groupEntity.freeData.EmptyDelete)
                        groupEntity.isFlagDestroy = true;
                }
            }

            groupEntity.isFlagSyncNonSelf = true;
        }

        private int CreateItemFromInventory(IEventArgs fr, FreeData fd, ItemInventory ii, Vector3 p, string realName, int entityId)
        {
            int itemAdded = 0;
            if (ii != null && ii.name != ChickenConstant.BagGround)
            {
                foreach (ItemPosition ip in ii.GetItems())
                {
                    FreeMoveEntity en = fr.GameContext.freeMove.CreateEntity();
                    en.AddEntityKey(new EntityKey(fr.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
                    en.AddPosition();
                    en.position.Value = new Vector3(p.x, p.y, p.z);

                    en.AddFreeData(FreeUtil.ReplaceVar(name, fr), null);
                    en.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, 1000);
                    en.freeData.Cat = FreeEntityConstant.DeadBox;

                    FreeItemInfo itemInfo = FreeItemConfig.GetItemInfo(ip.GetKey().GetKey());

                    if (itemInfo.cat == (int)ECategory.Weapon)
                    {
                        int key = CarryClipUtil.GetWeaponKey(ii.name, fd);
                        if (key >= 1 && key <= 3)
                        {
                            CarryClipUtil.AddCurrentClipToBag(key, fd, fr);
                        }
                    }

                    en.AddFlagImmutability(fr.GameContext.session.currentTimeObject.CurrentTime);

                    SimpleItemInfo info = new SimpleItemInfo(realName, itemInfo.cat, itemInfo.id, ip.GetCount(), en.entityKey.Value.EntityId);
                    en.freeData.Value = SingletonManager.Get<DeadBoxParser>().ToString(info);
                    en.freeData.IntValue = entityId;

                    en.isFlagSyncNonSelf = true;

                    itemAdded++;
                }
            }

            return itemAdded;
        }

        private void CreateItemFromItemDrop(IEventArgs fr, Vector3 p, ItemDrop drop, string realName)
        {
            FreeMoveEntity en = fr.GameContext.freeMove.CreateEntity();
            en.AddEntityKey(new EntityKey(fr.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
            en.AddPosition();
            en.position.Value = new Vector3(p.x, p.y, p.z);

            en.AddFreeData(FreeUtil.ReplaceVar(name, fr), null);
            en.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, 1000);
            en.freeData.Cat = FreeEntityConstant.DropBox;
            if (drop.cat == (int) ECategory.Weapon && SingletonManager.Get<WeaponResourceConfigManager>().IsArmors(drop.id))
            {
                drop.count = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(drop.id).Durable;
            }

            en.AddFlagImmutability(fr.GameContext.session.currentTimeObject.CurrentTime);

            SimpleItemInfo info = new SimpleItemInfo(realName, drop.cat, drop.id, drop.count, en.entityKey.Value.EntityId);
            en.freeData.Value = SingletonManager.Get<DeadBoxParser>().ToString(info);

            en.isFlagSyncNonSelf = true;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.CreateBoxItemAction;
        }
    }

}
