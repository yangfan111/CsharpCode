using App.Shared.Terrains;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
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
                Vector3 leftMinPos = TerrainCommonData.leftMinPos;
                data.IsShowRouteLine = true;
                data.RouteLineStartPoint = new Vector2(simpleProto.Fs[0] - leftMinPos.x, simpleProto.Fs[1] - leftMinPos.z);
                data.RouteLineEndPoint = new Vector2(simpleProto.Fs[2] - leftMinPos.x, simpleProto.Fs[3] - leftMinPos.z);
            }
        }
    }
}
