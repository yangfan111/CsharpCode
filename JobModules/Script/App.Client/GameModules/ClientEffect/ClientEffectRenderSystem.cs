using App.Client.GameModules.Player;
using Core.GameModule.Interface;
using Entitas;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectRenderSystem : AbstractRenderSystem<ClientEffectEntity>
    {
        public ClientEffectRenderSystem(Contexts context) : base(context)
        {
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
            var comp = entity.logic;
            if (!entity.assets.IsInitialized)
            {
                entity.assets.IsInitialized = true;
                comp.EffectLogic.Initialize(entity);
            }
            else
            {
                comp.EffectLogic.Render(entity);
            }
        }
    }
}