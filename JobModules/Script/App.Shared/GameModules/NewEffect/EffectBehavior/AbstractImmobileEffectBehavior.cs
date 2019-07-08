using App.Shared.Audio;
using Core.ObjectPool;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared
{
    public struct AudioEffectData
    {
        public int AudioClientEffectArg1;
        public int AudioClientEffectArg2;
        public AudioClientEffectType audioClientEffectType;

        public AudioEffectData(int audioClientEffectArg1, int audioClientEffectArg2,
                               AudioClientEffectType audioClientEffectType)
        {
            AudioClientEffectArg1      = audioClientEffectArg1;
            AudioClientEffectArg2      = audioClientEffectArg2;
            this.audioClientEffectType = audioClientEffectType;
        }

        public void PlayEffectAudio(ClientEffectEmitter emitter, Vector3 Position)
        {
            switch (audioClientEffectType)
            {
                case AudioClientEffectType.WeaponEnvHit:
                    GameAudioMedia.PlayHitEnvironmentAudio((AudioGrp_HitMatType) AudioClientEffectArg1,
                        AudioClientEffectArg2, emitter.nodeObject.AudioMono);
                    // AudioEntry.Logger.Info("Real Play Time:" +(_playerContext.flagSelfEntity.time.ClientTime - GlobalConst.BeforeAttackTime));
                    break;
                case AudioClientEffectType.WeaponPlayerHit:

                    GameAudioMedia.PlayHitPlayerAudio((EBodyPart) AudioClientEffectArg1, AudioClientEffectArg2,
                        Position);
                    break;
                case AudioClientEffectType.BulletDrop:
                    GameAudioMedia.PlayBulletDropAudio(AudioClientEffectArg1,
                        (AudioGrp_FootMatType) AudioClientEffectArg2, Position);
                    break;
                case AudioClientEffectType.ThrowExplosion:
                    GameAudioMedia.PlayEventAudio(AudioClientEffectArg1, Position);
                    break;
            }
        }
    }

    public abstract class AbstractImmobileEffectBehavior : EffectBehaviorAdapter
    {
        protected AudioEffectData AudioData;
        protected Transform Parent;
        protected Vector3 Position;


        public void Initialize(Vector3 position, AudioEffectData audioEffectData, Transform parent = null)
        {
            Position    = position;
            NeedRecycle = false;
            AudioData   = audioEffectData;
            Parent      = parent;
        }


        protected void Initialize(Vector3 position, Transform parent = null)
        {
            Position                        = position;
            NeedRecycle                     = false;
            AudioData.audioClientEffectType = AudioClientEffectType.Mute;
            Parent                          = parent;
        }

        protected abstract void SetPosition(ClientEffectEmitter emitter);

        public override void PlayEffect(ClientEffectEmitter emitter, GameObject effectGo)
        {
            effectGo.SetActive(true);
            if (Parent)
            {
                emitter.nodeObject.transform.SetParent(Parent);
            }

            AudioData.PlayEffectAudio(emitter, Position);
            SetPosition(emitter);
        }

        protected override void Free(ClientEffectEmitter emitter)
        {
            if (Parent)
                emitter.nodeObject.transform.SetParent(emitter.PoolFolder);
            ObjectAllocatorHolder<AbstractImmobileEffectBehavior>.Free(this);
        }
    }
}