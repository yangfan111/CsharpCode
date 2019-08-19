using App.Client.GameModules.Ui.UiAdapter;
using App.Shared;
using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core;
using Core.Enums;
using Core.Free;
using Core.Ui.Map;
using Core.Utils;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    class BlastUIHandler : ISimpleMesssageHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BlastUIHandler));

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.BlastScoreUI || key == FreeMessageConstant.ChangeWeapon || key == FreeMessageConstant.BombAreaMarkUI
                   || key == FreeMessageConstant.BombDropTipUI;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;

            if (data.Key == FreeMessageConstant.BlastScoreUI)
            {
                var ui = contexts.ui.uI;
                switch (data.Ks[0])
                {
                    case -1:
                        ui.PlayerCapacity = data.Ins[0];
                        ui.ScoreForWin = data.Ins[1];
                        ui.ScoreByCampTypeDict[(int) EUICampType.T] = data.Ins[2];
                        ui.ScoreByCampTypeDict[(int) EUICampType.CT] = data.Ins[3];
                        ui.PlayerCountByCampTypeDict[(int) EUICampType.T].PlayerCount = data.Ins[4];
                        ui.PlayerCountByCampTypeDict[(int) EUICampType.CT].PlayerCount = data.Ins[5];
                        ui.PlayerCountByCampTypeDict[(int) EUICampType.T].DeadPlayerCount = data.Ins[6];
                        ui.PlayerCountByCampTypeDict[(int) EUICampType.CT].DeadPlayerCount = data.Ins[7];
                        break;
                    case -2:
                        ui.GameTime = data.Ins[0];
                        break;
                    case 1:
                        ui.C4InstallState = (EUIBombInstallState) data.Ins[0];
                        ui.C4InitialProgress = data.Fs.Count > 0 ? data.Fs[0] : 0;
                        ui.IsPause = data.Ins[0] >= 1;
                        break;
                    case 2:
                        ui.IsPause = data.Bs[0];
                        break;
                    default:
                        break;
                }
            }

            if (data.Key == FreeMessageConstant.ChangeWeapon)
            {
                switch (data.Ins[0])
                {
                    case 0:
                        contexts.player.flagSelfEntity.WeaponController().UnArmWeapon(false);
                        break;
                    default:
                        contexts.player.flagSelfEntity.WeaponController().PureSwitchIn((EWeaponSlotType) data.Ins[0]);
                        break;
                }
            }

            if (data.Key == FreeMessageConstant.BombAreaMarkUI)
            {
                var blast = contexts.ui.blast;
                switch (data.Ks[0])
                {
                    case 0:
                        blast.BlastAPosition = new MapFixedVector3(data.Fs[0], data.Fs[1] + 2.0f, data.Fs[2]);
                        break;
                    case 1:
                        blast.BlastBPosition = new MapFixedVector3(data.Fs[0], data.Fs[1] + 2.0f, data.Fs[2]);
                        break;
                    case 2:
                        blast.IsC4Droped = data.Bs[0];
                        if (data.Bs[0])
                        {
                            blast.C4DropPosition = new MapFixedVector3(data.Fs[0], data.Fs[1], data.Fs[2]);
                        }
                        break;
                    case 3:
                        blast.C4SetStatus = data.Ins[0];
                        break;
                    default:
                        break;
                }
            }

            if (data.Key == FreeMessageConstant.BombDropTipUI)
            {
                var msg = "";
                if (data.Bs[0])
                {
                    msg = data.Ss[0] + " 携带的C4掉落在了地上";
                }
                else
                {
                    msg = data.Ss[0] + " 拾取了C4";
                }
                BaseTipData tip = new BaseTipData
                {
                    Title = msg,
                    DurationTime = 5000
                };
                contexts.ui.uISession.UiState[UiNameConstant.CommonSystemTipModel] = true;
                contexts.ui.uI.SystemTipDataQueue.Enqueue(tip);
            }
        }
    }
}
