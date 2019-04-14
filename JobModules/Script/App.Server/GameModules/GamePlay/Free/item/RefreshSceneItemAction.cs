using App.Shared.GameModules.Vehicle;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using Shared.Scripts.MapConfigPoint;
using System;
using System.Collections.Generic;
using UnityEngine;

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
                if (MapConfigPoints.current != null)
                {
                    foreach (MapConfigPoints.ID_Point point in MapConfigPoints.current.IDPints)
                    {
                        List<ItemDrop> list = FreeItemDrop.GetDropItems(point.ID, args.GameContext.session.commonSession.RoomInfo.MapId);

                        if (list.Count > 0)
                        {
                            TimerGameAction timer = new TimerGameAction();
                            timer.time = "200";
                            timer.count = list.Count.ToString();
                            timer.SetAction(new RefreshItemAction(list));
                            timer.Act(args);
                        }
                    }
                }
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
                    if (drop.cat == (int) ECategory.Vehicle)
                    {
                        VehicleEntityUtility.CreateNewVehicle(args.GameContext.vehicle, drop.id, args.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), GetGround(drop.pos));
                        break;
                    }
                    else
                    {
                        args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleEquipmentEntity((ECategory)drop.cat, drop.id, drop.count, GetGround(drop.pos));
                    }

                    List<ItemDrop> extra = FreeItemDrop.GetExtraItems(drop);
                    foreach (ItemDrop e in extra)
                    {
                        args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleEquipmentEntity((ECategory)e.cat, e.id, e.count, GetGround(drop.pos));
                    }
                    if (index >= list.Count)
                    {
                        break;
                    }
                }
            }
        }

        private Vector3 GetGround(Vector3 v)
        {
            Vector3 fromV = v;
            Vector3 toV = new Vector3(v.x, -10000, v.z);

            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));
            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo);

            if (hited)
            {
                return hitInfo.point;
            }
            else
            {
                return v;
            }
        }
    }
}
