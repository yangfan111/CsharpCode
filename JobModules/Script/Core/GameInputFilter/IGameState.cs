using XmlConfig;

namespace Core.GameInputFilter
{
    public interface IGameState 
    {
        EPlayerState State();
        bool IsState(EPlayerState State);
        void FilterInput(IFilteredInput input);
        bool IsInputEnabled(EPlayerInput input);
    }
}
