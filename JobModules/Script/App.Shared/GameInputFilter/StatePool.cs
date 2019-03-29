using Core.GameInputFilter;
using Core.Utils;
using System.Collections.Generic;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public class GameStatePool : IStatePool
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GameStatePool));
        private readonly Dictionary<EPlayerState, IGameState> _gameStateDic = new Dictionary<EPlayerState, IGameState>(CommonIntEnumEqualityComparer<EPlayerState>.Instance);

        public GameStatePool(Dictionary<EPlayerState, HashSet<EPlayerInput>> datas)
        {
            foreach(var data in datas)
            {
                _gameStateDic[data.Key] = new GameState(data.Key, data.Value);
            }
        }

        public void Reload(Dictionary<EPlayerState, HashSet<EPlayerInput>> datas)
        {
            _gameStateDic.Clear();
            foreach(var data in datas)
            {
                _gameStateDic[data.Key] = new GameState(data.Key, data.Value);
            }
        }

        public IGameState GetState(EPlayerState state)
        {
            if(!_gameStateDic.ContainsKey(state) && state != EPlayerState.None)
            {
                Logger.ErrorFormat("state {0} doesn't exist in game state dic ", state);
                return null;
            }
            return _gameStateDic[state];
        }
    }
}
