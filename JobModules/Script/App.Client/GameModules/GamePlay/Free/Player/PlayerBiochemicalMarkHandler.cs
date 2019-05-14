using Assets.Sources.Free;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Free.framework;
using System;
using Core.Free;
using Utils.Singleton;
using Assets.Sources.Free.UI;
using Assets.App.Client.GameModules.Ui;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerBiochemicalMarkHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerBiochemicalMark;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var selfEntity = contexts.player.flagSelfEntity;
            var ui = contexts.ui.uI;
            /*contexts.ui.uISession.UiState[UiNameConstant.BiochemicalMarkModel] = data.Bs[0];*/
            var type = data.Ins[0];
            var add = data.Bs[0];
            var playerId = data.Ls[0];
            var motherIdList = contexts.ui.uI.MotherIdList;
            var heroIdList = contexts.ui.uI.HeroIdList;
            var humanIdList = contexts.ui.uI.HumanIdList;

            switch (type) {
                case 0:
                    motherIdList.Clear();
                    heroIdList.Clear();
                    humanIdList.Clear();
                    break;
                case 1:
                    if (add) motherIdList.Add(playerId);
                    else motherIdList.Remove(playerId);
                    break;
                case 2:
                    if (add) heroIdList.Add(playerId);
                    else heroIdList.Remove(playerId);
                    break;
                case 3:
                    if (add) humanIdList.Add(playerId);
                    else humanIdList.Remove(playerId);
                    break;
            }

        }
    }
}