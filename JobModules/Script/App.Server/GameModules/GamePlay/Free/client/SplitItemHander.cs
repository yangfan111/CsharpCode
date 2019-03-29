using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.para;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.item;
using gameplay.gamerule.free.rule;
using UnityEngine;
using App.Server.GameModules.GamePlay.Free.client;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.XmlConfig;
using App.Server.GameModules.GamePlay.Free.item.config;
using com.wd.free.item;
using Utils.Configuration;
using XmlConfig;
using App.Server.GameModules.GamePlay.Free.item;
using Assets.App.Server.GameModules.GamePlay.Free;
using App.Server.GameModules.GamePlay.Free.chicken;
using com.wd.free.skill;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class SplitItemHandler : ParaConstant, IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.SplitItem;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            FreeData fd = (FreeData)player.freeData.FreeData;

            room.FreeArgs.TempUse(PARA_PLAYER_CURRENT, fd);

            string key = message.Ss[0];
            int count = message.Ins[0];

            if (key.StartsWith(ChickenConstant.BagDefault))
            {
                ItemPosition ip = FreeItemManager.GetItemPosition(room.FreeArgs, key, fd.GetFreeInventory().GetInventoryManager());
                FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());

                if (ip.GetCount() > count)
                {
                    ip.SetCount(ip.GetCount() - count);
                    ip.GetInventory().GetInventoryUI().ReDraw((ISkillArgs)room.FreeArgs, ip.GetInventory(), true);
                }
                else
                {
                    ip.GetInventory().RemoveItem((ISkillArgs)room.FreeArgs, ip);
                }

                room.RoomContexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleEquipmentEntity(
                        (Assets.XmlConfig.ECategory)info.cat,
                        info.id,
                        count,
                        fd.Player.position.Value);
            }

            room.FreeArgs.Resume(PARA_PLAYER_CURRENT);
        }

    }
}
