using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace Core.GameHandler
{
    public interface IGameEventHandler
    {
        void Handle(GameEvent evt, Entity entity);
    }
}
