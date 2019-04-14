using System.Collections.Generic;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;
namespace Core
{
    public interface IPlayerStateInterrupter
    {
        void DoRunTimeInterrupt(IUserCmd cmd);
        void InterruptCharactor();
    }

   
    public interface IPlayerStateColltector
    {
        HashSet<EPlayerState> GetCurrStates(EPlayerStateCollectType collectType = EPlayerStateCollectType.UseCache);


    }
    public interface IPlayerStateFiltedInputMgr
    {
        IFilteredInput EmptyInput { get; }
        IFilteredInput UserInput { get; }

        IFilteredInput ApplyUserCmd(IUserCmd userCmd);
    }
}
