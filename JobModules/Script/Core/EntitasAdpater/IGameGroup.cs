using System.Collections.Generic;
using Core.EntityComponent;

namespace Core.EntitasAdpater
{
    public interface IGameGroup
    {
        List<IGameEntity> GetGameEntities();
    }
}