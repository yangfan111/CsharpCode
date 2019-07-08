using App.Server.GameModules.GamePlay.Free.item;
using App.Shared.GameModules.Vehicle;
using Assets.XmlConfig;
using Core.GameModule.Interface;
using Core.Utils;
using Shared.Scripts.MapConfigPoint;
using Sharpen;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace App.Server
{
    public class ServerEntitiesInitSystem : IGamePlaySystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerEntitiesInitSystem));

        private Contexts _contexts;
        private int index;
        private bool loadConfig;
        private bool loadComplete;
        private List<ItemDrop> dropList;

        public ServerEntitiesInitSystem(Contexts contexts)
        {
            _contexts = contexts;
            index = 0;
            loadConfig = false;
            loadComplete = false;
            dropList = new List<ItemDrop>();
        }

        public void OnGamePlay()
        {
            if (!loadConfig && dropList.IsEmpty())
            {
                foreach (MapConfigPoints.ID_Point point in MapConfigPoints.current.IDPints)
                {
                    dropList.AddRange(SingletonManager.Get<FreeItemDrop>().GetDropItems(point.ID, _contexts.session.commonSession.RoomInfo.MapId));
                }
                loadConfig = true;
            }

            if (loadConfig && dropList.IsEmpty())
                return;

            if (_contexts.player.count > 0 && !loadComplete)
            {
                if (index >= dropList.Count)
                {
                    loadComplete = true;
                    return;
                }
                for (int i = 0; i < 20; i++)
                {
                    ItemDrop drop = dropList[index++];
                    if (drop.cat == (int) ECategory.Vehicle)
                    {
                        VehicleEntityUtility.CreateNewVehicle(_contexts.vehicle, drop.id, _contexts.session.commonSession.EntityIdGenerator.GetNextEntityId(), GetGround(drop.pos));
                        break;
                    }
                    else
                    {
                        _contexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity((ECategory)drop.cat, drop.id, drop.count, GetGround(drop.pos));
                        List<ItemDrop> extra = SingletonManager.Get<FreeItemDrop>().GetExtraItems(drop);
                        foreach (ItemDrop e in extra)
                        {
                            _contexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity((ECategory)e.cat, e.id, e.count, GetGround(drop.pos));
                        }
                    }

                }
            }
        }

        private Vector3 GetGround(Vector3 v)
        {
            Vector3 fromV = v + new Vector3(0, 0.1f, 0);
            Vector3 toV = new Vector3(v.x, -10000, v.z);

            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));
            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo, UnityLayers.PickupObstacleLayerMask);

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