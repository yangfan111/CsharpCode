using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core;
using Core.Free;
using Core.Ui.Map;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    class BlastUIHandler : ISimpleMesssageHandler
    {
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
                if (data.Ks[0] == -1)
                {
                    ui.PlayerCapacity = data.Ins[0];
                    ui.ScoreForWin = data.Ins[1];
                    ui.ScoreByCampTypeDict[(int)Core.Enums.EUICampType.T] = data.Ins[2];
                    ui.ScoreByCampTypeDict[(int)Core.Enums.EUICampType.CT] = data.Ins[3];
                    ui.PlayerCountByCampTypeDict[(int)Core.Enums.EUICampType.T].PlayerCount = data.Ins[4];
                    ui.PlayerCountByCampTypeDict[(int)Core.Enums.EUICampType.CT].PlayerCount = data.Ins[5];
                    ui.PlayerCountByCampTypeDict[(int)Core.Enums.EUICampType.T].DeadPlayerCount = data.Ins[6];
                    ui.PlayerCountByCampTypeDict[(int)Core.Enums.EUICampType.CT].DeadPlayerCount = data.Ins[7];
                }
                if (data.Ks[0] == -2)
                {
                    ui.GameTime = data.Ins[0];
                }
                if(data.Ks[0] == 1)
                {
                    ui.C4InstallState = (Core.Enums.EUIBombInstallState)data.Ins[0];
                }
                if (data.Ks[0] == 2)
                {
                    ui.IsPause = data.Bs[0];
                }
            }

            if (data.Key == FreeMessageConstant.ChangeWeapon)
            {
                contexts.player.flagSelfEntity.WeaponController().PureSwitchIn((EWeaponSlotType) data.Ins[0]);
            }

            if (data.Key == FreeMessageConstant.BombAreaMarkUI)
            {
                var blast = contexts.ui.blast;
                if (data.Ks[0] == 0)
                {
                    blast.BlastAPosition = new MapFixedVector3(data.Fs[0], data.Fs[1] + 2.0f, data.Fs[2]);
                }
                if (data.Ks[0] == 1)
                {
                    blast.BlastBPosition = new MapFixedVector3(data.Fs[0], data.Fs[1] + 2.0f, data.Fs[2]);
                }
                if (data.Ks[0] == 2)
                {
                    blast.IsC4Droped = data.Bs[0];
                    if (data.Bs[0])
                    {
                        blast.C4DropPosition = new MapFixedVector3(data.Fs[0], data.Fs[1], data.Fs[2]);
                    }
                }
                if (data.Ks[0] == 3)
                {
                    blast.C4SetStatus = data.Ins[0];
                }
            }

            if (data.Key == FreeMessageConstant.BombDropTipUI)
            {
                if (data.Bs[0])
                {
                    contexts.player.flagSelfEntity.tip.Content = data.Ss[0] + " 携带的C4掉落在了地上";
                }
                else
                {
                    contexts.player.flagSelfEntity.tip.Content = data.Ss[0] + " 拾取了C4";
                }
                contexts.player.flagSelfEntity.tip.Location = TipComponent.TipLocation.Top;   
            }
        }
    }
}
