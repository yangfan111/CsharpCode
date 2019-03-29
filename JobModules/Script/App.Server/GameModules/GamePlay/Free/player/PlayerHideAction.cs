using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Appearance;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.unit;
using Utils.Appearance;
using App.Shared.Components;

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
