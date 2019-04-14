using Core;
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
            foreach (var input in blockedInputs)
            {
                if (block)
                {
                    filteredInput.SetInput(input, false);
                }
                else
                {
                    if (filteredInput.IsInput(input))
                    {
                        block = true;
                    }
                }
            }
        }
    }
}