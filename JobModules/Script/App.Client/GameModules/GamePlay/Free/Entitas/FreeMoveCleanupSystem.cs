using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Sources.Free.Effect;
using Core.GameModule.Interface;
using Entitas;
using App.Client.GameModules.Free;
using App.Client.GameModules.GamePlay.Free.Utility;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Entitas
{
    public class FreeMoveCleanupSystem : IEntityCleanUpSystem
    {
        private IGroup _group;
        public FreeMoveCleanupSystem(FreeMoveContext freeMoveContext)
        {
            _group = freeMoveContext.GetGroup(FreeMoveMatcher.AnyOf(FreeMoveMatcher.FlagDestroy));
        }

        public void OnEntityCleanUp()
        {
            foreach (FreeMoveEntity entity in _group)
            {

                var effect = SingletonManager.Get<FreeEffectManager>().GetEffect("freemove_" + entity.entityKey.Value.EntityId);
                if (null != effect)
                {
                    SingletonManager.Get<FreeEffectManager>().RemoveEffect(effect);
                    //FreeGlobalVars.Loader.AddToGameObjectPool(effect.gameObject);
                    foreach(IFreeEffect e in effect.GetEffects())
                    {
                        e.Recycle();
                    }
                }
            }
        }
    }
}
