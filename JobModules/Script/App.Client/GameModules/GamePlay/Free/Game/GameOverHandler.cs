using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using App.Client.Utility;

namespace App.Client.GameModules.GamePlay.Free.Game
{
    class GameOverHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.GameOver;
        }

        public void Handle(SimpleProto data)
        {
            HallUtility.GameOver();
        }
    }
}
