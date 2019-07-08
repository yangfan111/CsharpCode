using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System.Collections.Generic;
using Core;
using Core.EntityComponent;
using Entitas;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    /// <summary>
    /// Defines the <see cref="PlayerStateFiltedInputMgr" />
    /// </summary>
    public class PlayerStateFiltedInputMgr : IPlayerStateFiltedInputMgr
    {
        private static readonly LoggerAdapter Logger =
            new LoggerAdapter(typeof(PlayerStateFiltedInputMgr));

        private List<PlayerStateInputData> currStateInputItems = new List<PlayerStateInputData>();

        public IFilteredInput UserInput { get; private set; }

        private IPlayerStateColltector playerStateCollector;

        public IFilteredInput EmptyInput { get; private set; }

        public PlayerStateFiltedInputMgr(IPlayerStateColltector playerStateCollector) : this(playerStateCollector,
            new ValuableFilteredInput())
        {
        }

        public PlayerStateFiltedInputMgr(IPlayerStateColltector playerStateCollector,
                                         ValuableFilteredInput  valuableFilteredInput)
        {
            this.playerStateCollector = playerStateCollector;
            UserInput                 = valuableFilteredInput;
            EmptyInput                = new EmptyFilteredInput();
        }


        private void UpdateStateInputItems(IUserCmd cmd)
        {
            currStateInputItems.Clear();
            
            //获取实时玩家状态,每帧只更新一次
            HashSet<EPlayerState> currStates = playerStateCollector.GetCurrStates(EPlayerStateCollectType.CurrentMoment);
            //匍匐捡枪不允许移动
            if (currStates.Contains(EPlayerState.Pickup) && currStates.Contains(EPlayerState.Prone))
            {
                cmd.MoveHorizontal = 0f;
                cmd.MoveVertical = 0f;
                UserInput.SetInput(EPlayerInput.IsRun,false) ;
                UserInput.SetInput(EPlayerInput.IsSprint,false) ;
                UserInput.SetInput(EPlayerInput.IsSlightWalk,false) ;
                
            }
            foreach (var state in currStates)
                currStateInputItems.Add(PlayerStateInputsDataMap.Instance.GetState(state));
        }

        private void BlockUserInput()
        {
            var IsUserThrowing = UserInput.IsInput(EPlayerInput.IsThrowing);
            UserInput.SetInput(EPlayerInput.IsThrowing, true);
         
            
            //与逻辑标志位
            var pullInterrupt = false;
            for (int i = 0, maxi = currStateInputItems.Count; i < maxi; i++)
            {
                PlayerStateInputData state = currStateInputItems[i];
                if (state == null) continue;
                state.BlockUnavaliableInputs(UserInput);
                pullInterrupt = pullInterrupt || state.IsInputEnabled(EPlayerInput.IsPullboltInterrupt);
            }
            if (!UserInput.IsInput(EPlayerInput.IsThrowing))
                UserInput.SetInput(EPlayerInput.IsThrowingInterrupt, true);
            else
                UserInput.SetInput(EPlayerInput.IsThrowing, IsUserThrowing);
            UserInput.SetInput(EPlayerInput.IsPullboltInterrupt, pullInterrupt);
        }

        public bool IsInputEnalbed(EPlayerInput input)
        {
            int index = currStateInputItems.FindIndex((state) => !state.IsInputEnabled(input));
            return index == -1;
        }

        private void BlockStateInput(IUserCmd cmd)
        {
            UpdateStateInputItems(cmd);
            BlockUserInput();
        }

        public IFilteredInput ApplyUserCmd(IUserCmd cmd, int debugMoveSignal)
        {
#if UNITY_EDITOR
            if (cmd.MoveHorizontal == 0)
            {
                cmd.MoveHorizontal = debugMoveSignal;
            }
#endif
            UserCmdInputConverter.ApplyCmdToInput(cmd, UserInput);
           
            BlockStateInput(cmd);
            return UserInput;
        }
    }
}