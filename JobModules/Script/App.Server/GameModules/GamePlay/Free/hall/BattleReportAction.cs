using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using com.wd.free.para;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.Free.hall
{
    public class BattleReportAction : AbstractGameAction
    {
        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
            var msg = GameOverMessage.Allocate();
            msg.HallRoomId = args.GameContext.session.commonSession.RoomInfo.HallRoomId;

            FreeGameRule rule = (FreeGameRule)args.Rule;

            foreach (PlayerEntity player in args.GameContext.player.GetInitializedPlayerEntities())
            {
                SimpleParable sp = new SimpleParable();
                SimpleParaList paras = new SimpleParaList();
                sp.SetList(paras);

                var gameOverPlayer = GameOverPlayer.Allocate();

                paras.AddFields(new ObjectFields(gameOverPlayer));
                gameOverPlayer.Id = player.playerInfo.PlayerId;

                args.Act(action, new TempUnit[] { new TempUnit("basic", sp) });
            }
        }
    }
}
