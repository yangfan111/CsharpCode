using System.Text;
using Core;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class ValuableFilteredInput : IFilteredInput
    {
        //readonly Dictionary<EPlayerInput, bool> _inputDic = new Dictionary<EPlayerInput, bool>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);
        //当前输出状态
        readonly bool []inputStates = new bool[(int) EPlayerInput.Length];

        public bool IsInput(EPlayerInput input)
        {
            return inputStates[(int) input];
        }
        public void SetInput(EPlayerInput input, bool val)
        {
            inputStates[(int) input] = val;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach(var v in inputStates)
            {
                if (v)
                    sb.AppendFormat("key:{0}",(EPlayerInput)i);
                i++;
            }
            return sb.ToString();
        }
        public void CopyTo(IFilteredInput filteredInput)
        {
            if (filteredInput is ValuableFilteredInput)
            {
                for (int i = 0; i < inputStates.Length; i++)
                {
                    filteredInput.SetInput((EPlayerInput) i, inputStates[i]);
                }
            }
           
        }
    }
}