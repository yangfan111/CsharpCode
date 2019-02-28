using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Player;
using Core.CameraControl;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class DoorPlaybackSystem :AbstractPlayerBackSystem<MapObjectEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(DoorPlaybackSystem));
        private ClientDoorListener _doorListener;

        private IGroup<MapObjectEntity> _doors;
        
        public DoorPlaybackSystem(Contexts contexts) : base(contexts)
        {
            _doorListener = new ClientDoorListener(contexts);
          
        }

        protected override IGroup<MapObjectEntity> GetIGroup(Contexts contexts)
        {
            return contexts.mapObject.GetGroup(MapObjectMatcher.AllOf(MapObjectMatcher.DoorData, MapObjectMatcher.RawGameObject));
        }

        protected override bool Filter(MapObjectEntity entity)
        {
            return !(entity.hasFlagImmutability && entity.flagImmutability.NeedSkipUpdate);
        }

        protected override void OnPlayBack(MapObjectEntity door)
        {
            if (door.rawGameObject.Value == null)
                return;
                
            var transform = door.rawGameObject.Value.transform;
            var rot = door.doorData.Rotation;
            var eulerAngles = transform.localEulerAngles;
            if (!rot.Equals(eulerAngles.y))
            {
                eulerAngles.y = rot;
                transform.localEulerAngles = eulerAngles;
                _logger.DebugFormat("Door Entity Id {0} Rot is {1}", door.entityKey.Value.EntityId, rot);
            }
        }

        protected override void BeforeOnPlayback()
        {
            _doorListener.Update();
        }
    }
}
