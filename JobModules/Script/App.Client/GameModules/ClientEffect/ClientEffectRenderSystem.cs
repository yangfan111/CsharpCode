using App.Client.GameModules.Player;
using App.Shared;
using App.Shared.Audio;
using Core.Utils;
using Entitas;
using WeaponConfigNs;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectRenderSystem : AbstractRenderSystem<ClientEffectEntity>
    {
        private LoggerAdapter _logger = new LoggerAdapter("ClientEffectRenderSystem");
        private PlayerContext _playerContext;

        public ClientEffectRenderSystem(Contexts context) : base(context)
        {
            _playerContext = context.player;
        }

        protected override IGroup<ClientEffectEntity> GetIGroup(Contexts contexts)
        {
            return contexts.clientEffect.GetGroup(ClientEffectMatcher.AllOf(ClientEffectMatcher.Logic,
            ClientEffectMatcher.Assets, ClientEffectMatcher.Position));
        }

        protected override bool Filter(ClientEffectEntity entity)
        {
            return entity.assets.IsLoadSucc;
        }

        protected override void OnRender(ClientEffectEntity entity)
        {
            if (!entity.assets.IsInitialized)
            {
                Initialize(entity);
            }
            else
            {
                entity.logic.EffectLogic.Render(entity);
            }
        }

        private void Initialize(ClientEffectEntity entity)
        {
            entity.assets.IsInitialized = true;
            entity.logic.EffectLogic.Initialize(entity);
            if (entity.hasAudio)
            {
                var audioEfcType = (AudioClientEffectType) entity.audio.AudioClientEffectType;
                switch (audioEfcType)
                {
                    // case AudioClientEffectType.WeaponEnvHit: 
                    //     // GameAudioMedia.PlayHitEnvironmentAudio((AudioGrp_HitMatType)entity.audio.AudioClientEffectArg1,
                    //     // entity.audio.AudioClientEffectArg2,entity.position.Value);
                    //     // _logger.Info("Real Play Time:"+(_playerContext.flagSelfEntity.time.ClientTime - GlobalConst.BeforeAttackTime));  
                    //     break;
                    case AudioClientEffectType.WeaponPlayerHit:

                        GameAudioMedia.PlayHitPlayerAudio((EBodyPart) entity.audio.AudioClientEffectArg1,
                        entity.audio.AudioClientEffectArg2, entity.position.Value);
                        break;
                    case AudioClientEffectType.BulletDrop:
                        GameAudioMedia.PlayBulletDropAudio(entity.audio.AudioClientEffectArg1,
                        (AudioGrp_FootMatType) entity.audio.AudioClientEffectArg2, entity.position.Value);
                        break;
                    case AudioClientEffectType.ThrowExplosion:
                        GameAudioMedia.PlayEventAudio(entity.audio.AudioClientEffectArg1, entity.position.Value);
                        break;
                }
            }
        }
    }
}