using App.Shared.Player;
using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Free.framework;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerMsgHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerScale ||
                key == FreeMessageConstant.PlayerRageStart||
                key == FreeMessageConstant.ThirdPerson ||
                key == FreeMessageConstant.CommonPickUpModel;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            if (data.Key == FreeMessageConstant.PlayerScale)
            {
                PlayerEntity pe = null;
                foreach (PlayerEntity playerEntity in contexts.player.GetEntities())
                {
                    if (playerEntity.playerInfo.PlayerId == data.Ls[0])
                    {
                        pe = playerEntity;
                        break;
                    }
                }

                if (pe != null)
                {
                    pe.RootGo().transform.localScale = data.Fs[0] * Vector3.one;
                }
            }
            else if (data.Key == FreeMessageConstant.PlayerRageStart)
            {
                if (data.Bs[0])
                {
                    contexts.player.flagSelfEntity.stateInterface.State.RageStart();
                }
                else
                {
                    contexts.player.flagSelfEntity.stateInterface.State.RageEnd();
                }
            }
            else if (data.Key == FreeMessageConstant.ThirdPerson)
            {
                var sessionObjects = contexts.session.clientSessionObjects;
                var generator = sessionObjects.UserCmdGenerator;
                UserCmd cmd = generator.GetUserCmd();
                PlayerEntity player = contexts.player.flagSelfEntity;

                if (player.hasCameraStateNew && player.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight)
                {
                    cmd.IsCameraFocus = true;
                }

                if (player.hasAppearanceInterface && player.appearanceInterface.Appearance.IsFirstPerson)
                {
                    cmd.ChangeCamera = true;
                }
            }
            else if (data.Key == FreeMessageConstant.CommonPickUpModel)
            {
                contexts.ui.uI.ShowBuffTip = data.Bs[0];
                if (data.Ss != null && !string.IsNullOrEmpty(data.Ss[0]))
                {
                    contexts.ui.uI.BuffTip = data.Ss[0];
                }
                else
                {
                    contexts.ui.uI.BuffTip = string.Empty;
                }
            }
        }
    }
}
