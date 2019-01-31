using System.Collections.Generic;
using Core.Components;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.GameModule.System;
using Core.SessionState;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.GameModule.Common
{
    public class CommonDestroySystem : AbstractStepExecuteSystem
    {
        private List<IGameGroup> _groups = new List<IGameGroup>();
        private ICommonSessionObjects _sessionObjects;

        public CommonDestroySystem(ICommonSessionObjects sessionObjects)
        {
            _sessionObjects = sessionObjects;

            foreach (var info in _sessionObjects.GameContexts.AllContexts)
            {
                if (info.CanContainComponent<FlagDestroyComponent>())
                {
                    _groups.Add(info.CreateGameGroup<FlagDestroyComponent>());
                }
            }
        }

        protected override void InternalExecute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Destroy);
                foreach (var info in _groups)
                {
                    foreach (IGameEntity entity in info.GetGameEntities())
                    {
                        foreach (var comp in entity.AssetComponents)
                        {
                           
                            comp.Recycle(_sessionObjects.AssetManager);
                            
                        }

                        if (_sessionObjects.AssetManager != null)
                            _sessionObjects.AssetManager.LoadCancel(entity.RealEntity);
                        entity.Destroy();
                    }
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Destroy);
            }
        }
    }
}