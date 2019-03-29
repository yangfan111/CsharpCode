using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.DebugSystem
{
    public struct MapObjectInfo
    {
        public Vector3 Position;
        public string Name;
    }

    public class MapObjectDebugInfo
    {
        public Vector3 OriginPosition;
        public List<MapObjectInfo> DebugInfos = new List<MapObjectInfo>();
    }

    public class MapObjectDebugInfoSystem : AbstractDebugInfoSystem<MapObjectDebugInfoSystem, MapObjectDebugInfo>
    {

        private Contexts _contexts;
        public MapObjectDebugInfoSystem(Contexts conexts)
        {
            _contexts = conexts;
        }
        protected override MapObjectDebugInfo GetDebugInfo(object param)
        {
            
            Vector3 position;
            if (SharedConfig.IsServer)
            {
                position = (Vector3) param;
            }
            else
            {
                var playerEntity = _contexts.player.flagSelfEntity;
                if (playerEntity == null || !playerEntity.hasPosition)
                    return null;
                position = playerEntity.position.Value;
            }


            var debugInfos = new MapObjectDebugInfo();
            debugInfos.OriginPosition = position;
            float radius = 10f;

            foreach (var MapObj in _contexts.mapObject.GetEntities())
            {
                if (MapObj == null) continue;
                if ((MapObj.position.Value - position).magnitude <= radius)
                {
                    var objInfo = new MapObjectInfo()
                    {
                        Position = MapObj.position.Value,
                        Name = MapObj.hasRawGameObject ? MapObj.rawGameObject.Value.name : "NoGameObject",
                    };
                    debugInfos.DebugInfos.Add(objInfo);
                }
            }

            return debugInfos;
        }
    }
}
