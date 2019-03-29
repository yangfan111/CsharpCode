using Core.Prediction.UserPrediction.Cmd;

namespace Core.GameInputFilter
{
    public interface IGameInputProcessor
    {
        void Init();
        void SetUserCmd(IUserCmd cmd);
        void AddState(IGameState gameState);
        IFilteredInput Filter();
        IFilteredInput DummyInput();
        IFilteredInput GetFilteredInput();
    }
}