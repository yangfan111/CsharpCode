using Core;
using System.Collections.Generic;
using Core.Utils;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
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
        private bool[] avaliableInputsContain;

        private List<EPlayerInput> unavaliableInputs = new List<EPlayerInput>();

        public PlayerStateInputData(EPlayerState state, HashSet<EPlayerInput> inputList)
        {
            ownedState = state;
            avaliableInputsContain = new bool[(int)EPlayerInput.Length];
            avaliableInputs =
                inputList ?? new HashSet<EPlayerInput>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);
            for (var e = EPlayerInput.None + 1; e < EPlayerInput.Length; e++)
            {
                if (!avaliableInputs.Contains(e))
                {
                    unavaliableInputs.Add(e);
                    avaliableInputsContain[(int)e] = false;
                }
                else
                {
                    avaliableInputsContain[(int)e] = true;
                }
            }
        }

        /// <summary>
        /// 屏蔽掉状态对应无效的输入
        /// </summary>
        /// <param name="BlockUnavaliableInputs"></param>
        public void BlockUnavaliableInputs(IFilteredInput filteredInput)
        {
            for (int i = 0, maxi = unavaliableInputs.Count; i < maxi; i++) {
                EPlayerInput input = unavaliableInputs[i];
#if UNITY_EDITOR
                if (GlobalConst.EnableInputBlockLog && input == GlobalConst.serachedInput)
                {
                    DebugUtil.MyLog("player state {0} block {1}", ownedState, input);
                }
#endif
                filteredInput.SetInput(input, false);
            }
        }

        public bool IsInputEnabled(EPlayerInput input)
        {
            return /*avaliableInputs.Contains(input)*/avaliableInputsContain[(int)input];
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