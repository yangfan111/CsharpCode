using App.Shared.Components;
using Core.GameModule.Interface;
using Utils.Appearance;

namespace Assets.App.Client.GameModules.GamePlay.Free.Entitas
{
    public class PlayerStateChangeSystem : IGamePlaySystem
    {
        private Contexts _contexts;

        public PlayerStateChangeSystem(Contexts contexts)
        {
            this._contexts = contexts;
        }

        public void OnGamePlay()
        {
            foreach (PlayerEntity player in _contexts.player.GetEntities())
            {
                if(player.gamePlay.GameState == GameState.Invisible)
                {
                    if(player.gamePlay.ClientState != GameState.Invisible)
                    {
                        AppearanceUtils.DisableRender(player.thirdPersonModel.Value);
                        player.gamePlay.ClientState = GameState.Invisible;
                    }
                    
                }
                if (player.gamePlay.GameState == GameState.Visible)
                {
                    if(player.gamePlay.ClientState == GameState.Invisible)
                    {
                        AppearanceUtils.EnableRender(player.thirdPersonModel.Value);
                        player.gamePlay.ClientState = GameState.Normal;
                    }
                }
            }
        }
    }
}
