using Core.Configuration;
using Core.GameModule.Interface;
using Core.GameTime;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using UnityEngine;

namespace App.Server.GameModules.SceneObject
{
    public class ServerSceneObjectThrowingSystem : IGamePlaySystem
    {
        IGroup<SceneObjectEntity> _throwingSceneObjectGroup;
        private ICurrentTime _currentTime;
        private List<SceneObjectEntity> _removeEntityList = new List<SceneObjectEntity>();
        private RuntimeGameConfig _gameConfig;

        public ServerSceneObjectThrowingSystem(SceneObjectContext sceneObjectContext, ICurrentTime currentTime, RuntimeGameConfig config)
        {
            _throwingSceneObjectGroup = sceneObjectContext.GetGroup(SceneObjectMatcher.Throwing);
            _currentTime = currentTime;
            _gameConfig = config; 
        }

        public void OnGamePlay()
        {
            _removeEntityList.Clear();
            foreach(SceneObjectEntity sceneObjectEntiy in _throwingSceneObjectGroup)
            {
                if(sceneObjectEntiy.throwing.Time < 1)
                {
                    sceneObjectEntiy.throwing.Time = _currentTime.CurrentTime;
                    continue;
                }
                var interval = _currentTime.CurrentTime - sceneObjectEntiy.throwing.Time;
                var secondInterval = interval * 0.001f;
                sceneObjectEntiy.throwing.Time = _currentTime.CurrentTime;
                sceneObjectEntiy.ReplaceFlagImmutability(_currentTime.CurrentTime);
                var velocity = sceneObjectEntiy.throwing.Velocity * secondInterval;
                velocity.y += _gameConfig.WeaponDropVSpeed * secondInterval;
                var lastPos = sceneObjectEntiy.position.Value;
                var newPos = lastPos + velocity;
                sceneObjectEntiy.throwing.Velocity = velocity;
                Debug.LogFormat("Velotcity is {0} interval {1} offset {2} pos {3}", velocity, interval, (newPos - lastPos).ToStringExt(), 
                    sceneObjectEntiy.position.Value.ToStringExt());
                RaycastHit hitInfo;
                if(Physics.Linecast(lastPos, newPos, out hitInfo, UnityLayerManager.GetLayerMask(EUnityLayerName.Default) | UnityLayerManager.GetLayerMask(EUnityLayerName.Terrain)))
                {
                    sceneObjectEntiy.position.Value = hitInfo.point;
                    Debug.LogFormat("hit {0} in {1}", hitInfo.transform.name, hitInfo.point);
                    _removeEntityList.Add(sceneObjectEntiy);
                }
                else
                {
                    sceneObjectEntiy.position.Value = newPos;
                }
            }
            foreach(var sceneObjectentity in _removeEntityList)
            {
                if(null != sceneObjectentity && sceneObjectentity.isEnabled && sceneObjectentity.hasThrowing)
                {
                    sceneObjectentity.RemoveThrowing();
                }
            }
        }
    }
}
