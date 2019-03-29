using System;
using System.Collections.Generic;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.SessionState;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.GameModule.Common
{
    public class CommonLifeTimeSystem : AbstractStepExecuteSystem
    {
        private List<IGameGroup> _groups = new List<IGameGroup>();

        public CommonLifeTimeSystem(
            IGameContexts gameContexts)
        {
            foreach (var info in gameContexts.AllContexts)
            {
                if (info.CanContainComponent<LifeTimeComponent>())
                {
                    _groups.Add(info.CreateGameGroup<LifeTimeComponent>());
                }
            }
        }

        protected override void InternalExecute()
        {
            try
            {
                var now = DateTime.Now;
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.LifeTime);
                foreach (var info in _groups)
                {
                    foreach (IGameEntity entity in info.GetGameEntities())
                    {
                        var lifeComp = entity.LifeTimeComponent;

                        if ((now - lifeComp.CreateTime).TotalMilliseconds > lifeComp.LifeTime)
                        {
                            if (!entity.IsDestroy)
                                entity.AddComponent<FlagDestroyComponent>();
                        }
                    }
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.LifeTime);
            }
        }
    }
}