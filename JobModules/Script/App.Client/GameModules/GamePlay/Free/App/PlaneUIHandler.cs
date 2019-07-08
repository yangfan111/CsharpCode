using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class PlaneUIHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlaneUI;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var ui = contexts.ui.uI;

            // 显示更新
            if (data.Bs[0])
            {
                // 显示并更新
                if (data.Bs[1])
                {
                    contexts.ui.uISession.UiState[UiNameConstant.ChickenPlaneModel] = true;
                    ui.CurPlayerCountInPlane = data.Ins[0];
                    ui.TotalPlayerCountInPlane = data.Ins[1];
                }
                else
                {
                    // 只更新
                    ui.CurPlayerCountInPlane = data.Ins[0];
                    ui.TotalPlayerCountInPlane = data.Ins[1];
                }
            }
            else
            {
                // 隐藏
                contexts.ui.uISession.UiState[UiNameConstant.ChickenPlaneModel] = false;
            }
        }
    }
}
