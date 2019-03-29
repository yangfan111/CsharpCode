using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.util;
using App.Server.GameModules.GamePlay.Free.map.position;
using UnityEngine;
using com.cpkf.yyjd.tools.util.math;
using Assets.XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class RefreshItemAtPosAction : AbstractGameAction
    {
        private string cat;
        private string count;
        private IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            ItemDrop[] list = FreeItemDrop.GetDropItems(FreeUtil.ReplaceVar(cat, args), FreeUtil.ReplaceInt(count, args));

            if (list != null)
            {
                Vector3 p = UnityPositionUtil.ToVector3(pos.Select(args));
                foreach (ItemDrop drop in list)
                {
                    args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.
                        CreateSimpleEquipmentEntity((ECategory)drop.cat, drop.id, drop.count, new Vector3(p.x + RandomUtil.Random(-100, 100) / 100f, p.y, p.z + RandomUtil.Random(-100, 100) / 100f));
                }
            }

        }
    }
}
