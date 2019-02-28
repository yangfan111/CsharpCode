using System.Collections.Generic;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectCleanUpSystem : ReactiveEntityCleanUpSystem<ClientEffectEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientEffectCleanUpSystem));

        public ClientEffectCleanUpSystem(ClientEffectContext context):base(context)
        {
        }

        public override void SingleExecute(ClientEffectEntity entity)
        {
            foreach(var asset in entity.assets.LoadedAssets)
            {
                var go = asset.Value.AsGameObject;
                if(null == go)
                {
                    Logger.Error("some assets is not gameobject in client effect load objects");
                    continue;
                }
                go.transform.parent = null;
            }
        }

        protected override bool Filter(ClientEffectEntity entity)
        {
            return entity.hasAssets;
        }
        protected override ICollector<ClientEffectEntity> GetTrigger(IContext<ClientEffectEntity> context)
        {
            return context.CreateCollector(ClientEffectMatcher.FlagDestroy.Added());
        }
    }
}
