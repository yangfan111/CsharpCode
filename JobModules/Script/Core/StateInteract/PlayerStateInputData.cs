using Core;
using System.Collections.Generic;

using Utils.Utils;
using XmlConfig;

namespace Core
{
    /// <summary>
    ///configitem for player state ==> Inputs
    /// </summary>
    public class PlayerStateInputData 
    {
        /// <summary>
        ///状态类型
        /// </summary>
        private EPlayerState ownedState;

        /// <summary>
        /// 状态类型对应的生效输入
        /// </summary>
        private HashSet<EPlayerInput> avaliableInputs;

        private List<EPlayerInput> unavaliableInputs = new List<EPlayerInput>();

        public PlayerStateInputData(EPlayerState state, HashSet<EPlayerInput> inputList)
        {
            ownedState = state;
            avaliableInputs =
                inputList ?? new HashSet<EPlayerInput>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);
            for (var e = EPlayerInput.None + 1; e < EPlayerInput.Length; e++)
            {
                if (!avaliableInputs.Contains(e))
                    unavaliableInputs.Add(e);
            }
        }

        /// <summary>
        /// 屏蔽掉状态对应无效的输入
        /// </summary>
        /// <param name="BlockUnavaliableInputs"></param>
        public void BlockUnavaliableInputs(IFilteredInput filteredInput)
        {
            unavaliableInputs.ForEach((input =>filteredInput.SetInput(input,false)));
        }

        public bool IsInputEnabled(EPlayerInput input)
        {
            return avaliableInputs.Contains(input);
        }

        public bool IsState(EPlayerState State)
        {
            return ownedState == State;
        }

        public EPlayerState State
        {
            get { return ownedState; }
        }
    }
}