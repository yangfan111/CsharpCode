using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using App.Server.GameModules.GamePlay.Free.player;
using Assets.Sources.Free.UI;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerAnimationHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerAnimation;
        }

        public void Handle(SimpleProto data)
        {
            int ani = data.Ins[0];
            var contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            PlayerAnimationAction.DoAnimation(contexts, ani, contexts.player.flagSelfEntity, false);
        }
    }
}
