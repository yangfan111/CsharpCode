using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Enums;
using Core.Free;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class UICountDownHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.CountDown;
        }

        public void Handle(SimpleProto simpleProto)
        {
//            var data = SingletonManager.Get<FreeUiManager>().Contexts1.ui.uiDataAdapterEntity.uiDataAdapter.CrossHariState;
//            data.SetCountDown(simpleProto.Bs[0], simpleProto.Fs[0]);
            SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.CountingDown = simpleProto.Bs[0];
            SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.CountDownNum = simpleProto.Fs[0];
        }
    }
}