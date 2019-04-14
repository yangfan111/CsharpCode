using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerVisibilityAction : AbstractPlayerAction
    {
        private bool visibility;

        private bool teammate;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            playerEntity.gamePlay.Invisible = !visibility;
            playerEntity.gamePlay.VisibleToTeammate = teammate;
            foreach (PlayerEntity pe in args.GameContext.player.GetEntities())
            {
                if (playerEntity != pe && (pe.playerInfo.TeamId == playerEntity.playerInfo.TeamId) == teammate)
                {
                    SimpleProto sp = FreePool.Allocate();
                    sp.Key = FreeMessageConstant.PlayerVisibility;
                    sp.Ls.Add(playerEntity.playerInfo.PlayerId);
                    sp.Bs.Add(visibility);
                    FreeMessageSender.SendMessage(pe, sp);
                }
            }
        }
    }
}
