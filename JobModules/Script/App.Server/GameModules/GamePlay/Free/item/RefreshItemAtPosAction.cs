﻿using App.Server.GameModules.GamePlay.Free.map.position;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.util;
using Core.Free;
using System;
using UnityEngine;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class RefreshItemAtPosAction : AbstractGameAction, IRule
    {
        private string cat;
        private string count;
        private IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            ItemDrop[] list = SingletonManager.Get<FreeItemDrop>().GetDropItems(FreeUtil.ReplaceVar(cat, args), FreeUtil.ReplaceInt(count, args), args.GameContext.session.commonSession.RoomInfo.MapId);

            if (list != null)
            {
                Vector3 p = UnityPositionUtil.ToVector3(pos.Select(args));
                foreach (ItemDrop drop in list)
                {
                    args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.
                        CreateSimpleObjectEntity((ECategory)drop.cat, drop.id, drop.count, new Vector3(p.x + RandomUtil.Random(-100, 100) / 100f, p.y, p.z + RandomUtil.Random(-100, 100) / 100f));
                }
            }

        }

        public int GetRuleID()
        {
            return (int)ERuleIds.RefreshItemAtPosAction;
        }
    }
}
