using App.Server.GameModules.GamePlay.Free.map.position;
using App.Shared.GameModules.Player;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.util;
using Core;
using Core.Free;
using Core.Utils;
using System;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class CreateSceneObjectAction : AbstractPlayerAction, IRule
    {
        public string cat;
        public string id;
        public string count;
        public string time;
        public IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);
            Vector3 dropPos = UnityPositionUtil.ToVector3(pos.Select(args));
            if (player != null)
            {
                RaycastHit hit;
                Vector3 throwPos = player.GetHandWeaponPosition();
                Ray ray = new Ray(throwPos, dropPos - throwPos);
                if(Physics.Raycast(ray, out hit, Vector3.Distance(throwPos, dropPos) - 0.01f, UnityLayers.PickupObstacleLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hit.point + new Vector3(0, 0.1f, 0), Vector3.down, out vhit, 100, UnityLayers.PickupObstacleLayerMask))
                    {
                        dropPos = vhit.point;
                    }
                    else
                    {
                        dropPos = hit.point;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(dropPos + new Vector3(0, 0.1f, 0), Vector3.down, out vhit, 100, UnityLayers.PickupObstacleLayerMask))
                    {
                        dropPos = vhit.point;
                    }
                }
            }
            SceneObjectEntity entity = (SceneObjectEntity) args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleObjectEntity(
                (Assets.XmlConfig.ECategory)FreeUtil.ReplaceInt(cat, args), FreeUtil.ReplaceInt(id, args), FreeUtil.ReplaceInt(count, args), dropPos, FreeUtil.ReplaceInt(count, args));
            if(entity != null && !string.IsNullOrEmpty(time))
            {
                entity.AddLifeTime(DateTime.Now, args.GetInt(time));
            }
            Debug.LogFormat("create item {0},{1},{2}", FreeUtil.ReplaceInt(cat, args), FreeUtil.ReplaceInt(id, args), FreeUtil.ReplaceInt(count, args));
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.CreateSceneObjectAction;
        }

        public static void CreateDropItem(PlayerEntity player, int cat, int id, int count, ISceneObjectEntityFactory factory)
        {
            var dropPos = player.position.Value + player.characterContoller.Value.transform.forward * 2;
            if (player != null)
            {
                RaycastHit hit;
                Vector3 throwPos = player.GetHandWeaponPosition();
                Ray ray = new Ray(throwPos, dropPos - throwPos);
                if(Physics.Raycast(ray, out hit, Vector3.Distance(throwPos, dropPos) - 0.01f, UnityLayers.PickupObstacleLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hit.point + new Vector3(0, 0.1f, 0), Vector3.down, out vhit, 100, UnityLayers.PickupObstacleLayerMask))
                    {
                        dropPos = vhit.point;
                    }
                    else
                    {
                        dropPos = hit.point;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(dropPos + new Vector3(0, 0.1f, 0), Vector3.down, out vhit, 100, UnityLayers.PickupObstacleLayerMask))
                    {
                        dropPos = vhit.point;
                    }
                }
            }
            factory.CreateSimpleObjectEntity((ECategory) cat, id, count, dropPos, count);
        }
    }
}
