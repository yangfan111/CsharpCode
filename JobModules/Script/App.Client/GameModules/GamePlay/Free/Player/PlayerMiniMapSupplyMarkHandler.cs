using Assets.Sources.Free;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Free.framework;
using System;
using Core.Free;
using Utils.Singleton;
using Assets.Sources.Free.UI;
using com.wd.free.map;
using Core.Enums;
using Core.Ui.Map;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerMiniMapSupplyMarkHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerMiniMapSupplyMark;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var selfEntity = contexts.player.flagSelfEntity;
            var ui = contexts.ui.uI;
            var type = data.Ins[0];

            var x = data.Fs[0];
            var y = data.Fs[1];
            var z = data.Fs[2];

            var key = data.Ss[0];
            var supplyPosMap = contexts.ui.map.SupplyPosMap;
            switch (type) {
                case (int)EMiniMapSupplyMark.SupplyAdd:
                    MapFixedVector3 pos;
                    if (!supplyPosMap.TryGetValue(key, out pos))
                    {
                        supplyPosMap.Add(key, new MapFixedVector3(x, y, z));
                    }
                    break;
                case (int)EMiniMapSupplyMark.SupplyRemove:
                    supplyPosMap.Remove(key);
                    break;
                case (int)EMiniMapSupplyMark.SupplyClear:
                    supplyPosMap.Clear();
                    break;
            }
        }
    }
}
