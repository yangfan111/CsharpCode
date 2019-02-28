using Core.Utils;
using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public interface IPlayerStateMatrix
    {
        void Init();
        void IsEnabled(EPlayerState state, EPlayerInput input);
    }

    public class PlayerStateMatrix
    {
        Dictionary<EPlayerState, HashSet<EPlayerInput>> _conditions = new Dictionary<EPlayerState, HashSet<EPlayerInput>>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);

        public void AddCondition(EPlayerState state, EPlayerInput input)
        {
            if (!_conditions.ContainsKey(state))
            {
                _conditions[state] = new HashSet<EPlayerInput>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);
            }
            _conditions[state].Add(input);
        }

        public bool IsEnabled(EPlayerState state, EPlayerInput input)
        {
            if(!_conditions.ContainsKey(state))
            {
                return false;
            }
            if(!_conditions[state].Contains(input))
            {
                return false;
            }
            return true;
        }
    }
}