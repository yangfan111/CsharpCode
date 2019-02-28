using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using gameplay.gamerule.free.item;
using com.wd.free.item;
using com.wd.free.unit;
using App.Server.GameModules.GamePlay.Free.map.position;
using App.Server.GameModules.GamePlay.Free.action;
using Assets.XmlConfig;
using UnityEngine;
using gameplay.gamerule.free.action;
using com.cpkf.yyjd.tools.util.collection;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class RefreshSceneItemAction : AbstractGameAction
    {
        private bool remove;

        public override void DoAction(IEventArgs args)
        {
            if (remove)
            {
                args.GameContext.sceneObject.DestroyAllEntities();
            }
            else
            {
                Accumulator<string> acc = new Accumulator<string>();
                if (MapConfigPoints.current != null)
                {
                    foreach (MapConfigPoints.ID_Point point in MapConfigPoints.current.IDPints)
                    {
                        List<ItemDrop> list = FreeItemDrop.GetDropItems(point.ID);

                        foreach (ItemDrop item in list)
                        {
                            acc.AddKey(string.Format("{0}_{1}",item.cat , item.id), 1);
                        }

                        TimerGameAction timer = new TimerGameAction();
                        timer.time = "200";
                        timer.count = list.Count.ToString();
                        timer.SetAction(new RefreshItemAction(list));

                        timer.Act(args);
                    }
                }

                List<string> items = new List<string>();
                foreach (string key in acc.KeysSortedByValue())
                {
                    items.Add(string.Format("{0}={1}",key ,acc.GetCount(key)));
                }
                Debug.LogFormat("items:\n{0}", string.Join("\n", items.ToArray()));
            }
        }
    }

    class RefreshItemAction : AbstractGameAction
    {
        private int index;
        private List<ItemDrop> list;

        public RefreshItemAction(List<ItemDrop> list)
        {
            this.index = 0;
            this.list = list;
        }
        public override void DoAction(IEventArgs args)
        {
            if (index < list.Count)
            {
                for (int i = 0; i < 20; i++)
                {
                    ItemDrop drop = list[index++];
                    args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.
                                        CreateSimpleEquipmentEntity((ECategory)drop.cat, drop.id, drop.count, new Vector3(drop.pos.x, drop.pos.y, drop.pos.z));

                    List<ItemDrop> extra = FreeItemDrop.GetExtraItems(drop);
                    foreach (ItemDrop e in extra)
                    {
                        args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.
                        CreateSimpleEquipmentEntity((ECategory)e.cat, e.id, e.count, new Vector3(drop.pos.x, drop.pos.y, drop.pos.z));
                    }
                    if (index >= list.Count)
                    {
                        break;
                    }
                }
            }
        }
    }
}
