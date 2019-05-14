using Core;
using Core.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class InputBlockGroup
    {
        private EPlayerInput[] blockedInputs;

        public static InputBlockGroup Create(EPlayerInput[] blockedInputs)
        {
            var instance = new InputBlockGroup(blockedInputs);
            return instance;
        }

        public InputBlockGroup(EPlayerInput[] in_blockInputs)
        {
            blockedInputs = in_blockInputs;
        }

        public void DoBlock(IFilteredInput filteredInput)
        {
            var block = false;
            var blockedInput = EPlayerInput.None;
            foreach (var input in blockedInputs)
            {
                if (block)
                {
#if UNITY_EDITOR
                    if (GlobalConst.EnableInputBlockLog)
                    {
                        if (input == GlobalConst.serachedInput)
                        {
                            DebugUtil.MyLog("{0} Blocked input {1}", blockedInput, GlobalConst.serachedInput);
                        }
                    }
#endif
                    filteredInput.SetInput(input, false);
                }
                else
                {
                    if (filteredInput.IsInput(input))
                    {
                        blockedInput = input;
                        block = true;
                    }
                }
            }
        }
    }
}