using App.Shared;
using App.Shared.Configuration;
using Core.Components;
using Core.GameModule.Interface;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.SceneManagement
{
    public class ClientAutoWorldShiftRenderSystem:ILateUpdateSystem
    {
        private readonly Contexts _contexts;
        private readonly float _worldShiftDistance;

        public ClientAutoWorldShiftRenderSystem(Contexts contexts)
        {
            _contexts = contexts;
            int mapId = SingletonManager.Get<MapConfigManager>().SceneParameters.Id;
            _worldShiftDistance = mapId == 0 ? SharedConfig.TestWorldShiftDistance : SharedConfig.WorldShiftDistance;
        }


        private float _lastTime = 0;
        public void OnLateUpdate()
        {
            var time = Time.time;
            if (time - _lastTime > 10 && SharedConfig.WorldShiftEnable)
            {
                var position = Camera.main.transform.position;

                if (position.y < 1000 && (Mathf.Abs(position.x) > _worldShiftDistance || Mathf.Abs(position.z) > _worldShiftDistance))
                {
                    SingletonManager.Get<WorldShiftManager>()
                        .SetOrgin(WorldOrigin.Origin + new Vector3(position.x, 0, position.z));
                }

                _lastTime = time;
            }
        }

        
    }
}