using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CameraControl;
using Core.GameModule.Interface;
using Core.GameTime;
using App.Shared.SceneTriggerObject;
using Entitas;
using UnityEngine;

namespace App.Shared.GameModules.SceneObject
{
    public class DoorRotateSystem : IGamePlaySystem
    {
        private  const float RotateSpeed = 180f; //90 degree/seconds

        private IGroup<MapObjectEntity> _activeDoors;
        private List<MapObjectEntity> _deactiveDoors = new List<MapObjectEntity>();
        private DateTime _lastTime = DateTime.MinValue;
        private ITriggerObjectListener _doorListener;
        private ICurrentTime _currentTime;
        public DoorRotateSystem(Contexts contexts, ITriggerObjectListener listener = null)
        {
            _doorListener = listener;
            _activeDoors = contexts.mapObject.GetGroup(MapObjectMatcher.DoorRotate);
           
                _currentTime = contexts.session.currentTimeObject;
            

        }

        public void OnGamePlay()
        {
            var currentTime = DateTime.Now;
            if (_lastTime == DateTime.MinValue)
            {
                _lastTime = currentTime;
                return;
            }

            var deltaTime = (float) (currentTime - _lastTime).TotalMilliseconds * 0.001f;
            _lastTime = currentTime;

            foreach (var door in _activeDoors.GetEntities())
            {
                var rotData = door.doorRotate;
                var current = rotData.Current;
                var from = rotData.From;
                var to = rotData.To;

                var data = door.doorData;

                var transform = door.rawGameObject.Value.transform;
                if (to > from)
                {
                    current += RotateSpeed * deltaTime;
                    current = Mathf.Clamp(current, from, to);
                }
                else
                {
                    current -= RotateSpeed * deltaTime;
                    current = Mathf.Clamp(current, to, from);
                }

                var eulerAngles = transform.localEulerAngles;
                eulerAngles.y = current;
                transform.localEulerAngles = eulerAngles;

                if (current.Equals(to))
                {
                    _deactiveDoors.Add(door);
                }
                else
                {
                    rotData.Current = current;
                }

                data.Rotation = current < 0 ? current + 360f : current;
                if(door.hasFlagImmutability)
                door.flagImmutability.LastModifyServerTime = _currentTime.CurrentTime;
            }

            foreach (var door in _deactiveDoors)
            {
                door.doorData.State = door.doorRotate.EndState;
                door.RemoveDoorRotate();
                if(door.hasFlagImmutability)
                    door.flagImmutability.LastModifyServerTime = _currentTime.CurrentTime;
            }

            _deactiveDoors.Clear();
        }
    }
}
