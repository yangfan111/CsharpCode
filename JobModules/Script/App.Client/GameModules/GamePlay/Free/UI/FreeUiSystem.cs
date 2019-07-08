using App.Client.GameModules.GamePlay.Free.Scene;
using App.Client.GameModules.GamePlay.Free.Scripts;
using Assets.Sources.Free.Scene;
using Assets.Sources.Free.UI;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.GameModule.System;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class FreeUiSystem:IUiHfrSystem
    {
        public void OnUiRender(float interval)
        {
            SingletonManager.Get<SimpleUIUpdater>().Update();
            SingletonManager.Get<ClientFPSUpdater>().Update();
            SingletonManager.Get<FogManager>().Update();
        }
    }
}