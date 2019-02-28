using Core.GameModule.Interface;

namespace App.Client.GameModules.Player
{
    public abstract class AbstractSelfPlayerRenderSystem:IRenderSystem
    {

        public abstract void OnRender(PlayerEntity playerEntity);

        public readonly PlayerContext _playerContext;

        protected AbstractSelfPlayerRenderSystem(Contexts contexts)
        {
            _playerContext = contexts.player;
        }

        protected virtual bool Filter(PlayerEntity playerEntity)
        {
            return true;
        }

        public void OnRender()
        {
            var player = _playerContext.flagSelfEntity;
            if (player != null)
            {
                if (Filter(player))
                {
                    OnRender(player);
                }
                else
                {
                    OnFilterRender();
                }
            }
        }

        public virtual void OnFilterRender()
        {
            
        }
    }
}