using Core.GameInputFilter;
using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public class GameState : IGameState
    {
        private EPlayerState _playerStae;
        private HashSet<EPlayerInput> _avaliableInputs;
        private HashSet<EPlayerInput> _unavaliableInputs = new HashSet<EPlayerInput>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance);
        public GameState(EPlayerState state, HashSet<EPlayerInput> inputList)
        {
            _playerStae = state;
            _avaliableInputs = inputList;
            if(null == _avaliableInputs)
            {
                _avaliableInputs = new HashSet<EPlayerInput>(CommonIntEnumEqualityComparer<EPlayerInput>.Instance); 
            }
            for(var e = EPlayerInput.None + 1; e < EPlayerInput.Length; e++)
            {
                if(!_avaliableInputs.Contains(e))
                {
                    _unavaliableInputs.Add(e);
                }
            }
        }
        
        public void FilterInput(IFilteredInput filteredInput)
        {
            foreach(var input in _unavaliableInputs)
            {
                filteredInput.BlockInput(input);
            }
        }

        public bool IsInputEnabled(EPlayerInput input)
        {
            return _avaliableInputs.Contains(input);
        }

        public bool IsState(EPlayerState State)
        {
            return _playerStae == State;
        }

        public EPlayerState State()
        {
            return _playerStae;
        }
    }
}
