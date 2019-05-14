using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    class GroupClassicUIHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.CommonRoundOverUI || key == FreeMessageConstant.CountDownTipUI || key == FreeMessageConstant.ResetBattleData
                || key == FreeMessageConstant.RevengeTagUI;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;

            if (data.Key == FreeMessageConstant.CommonRoundOverUI)
            {
                var ui = contexts.ui.uI;
                contexts.ui.uISession.UiState[UiNameConstant.CommonRoundOverModel] = data.Bs[0];
                ui.CurRoundCount = data.Ins[0];
                ui.ScoreByCampTypeDict[(int)Core.Enums.EUICampType.T] = data.Ins[1];
                ui.ScoreByCampTypeDict[(int)Core.Enums.EUICampType.CT] = data.Ins[2];
            }

            if (data.Key == FreeMessageConstant.CountDownTipUI)
            {
                var ui = contexts.ui.uI;
                ui.CountdownTipDataList.Clear();
                BaseTipData cdTipData = new BaseTipData
                {
                    Title = data.Ss[0],
                    DurationTime = data.Ins[0]
                };
                ui.CountdownTipDataList.Add(cdTipData);
                contexts.ui.uISession.UiState[UiNameConstant.CommonCountdownTipModel] = true;
            }

            if (data.Key == FreeMessageConstant.ResetBattleData)
            {
                contexts.player.flagSelfEntity.statisticsData.Battle.Reset();
                contexts.ui.uISession.UiState[UiNameConstant.CommonTechStatModel] = false;
            }

            if (data.Key == FreeMessageConstant.RevengeTagUI)
            {
                contexts.ui.uI.KillerId = data.Ls[0];
                contexts.ui.uI.KillerChanged = true;
            }
        }
    }
}
