using App.Client.GameModules.Ui;
using App.Client.SceneManagement;
using App.Shared.Components.Ui;
using App.Shared.DebugHandle;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Core.GameModule.Step;
using Free.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.SceneManagement.DistanceCulling;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class ShowCodeUIHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.ShowCodeUI;
        }

        public void Handle(SimpleProto simpleUI)
        {
            int ui = simpleUI.Ks[0];
            bool show = simpleUI.Bs[0];

            var playerEntity = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;

            if (ui == -2)
            {
                if (show)
                {
                    BigMapDebug.HandleCommand(playerEntity, new string[] { "bud1" });
                }
                else
                {
                    BigMapDebug.HandleCommand(playerEntity, new string[] { "bud0" });
                }

            }
            else if (ui == -3)
            {
                if (show)
                {
                    BigMapDebug.HandleCommand(playerEntity, new string[] { "tr1" });
                }
                else
                {
                    BigMapDebug.HandleCommand(playerEntity, new string[] { "tr0" });
                }
            }
            else if (ui == -5)
            {
                StepExecuteManager.Instance.SetFps(EEcecuteStep.CmdFrameStep, 30);
            }
            else if (ui == -6)
            {
                StepExecuteManager.Instance.SetFps(EEcecuteStep.CmdFrameStep, 60);
            }
            else if (ui == -7)
            {
//                SceneCulling.Enable = true;
            }
            else if (ui == -8)
            {
//                SceneCulling.Enable = false;
            }
            else
            {
                string com = "showui";
                if (!show)
                {
                    com = "hideui";
                }

                UISessionComponent uiSession = SingletonManager.Get<FreeUiManager>().Contexts1.ui.uISession;
                if (uiSession != null)
                {
                    if (show)
                    {
                        uiSession.HideGroup.Remove((Core.Ui.UiGroup)ui);
                    }
                    else
                    {
                        uiSession.HideGroup.Add((Core.Ui.UiGroup)ui);
                    }
                }
            }
        }
    }
}
