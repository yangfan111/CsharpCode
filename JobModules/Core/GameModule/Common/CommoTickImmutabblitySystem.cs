using System;
using System.Collections.Generic;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.SessionState;
using Entitas;

namespace Core.GameModule.Common
{
    public class CommoTickImmutabblitySystem : AbstractStepExecuteSystem
    {
        private List<IGameGroup> _groups = new List<IGameGroup>();

        public CommoTickImmutabblitySystem(
            IGameContexts gameContexts)
        {
            foreach (var info in gameContexts.AllContexts)
            {
                if (info.CanContainComponent<FlagImmutabilityComponent>())
                {
                    _groups.Add(info.CreateGameGroup<FlagImmutabilityComponent>());
                }
            }
        }

        protected override void InternalExecute()
        {
            foreach (var info in _groups)
            {


                foreach (IGameEntity entity in info.GetGameEntities())
                {
                    var comp = entity.GetComponent<FlagImmutabilityComponent>();
                    comp.Tick((int)(Interval*1000));
                   
                }


            }
        }

       
    }
}
