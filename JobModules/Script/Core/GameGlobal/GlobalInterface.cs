using System.Collections.Generic;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;

namespace Core
{
    /// <summary>
    /// Defines the <see cref="ISessionMode" />
    /// </summary>
    public interface ISessionMode
    {
        int ModeId { get; }
    }
    public interface IGameWeapon
    {
        EntityKey Owner { get; }
    }
    public interface IFilteredInput 
    {
        bool IsInput(EPlayerInput input);
        /// <summary>
        /// 设置输入值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="val"></param>
        void SetInput(EPlayerInput input, bool val);
    }
    public interface IPlayerStateInterrupter
    {
        void DoRunTimeInterrupt(IUserCmd cmd);
        void InterruptCharactor();

        bool IsInterrupted(EInterruptType interruptType);
    }

   
    public interface IPlayerStateColltector
    {
        HashSet<EPlayerState> GetCurrStates(EPlayerStateCollectType collectType = EPlayerStateCollectType.UseCache);


    }
    public interface IPlayerStateFiltedInputMgr
    {
        IFilteredInput EmptyInput { get; }
        IFilteredInput UserInput  { get; }

        IFilteredInput ApplyUserCmd(IUserCmd userCmd);
    }
}
