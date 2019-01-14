using Core.EntityComponent;
using Core.GameInputFilter;
using System.Collections.Generic;

namespace App.Shared.GameInputFilter
{
    public class StateProviderPool : IStateProviderPool
    {
        private Dictionary<EntityKey, StateProvider> _providerDic = new Dictionary<EntityKey, StateProvider>(new EntityKeyComparer());

        public void AddStateProvider(PlayerEntity player, IStatePool pool)
        {
            var provider = new StateProvider(new PlayerStateAdapter(player), pool);
            _providerDic[player.entityKey.Value] = provider;
            if(!player.hasPlayerStateProvider)
            {
                player.AddPlayerStateProvider(provider);
            }
        }

        public IStateProvider GetStateProvider(EntityKey key)
        {
            if(_providerDic.ContainsKey(key))
            {
                return _providerDic[key];
            }
            return null;
        }
    }
}
