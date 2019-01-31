using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace Core.GameModule.System
{
    public abstract class ReactiveEntityInitSystem<T>: ReactiveSystem<T>, IEntityInitSystem where T : Entity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof( ReactiveEntityInitSystem<T>));
        protected ReactiveEntityInitSystem(IContext<T> context) : base(context)
        {
            Activate();
        }

        public void OnEntityInit()
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
                    _logger.ErrorFormat("Exception :{0}",e);
                   
                }
            }
        }
    }
}