using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.unit;
using App.Server.GameModules.GamePlay.free.player;

namespace com.wd.free.action
{
    [Serializable]
    public abstract class AbstractPlayerAction : AbstractGameAction
    {
        protected string player;

        public void SetPlayer(string player)
        {
            this.player = player;
        }

        public PlayerEntity GetPlayerEntity(IEventArgs args)
        {
            if (!string.IsNullOrEmpty(player))
            {
                FreeData fd = (FreeData)args.GetUnit(player);
                return fd.Player;
            }

            return null;
        }

        public FreeData GetPlayer(IEventArgs args)
        {
            if (!string.IsNullOrEmpty(player))
            {
                return (FreeData)args.GetUnit(player);
            }

            return null;
        }
    }
}
