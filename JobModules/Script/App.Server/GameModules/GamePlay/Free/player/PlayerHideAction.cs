using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;
using System;
using Utils.Appearance;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerHideAction : AbstractPlayerAction
    {
        private string show;

        public override void DoAction(IEventArgs args)
        {
            IGameUnit unit = GetPlayer(args);

            if(unit != null)
            {
                PlayerEntity player = ((FreeData)unit).Player;

                if (FreeUtil.ReplaceBool(show, args))
                {
                    AppearanceUtils.EnableRender(player.thirdPersonModel.Value);
                    player.gamePlay.GameState = GameState.Visible;
                }
                else
                {
                    AppearanceUtils.DisableRender(player.thirdPersonModel.Value);
                    player.gamePlay.GameState = GameState.Invisible;
                }
            }
            
        }
    }
}
