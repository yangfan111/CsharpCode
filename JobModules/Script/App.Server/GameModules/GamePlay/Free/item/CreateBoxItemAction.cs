using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Server.GameModules.GamePlay.Free.map.position;
using App.Shared;
using App.Shared.GameModules.GamePlay.Free;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.map.position;
using com.wd.free.util;
using Core.EntityComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class CreateBoxItemAction : AbstractGameAction
    {
        private string name;
        private string id;
        private string cat;
        private string count;
        private IPosSelector pos;
        private int type;

        public override void DoAction(IEventArgs args)
        {
       
            Vector3 p = UnityPositionUtil.ToVector3(pos.Select(args));
            string realName = FreeUtil.ReplaceVar(name, args);
            var groupEntity = args.GameContext.freeMove.CreateEntity();
            groupEntity.AddEntityKey(new EntityKey(args.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
            groupEntity.AddPosition(new Vector3(p.x, p.y, p.z));
            groupEntity.AddFreeData("", null);
            groupEntity.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, 1000);
            groupEntity.freeData.Value = "";
            if (type == 1)
            {
                groupEntity.freeData.Cat = FreeEntityConstant.DeadBoxGroup;
            }
            else if (type == 2)
            {
                groupEntity.freeData.Cat = FreeEntityConstant.DropBoxGroup;
            }
            groupEntity.isFlagSyncNonSelf = true;

            if (string.IsNullOrEmpty(id))
            {
                ItemDrop[] list = FreeItemDrop.GetDropItems(FreeUtil.ReplaceVar(cat, args), FreeUtil.ReplaceInt(count, args));
                foreach (ItemDrop drop in list)
                {
                    CreateItemFromItemDrop(args, p, drop, realName);
                    List<ItemDrop> extra = FreeItemDrop.GetExtraItems(drop);
                    foreach (ItemDrop e in extra)
                    {
                        CreateItemFromItemDrop(args, p, e, realName);
                    }
                }
            }
            else
            {
                PlayerEntity player = args.GameContext.player.GetEntityWithEntityKey(new EntityKey(FreeUtil.ReplaceInt(id, args), (short)EEntityType.Player));
                if (player != null)
                {
                    groupEntity.freeData.IntValue = player.entityKey.Value.EntityId;
                    realName = player.playerInfo.PlayerName;
                    FreeData fd = ((FreeData)player.freeData.FreeData);
                    foreach (string inv in fd.GetFreeInventory().GetInventoryManager().GetInventoryNames())
                    {
                        ItemInventory ii = fd.GetFreeInventory().GetInventoryManager().GetInventory(inv);
                        if (ii.name != ChickenConstant.BagDefault)
                        {
                            CreateItemFromInventory(args, fd, ii, p, realName);
                        }
                    }
                    CreateItemFromInventory(args, fd, fd.GetFreeInventory().GetInventoryManager().GetDefaultInventory(), p, realName);
                }
            }
        }

        private void CreateItemFromInventory(IEventArgs fr, FreeData fd, ItemInventory ii, Vector3 p, string realName)
        {
            if (ii != null && ii.name != "belt" && ii.name != ChickenConstant.BagGround)
            {
                foreach (ItemPosition ip in ii.GetItems())
                {
                    FreeMoveEntity en = fr.GameContext.freeMove.CreateEntity();
                    en.AddEntityKey(new EntityKey(fr.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
                    en.AddPosition(new Vector3(p.x, p.y, p.z));

                    en.AddFreeData(FreeUtil.ReplaceVar(name, fr), null);
                    en.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, 1000);
                    if (type == 1)
                    {
                        en.freeData.Cat = FreeEntityConstant.DeadBox;
                    }
                    else if (type == 2)
                    {
                        en.freeData.Cat = FreeEntityConstant.DropBox;
                    }

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

                    en.isFlagSyncNonSelf = true;
                }
            }
        }

        private void CreateItemFromItemDrop(IEventArgs fr, Vector3 p, ItemDrop drop, string realName)
        {
            FreeMoveEntity en = fr.GameContext.freeMove.CreateEntity();
            en.AddEntityKey(new EntityKey(fr.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
            en.AddPosition(new Vector3(p.x, p.y, p.z));

            en.AddFreeData(FreeUtil.ReplaceVar(name, fr), null);
            en.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, 1000);
            if (type == 1)
            {
                en.freeData.Cat = FreeEntityConstant.DeadBox;
            }
            else if (type == 2)
            {
                en.freeData.Cat = FreeEntityConstant.DropBox;
            }

            en.AddFlagImmutability(fr.GameContext.session.currentTimeObject.CurrentTime);

            SimpleItemInfo info = new SimpleItemInfo(realName, drop.cat, drop.id, drop.count, en.entityKey.Value.EntityId);
            en.freeData.Value = SingletonManager.Get<DeadBoxParser>().ToString(info);

            en.isFlagSyncNonSelf = true;
        }
    }

}
