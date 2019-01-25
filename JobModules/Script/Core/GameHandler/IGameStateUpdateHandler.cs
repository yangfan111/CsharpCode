using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace Core.GameHandler
{
    public interface IGameStateUpdateHandler
    {
        void Update(Entity entity);
    }
}
