using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using App.Server.GameModules.GamePlay.Free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using Core.Utils;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class CreateSceneObjectAction : AbstractPlayerAction
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
                Ray ray = new Ray(player.position.Value, dropPos);
                if(Physics.Raycast(ray, out hit, Vector3.Distance(player.position.Value, dropPos), UnityLayers.SceneCollidableLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hit.point, Vector3.down, out vhit, 10000, UnityLayers.SceneCollidableLayerMask))
                    {
                        dropPos = vhit.point;
                    }
                    else
                    {
                        dropPos = hit.point;
                    }
                }
            }
            SceneObjectEntity entity = (SceneObjectEntity) args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.CreateSimpleEquipmentEntity(
                (Assets.XmlConfig.ECategory)FreeUtil.ReplaceInt(cat, args), FreeUtil.ReplaceInt(id, args), FreeUtil.ReplaceInt(count, args), dropPos);
            if(entity != null && !string.IsNullOrEmpty(time))
            {
                entity.AddLifeTime(DateTime.Now, args.GetInt(time));
            }
            Debug.LogFormat("create item {0},{1},{2}", FreeUtil.ReplaceInt(cat, args), FreeUtil.ReplaceInt(id, args), FreeUtil.ReplaceInt(count, args));
        }
    }
}
