using Core.GameInputFilter;
using System;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public class DummyFilteredInput : IFilteredInput
    {
        public void BlockInput(EPlayerInput input)
        {
        }

        public bool IsInput(EPlayerInput input)
        {
            return false;
        }

        public bool IsInputBlocked(EPlayerInput input)
        {
            return false;
        }

        public void Reset()
        {
        }

        public void SetInput(EPlayerInput input, bool val)
        {
        }
    }
}
