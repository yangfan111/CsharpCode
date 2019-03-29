using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.GameHandler
{
    public abstract class BaseGameHandlerRegister
    {
        private List<IGameStateUpdateHandler> _updateHandlerList = new List<IGameStateUpdateHandler>();

        protected void RegisterUpdateHandler(IGameStateUpdateHandler handler)
        {
            _updateHandlerList.Add(handler);
        }

        protected void RegisterEventHandler(GameEvent evt, IGameEventHandler handler)
        {
            GameEventComponent.RegisterHandler(evt, handler);
        }

        public List<IGameStateUpdateHandler> GetGameStateUpdateHandlerList()
        {
            return _updateHandlerList;
        }
    }
}
