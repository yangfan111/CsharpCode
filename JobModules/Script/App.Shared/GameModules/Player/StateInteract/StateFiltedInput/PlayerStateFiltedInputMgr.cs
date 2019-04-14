using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System.Collections.Generic;
using Core;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    /// <summary>
    /// Defines the <see cref="PlayerStateFiltedInputMgr" />
    /// </summary>
    public class PlayerStateFiltedInputMgr:IPlayerStateFiltedInputMgr
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


        private void UpdateStateInputItems()
        {
            currStateInputItems.Clear();
            //获取当前玩家状态
            var currStates = playerStateCollector.GetCurrStates(EPlayerStateCollectType.UseMoment);
            foreach (var state in currStates)
                currStateInputItems.Add(PlayerStateInputsDataMap.Instance.GetState(state));
        }

        private void BlockUserInput()
        {
            currStateInputItems.ForEach((state) =>
            {
                if (state != null)
                    state.BlockUnavaliableInputs(UserInput);
            });
        }

        public bool IsInputEnalbed(EPlayerInput input)
        {
            int index = currStateInputItems.FindIndex((state) => !state.IsInputEnabled(input));
            return index == -1;
        }

        private void BlockStateInput()
        {
            UpdateStateInputItems();
            currStateInputItems.ForEach((state) =>
            {
                if (state != null)
                    state.BlockUnavaliableInputs(UserInput);
            });
        }

        public IFilteredInput ApplyUserCmd(IUserCmd cmd)
        {
            UserCmdInputConverter.ApplyCmdToInput(cmd, UserInput);
            BlockStateInput();
            return UserInput;
        }
    }
}
