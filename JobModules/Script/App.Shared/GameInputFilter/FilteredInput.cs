using Core.GameInputFilter;
using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameInputFilter
{

    public class FilteredInput : IFilteredInput
    {
        Dictionary<EPlayerInput, bool> _inputDic = new Dictionary<EPlayerInput, bool>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);
        HashSet<EPlayerInput> _blockedInputs = new HashSet<EPlayerInput>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);

        public bool IsInput(EPlayerInput input)
        {
            if (_inputDic.ContainsKey(input))
            {
                return _inputDic[input];
            }
            return false;
        }

        public void SetInput(EPlayerInput input, bool val)
        {
            _inputDic[input] = val;
            _blockedInputs.Remove(input);
        }

        public void Reset()
        {
            for (var e = EPlayerInput.None + 1; e < EPlayerInput.Length; e++)
            {
                _inputDic[e] = false;
            }
        }

        public void CopyTo(IFilteredInput input)
        {
            if (null == input)
            {
                return;
            }
            foreach (var pair in _inputDic)
            {
                input.SetInput(pair.Key, pair.Value);
            }
        }

        public bool IsInputBlocked(EPlayerInput input)
        {
            return _blockedInputs.Contains(input);
        }

        public void BlockInput(EPlayerInput input)
        {
            _inputDic[input] = false;
            _blockedInputs.Add(input);
        }
    }
}
