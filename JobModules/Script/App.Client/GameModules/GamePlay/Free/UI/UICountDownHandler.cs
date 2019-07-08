using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class UICountDownHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.CountDown;
        }

        public void Handle(SimpleProto data)
        {
            var ui = SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI;
            ui.CountingDown = data.Bs[0];
            ui.CountDownNum = data.Fs[0];
            if (data.Bs.Count > 1)
            {
                ui.HaveCompletedCountDown = data.Bs[1];
            }
            if (data.Ins.Count > 0 && data.Ins[0] != 0)
            {
                SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity.tip.TipType = (ETipType) data.Ins[0];
            }
        }
    }
}