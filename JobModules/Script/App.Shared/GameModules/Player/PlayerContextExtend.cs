using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Utils.Singleton;

namespace App.Shared.GameModules.Player
{

    internal class PlayerContextCache : DisposableSingleton<PlayerContextCache>
    {
        public IGroup<PlayerEntity> InitializedPlayerGroup;

        protected override  void OnDispose()
        {
            InitializedPlayerGroup = null;
        }
    }


    public static class PlayerContextExtend 
    {
        public static PlayerEntity[] GetInitializedPlayerEntities(this PlayerContext context)
        {
            var group = SingletonManager.Get<PlayerContextCache>().InitializedPlayerGroup;
            if (group == null)
            {
                group = context.GetGroup(PlayerMatcher.Initialized);
                SingletonManager.Get<PlayerContextCache>().InitializedPlayerGroup = group;
            }

            return group.GetEntities();
        }
    }
}
