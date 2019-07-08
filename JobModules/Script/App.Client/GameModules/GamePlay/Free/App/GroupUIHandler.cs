using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Enums;
using Core.Free;
using Core.Room;
using Free.framework;
using System.Collections.Generic;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    class GroupUIHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.GroupScoreUI || key == FreeMessageConstant.GroupTechStatUI
                || key == FreeMessageConstant.GroupGameOverUI || key == FreeMessageConstant.PlayerTipHide ||
                key == FreeMessageConstant.CommonPlayerInfoUI;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;

            if (data.Key == FreeMessageConstant.GroupScoreUI)
            {
                int totalScore = data.Ins[0];
                int t1Score = data.Ins[1];
                int t2Score = data.Ins[2];
                int time = data.Ins[3];

                var ui = contexts.ui.uI;
                ui.GameTime = time;
                ui.ScoreForWin = totalScore;
                ui.ScoreByCampTypeDict[(int)EUICampType.T] = t1Score;
                ui.ScoreByCampTypeDict[(int)EUICampType.CT] = t2Score;
            }

            if (data.Key == FreeMessageConstant.GroupGameOverUI)
            {
                var ui = contexts.ui.uI;
                //ui.IsGameOver = data.Bs[0];
                contexts.ui.uISession.UiState[UiNameConstant.CommonGameOverModel] = data.Bs[0];
                ui.GameResult = (EUIGameResultType)data.Ins[0];
            }

            if (data.Key == FreeMessageConstant.CommonPlayerInfoUI) {
                var ui = contexts.ui.uI;
                contexts.ui.uISession.UiState[UiNameConstant.CommonPlayerInfo] = data.Bs[0];
            }

            if (data.Key == FreeMessageConstant.GroupTechStatUI)
            {
                var ui = contexts.ui.uI;
                //ui.RoomId = 1;
                ui.PlayerCount = data.Ins[0];

                RoomInfo room = contexts.session.commonSession.RoomInfo;

                ui.ChannelName = room.ChannelName;
                ui.RoomName = room.RoomName;
                ui.RoomId = room.RoomDisplayId;
                ui.PlayerCapacity = room.RoomCapacity;

                long selfId = 0;
                if (contexts.player.flagSelfEntity != null)
                {
                    selfId = contexts.player.flagSelfEntity.playerInfo.PlayerId;
                }

                List<IGroupBattleData> team1 = new List<IGroupBattleData>();
                List<IGroupBattleData> team2 = new List<IGroupBattleData>();
                for (int i = 0; i < data.Ps.Count; i++)
                {
                    SimpleProto p = data.Ps[i];
                    int team = p.Ins[0];

                    IGroupBattleData groupdata = ToBattleData(selfId, p);

                    if (team == (int)EUICampType.T)
                    {
                        team1.Add(groupdata);
                    }
                    else
                    {
                        team2.Add(groupdata);
                    }

                    if (groupdata.IsMySelf)
                    {
                        ui.MyselfGameTitle = groupdata.GameTitle;
                    }
                }

                ui.GroupBattleDataDict[(int)EUICampType.T] = team1;
                ui.GroupBattleDataDict[(int)EUICampType.CT] = team2;
                ui.GroupBattleDataChanged = true;
            }

            if (data.Key == FreeMessageConstant.PlayerTipHide)
            {
                if (contexts.player.flagSelfEntity != null)
                {
                    contexts.player.flagSelfEntity.gamePlay.TipHideStatus = data.Bs[0];
                }
            }
        }

        private IGroupBattleData ToBattleData(long selfId, SimpleProto msg)
        {
            GroupBattleData data = new GroupBattleData();
            data.IsDead = msg.Bs[0];
            data.GameTitle = msg.Ins[2];
            data.KillCount = msg.Ins[3];
            data.DeadCount = msg.Ins[4];
            data.AssistCount = msg.Ins[5];
            data.Damage = msg.Ins[6];
            data.IsMySelf = msg.Ins[1] == selfId;
            data.Name = msg.Ss[0];
            data.CorpsName = msg.Ss[1];
            data.Ping = msg.Ins[7];
            data.C4PlantCount = msg.Ins[8];
            data.C4DefuseCount = msg.Ins[9];
            data.HaveC4 = msg.Bs[1];

            return data;
        }
    }
}
