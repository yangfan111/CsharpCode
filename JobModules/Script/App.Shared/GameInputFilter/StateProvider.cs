using Core.GameInputFilter;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public class StateProvider : IStateProvider
    {
        private IPlayerStateAdapter _playerState;
        private IStatePool _statePool;

        public StateProvider(IPlayerStateAdapter player, IStatePool pool)
        {
            _playerState = player;
            _statePool = pool;
        }

        public HashSet<EPlayerState> GetCurrentStates()
        {
            return _playerState.GetCurrentStates(); 
        }

        public void ApplyStates(IGameInputProcessor processor)
        {
            var states = _playerState.GetCurrentStates();
            foreach(var state in states)
            {
                if(state == EPlayerState.None)
                {
                    continue;
                }
                processor.AddState(_statePool.GetState(state));
            }
            //处理复合状态
            if(states.Contains(EPlayerState.Prone) && (states.Contains(EPlayerState.Run) ||states.Contains(EPlayerState.Sprint) || states.Contains(EPlayerState.Walk)))
            {
                processor.AddState(_statePool.GetState(EPlayerState.ProneMove));
            }
        }
    }
}
