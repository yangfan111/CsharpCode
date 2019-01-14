
using System;
using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace Core.GameModule.System
{
    public abstract class ReactiveEntityCleanUpSystem<T>: ReactiveSystem<T>, IEntityCleanUpSystem where T : Entity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof( ReactiveEntityCleanUpSystem<T>));
        protected ReactiveEntityCleanUpSystem(IContext<T> context) : base(context)
        {
            Activate();
        }

        public void OnEntityCleanUp()
        {
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
                    SingleExecute(entities[i]);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("SingleExecute error;{0}",e);
                }
            }
        }
    }
}