using System.Collections.Generic;
using XmlConfig;

namespace Core.GameInputFilter
{
    public interface IStateProvider
    {
        void ApplyStates(IGameInputProcessor processor);
        HashSet<EPlayerState> GetCurrentStates();
    }
}