using Core;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class EmptyFilteredInput : IFilteredInput
    {
        public void BlockInput(EPlayerInput input)
        {
        }

        public bool IsInput(EPlayerInput input)
        {
            return false;
        }

        public  bool IsInputBlocked(EPlayerInput input)
        {
            return false;
        }

        public  void ResetInputStates()
        {
        }

        public void SetInput(EPlayerInput input, bool val)
        {
            
        }
    }
}
