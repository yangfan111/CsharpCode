using System;
using System.Collections.Generic;
using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public abstract class ReactiveResourceLoadSystem<T>: ReactiveSystem<T>, IResourceLoadSystem where T : Entity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ReactiveResourceLoadSystem<T>));
        private readonly CustomProfileInfo _info;
        protected ReactiveResourceLoadSystem(IContext<T> context) : base(context)
        {
            Activate();
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("SingleExecute");
        }

        protected ReactiveResourceLoadSystem(ICollector<T> collector) : base(collector)
        {
        }

        public ILoadRequestManager LoadRequestManager { get; private set; }

       
        public virtual void OnLoadResources(ILoadRequestManager coRoutineManager)
        {
            LoadRequestManager = coRoutineManager;
            
            Execute();
        }
        public abstract void SingleExecute(T entity);
        protected override void Execute(List<T> entities)
        {
            int count = entities.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    _info.BeginProfile();
                    SingleExecute(entities[i]);
                }
                catch (Exception e)
                {

                    _logger.ErrorFormat("SingleExecute error;{0}", e);
                }
                finally
                {
                    _info.EndProfile();
                }
            }
        }
    }
}