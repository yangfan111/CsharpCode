using Core.EntityComponent;
using Core;
using System.Collections.Generic;
using Utils.Utils;

namespace App.Shared.GameModules.Player
{
    [System.Obsolete]
    public class PlayerStateCollectorPool
    {


        private Dictionary<EntityKey, IPlayerStateColltector> collectorPool =
            new Dictionary<EntityKey, IPlayerStateColltector>(new EntityKeyComparer());

        public void AddStateCollector(EntityKey key,IPlayerStateColltector value)
        {
            collectorPool[key] = value;
        }

        public void Update()
        {
           // collectorPool.ForEach((collector=>collector.Value.Update()));
        }
        public IPlayerStateColltector GetStateCollector(EntityKey key)
        {
            IPlayerStateColltector colltector;
            collectorPool.TryGetValue(key, out colltector);
            return colltector;
        }
    }
}