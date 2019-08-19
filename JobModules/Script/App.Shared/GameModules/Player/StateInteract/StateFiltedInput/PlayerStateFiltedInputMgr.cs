using Core;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Free;
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
            ValuableFilteredInput valuableFilteredInput)
        {
            this.playerStateCollector = playerStateCollector;
            UserInput = valuableFilteredInput;
            EmptyInput = new EmptyFilteredInput();
        }


        private void UpdateStateInputItems(IUserCmd cmd)
        {
            currStateInputItems.Clear();

            //获取实时玩家状态,每帧只更新一次
            HashSet<EPlayerState> currStates =
                playerStateCollector.GetCurrStates(EPlayerStateCollectType.CurrentMoment);
            //匍匐捡枪不允许移动
            if (currStates.Contains(EPlayerState.Pickup) && currStates.Contains(EPlayerState.Prone))
            {
                cmd.MoveHorizontal = 0f;
                cmd.MoveVertical = 0f;
                UserInput.SetInput(EPlayerInput.IsRun, false);
                UserInput.SetInput(EPlayerInput.IsSprint, false);
                UserInput.SetInput(EPlayerInput.IsSlightWalk, false);

            }

            foreach (var state in currStates)
                currStateInputItems.Add(PlayerStateInputsDataMap.Instance.GetState(state));
        }

        // private struct InputInfo
        // {
        //     public EPlayerInput input;
        //     public bool isInput;
        //     public Action execAction;
        //
        //     public InputInfo(EPlayerInput input, bool isInput, Action execAction)
        //     {
        //         this.isInput = isInput;
        //         this.input = input;
        //         this.execAction = execAction;
        //     }
        // }
        //
        // private List<InputInfo> tempOriginInputs = new List<InputInfo>();

        // private void CacheBeingBlockCmd(EPlayerInput playerInput, System.Action execAction)
        // {
        //     tempOriginInputs.Add(new InputInfo(playerInput, UserInput.IsInput(playerInput), execAction));
        //     UserInput.SetInput(playerInput, true);
        // }
        //
        // private void ExecWhenBlockCmd()
        // {
        //     foreach (var oinput in tempOriginInputs)
        //     {
        //         if (!UserInput.IsInput(oinput.input))
        //         {
        //             oinput.execAction();
        //         }
        //         else
        //         {
        //             UserInput.SetInput(oinput.input, oinput.isInput);
        //         }
        //     }
        // }
        private void DoReloadInterrupt()
        {
            //    UserInput.SetInput(EPlayerInput.IsReloadInterrupt, true);

        }

        private HashSet<InterruptConfigType> interruptMergeBuffer = new HashSet<InterruptConfigType>();
        private void BlockUserInput()
        {
            interruptMergeBuffer.Clear();
            PlayerStateInputData state;
            for (int i = 0, maxi = currStateInputItems.Count; i < maxi; i++)
            {
                state = currStateInputItems[i];
                if (state == null) continue;
                state.BlockUnavaliableInputs(UserInput);
                state.MergeInterrupts(interruptMergeBuffer);
            }
            EPlayerInput input;
            foreach (var interrupt in interruptMergeBuffer)
            {
                input = interrupt.ToEPlayerInput();
                if(input != EPlayerInput.None)
                    UserInput.SetInput(input, true);
            //    DebugUtil.MyLog("Set {0}",interrupt.ToEPlayerInput());
            }
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
            if (playerStateCollector.GetCurrStates(EPlayerStateCollectType.CurrentMoment)
                .Contains(EPlayerState.FinalPosing))
                return EmptyInput;

            UserCmdInputConverter.ApplyCmdToInput(cmd, UserInput);
            UserCmdInputConverter.ApplyInputStaticBlock(UserInput);
            BlockStateInput(cmd);
            UserCmdInputConverter.ApplyInputInterrupt(UserInput);
            return UserInput;
        }
    }
}