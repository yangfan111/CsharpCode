using App.Shared.Audio;
using Core;
using Core.GameModule.Interface;
using Entitas;
using System;

namespace App.Client.GameModules.Sound
{
    public class ClientBombSoundSystem : IGamePlaySystem
    {
        private const long INTERVAL1 = 1000L;
        private const long INTERVAL2 = 600L;
        private const long INTERVAL3 = 300L;

        private Contexts _contexts;
        private IGroup<SceneObjectEntity> soGroup;
        private IGroup<FreeMoveEntity> fmGroup;

        public ClientBombSoundSystem(Contexts contexts)
        {
            _contexts = contexts;
            soGroup = contexts.sceneObject.GetGroup(SceneObjectMatcher.BombSound);
            fmGroup = contexts.freeMove.GetGroup(FreeMoveMatcher.BombSound);
        }

        public void OnGamePlay()
        {
            var now = DateTime.Now.Ticks / 10000L;
            foreach (SceneObjectEntity entity in soGroup)
            {
                if (now - entity.bombSound.LastSoundTime >= INTERVAL1)
                {
                    GameAudioMedia.PlayUniqueEventAudio(entity.position.Value, EAudioUniqueId.C4_Alarm);
                    entity.bombSound.LastSoundTime = now;
                }
            }

            foreach (FreeMoveEntity entity in fmGroup)
            {
                var interval = INTERVAL3;
                if (now - entity.bombSound.CreateTime <= 24000L)
                {
                    interval = INTERVAL1;
                }
                else if (now - entity.bombSound.CreateTime <= 30000L)
                {
                    interval = INTERVAL2;
                }

                if (now - entity.bombSound.LastSoundTime >= interval)
                {
                    GameAudioMedia.PlayUniqueEventAudio(entity.position.Value, EAudioUniqueId.C4_Alarm);
                    entity.bombSound.LastSoundTime = now;
                }
            }
        }
    }
}
