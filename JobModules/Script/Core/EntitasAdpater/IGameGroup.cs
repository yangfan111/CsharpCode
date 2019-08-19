using System.Collections.Generic;
using Core.EntityComponent;

namespace Core.EntityComponent
{
    public interface IGameGroup
    {
        List<IGameEntity> GetGameEntities();
    }
}