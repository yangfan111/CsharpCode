using System;
using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace Core.GameModule.System
{
    public abstract class ReactiveGamePlaySystem<T>: ReactiveSystem<T>, IGamePlaySystem where T : Entity
    {  
        
        protected ReactiveGamePlaySystem(IContext<T> context) : base(context)
        {
            Activate();
        }


        public void OnGamePlay()
        {
            Execute();
        }
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ReactiveGamePlaySystem<T>));
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