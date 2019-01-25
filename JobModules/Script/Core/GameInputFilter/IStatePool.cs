using System.Collections.Generic;
using XmlConfig;

namespace Core.GameInputFilter
{
    public interface IStatePool
    {
        IGameState GetState(EPlayerState stat);
        void Reload(Dictionary<EPlayerState, HashSet<EPlayerInput>> datas);
    }
}