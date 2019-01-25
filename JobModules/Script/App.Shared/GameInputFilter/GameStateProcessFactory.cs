using Core.GameInputFilter;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameInputFilter
{
    public class GameStateProcessFactory : IGameStateProcessorFactory
    {
        private IStateProviderPool _stateProviderPool;
        private IStatePool _statePool;

        public IStateProviderPool GetProviderPool()
        {
            if(null == _stateProviderPool)
            {
                _stateProviderPool = new StateProviderPool();
            }
            return _stateProviderPool;
        }

        public IStatePool GetStatePool()
        {
            if(null == _statePool)
            {
                _statePool = new GameStatePool(SingletonManager.Get<StateTransitionConfigManager>().GetTransitons());
            }
            return _statePool;
        }
    }
}
