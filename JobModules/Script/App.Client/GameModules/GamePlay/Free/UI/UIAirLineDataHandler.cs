using App.Shared.Terrains;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Components;
using Core.Free;
using Core.Ui.Map;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class UIAirLineDataHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.AirLineData;
        }

        public void Handle(SimpleProto simpleProto)
        {
            var data = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;
            if (simpleProto.Bs[0])
            {
                data.IsShowRouteLine = false;
            }
            else
            {
                data.IsShowRouteLine = true;
                data.RouteLineStartPoint = new MapFixedVector2(simpleProto.Fs[0], simpleProto.Fs[1]);
                data.RouteLineEndPoint = new MapFixedVector2(simpleProto.Fs[2], simpleProto.Fs[3]);
            }
        }
    }
}
