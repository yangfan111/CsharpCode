using App.Shared.Components.Ui;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Terrains;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Scene
{
    public class PoisonCircleHandler : ISimpleMesssageHandler
    {

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PoisonCircle;
        }

        public void Handle(SimpleProto message)
        {
//            var data = SingletonManager.Get<FreeUiManager>().Contexts1.ui.uiDataAdapterEntity.uiDataAdapter.MiniMapUIAdapter;
            var data = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;

            int type = message.Ks[0];

            //获取当前所在地图配置数据
            Vector3 leftMinPos = TerrainCommonData.leftMinPos;
           
            if (type == 0)
            {
                //DuQuanInfo 构造函数修改了参数 需要重新接入数据
                int level = data.CurDuquan.Level + 1;

                data.CurDuquan = new DuQuanInfo(level, new Vector2(message.Fs[0] - leftMinPos.x, message.Fs[1] - leftMinPos.z), message.Ins[0], message.Ins[1], message.Ins[2]);
                data.NextDuquan = new DuQuanInfo(level + 1, new Vector2(message.Fs[2] - leftMinPos.x, message.Fs[3] - leftMinPos.z), message.Ins[3], message.Ins[4], message.Ins[5]);
            }
            else
            {
                data.BombArea = new BombAreaInfo(new Vector3(message.Fs[0] - leftMinPos.x, 0, message.Fs[1] - leftMinPos.z), message.Ins[0], data.BombArea.Num + 1);
            }
        }
    }
}
