using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Assets.Sources.Free.UI;
using Core.Components;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerMoveHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.EntityMoveTo;
        }

        public void Handle(SimpleProto data)
        {
            var contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            if(contexts.player.flagSelfEntity != null)
            {
                //contexts.player.flagSelfEntity.position.Value = new UnityEngine.Vector3(data.Fs[0], data.Fs[1], data.Fs[2]);
                contexts.player.flagSelfEntity.latestAdjustCmd.SetPos(new UnityEngine.Vector3(data.Fs[0], data.Fs[1],
                    data.Fs[2]));
            }
        }
    }
}
