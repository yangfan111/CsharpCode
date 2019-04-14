using App.Client.GameModules.Player;
using App.Shared;
using App.Shared.EntityFactory;
using Core.GameModule.Interface;
using Entitas;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectRenderSystem : AbstractRenderSystem<ClientEffectEntity>
    {
        public ClientEffectRenderSystem(Contexts context) : base(context)
        {
            _playerContext = context.player;
        }

        private PlayerContext _playerContext;
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
            var comp = entity.logic;
            if (!entity.assets.IsInitialized)
            {
                entity.assets.IsInitialized = true;
                comp.EffectLogic.Initialize(entity);
                if (entity.hasAudio)
                {
              
                    var audioEfcType = (AudioClientEffectType) entity.audio.AudioClientEffectType;
                    switch (audioEfcType)
                    {
                        case AudioClientEffectType.BulletHit: 
                            GameAudioMedia.PlayHitEnvironmentAudio((AudioGrp_HitMatType)entity.audio.AudioClientEffectArg1,entity.position.Value);
                            break;
                        case AudioClientEffectType.BulletDrop:
                            GameAudioMedia.PlayBulletDropAudio(entity.audio.AudioClientEffectArg1,
                                (AudioGrp_FootMatType)entity.audio.AudioClientEffectArg2,entity.position.Value);
                            break;
                    }
                }
            }
            else
            {
                comp.EffectLogic.Render(entity);
            }
            
            
        }
    }
}