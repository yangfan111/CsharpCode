using Core.GameInputFilter;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared.GameInputFilter
{
    public class GameInputProcessor : IGameInputProcessor
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GameInputProcessor));
        private List<IGameState> _gameStates = new List<IGameState>();
        private IFilteredInput _userInput;
        private IFilteredInput _dummyUserInput;
        private UserCommandMapper _userCmdMapper;
        private IStateProvider _stateProvider;

        public GameInputProcessor(UserCommandMapper mapper, IStateProvider provider, IFilteredInput input, IFilteredInput dummyInput)
        {
            _userCmdMapper = mapper;
            _userInput = input;
            _stateProvider = provider;
            _dummyUserInput = dummyInput;
        }

        public void Init()
        {
            _gameStates.Clear();
        }

        public IFilteredInput Filter()
        {
            if(null == _stateProvider)
            {
                return null;
            }
            _stateProvider.ApplyStates(this);
            var userInput = _userInput as IFilteredInput;
            foreach(var state in _gameStates)
            {
                if(null != state)
                {
                    state.FilterInput(userInput);
                }
                else
                {
                    Logger.Error("state is null !");
                }
            }
            return userInput;
        }

        public void AddState(IGameState gameState)
        {
            _gameStates.Add(gameState);
        }
        
        public void ResetStates()
        {
            _gameStates.Clear();
        }

        public bool IsInputEnalbed(EPlayerInput input)
        {
            foreach(var state in _gameStates)
            {
                if(!state.IsInputEnabled(input))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetUserCmd(IUserCmd cmd)
        {
            _userCmdMapper.Map(cmd, _userInput);
        }

        public IFilteredInput GetFilteredInput()
        {
            return _userInput;
        }

        public IFilteredInput DummyInput()
        {
            return _dummyUserInput;
        }
    }
}
