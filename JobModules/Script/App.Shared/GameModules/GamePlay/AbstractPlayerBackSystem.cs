using System;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Player
{
    public abstract class AbstractGamePlaySystem<TEntity> : IGamePlaySystem where TEntity : class, IEntity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractGamePlaySystem<TEntity>));
        protected abstract IGroup<TEntity> GetIGroup(Contexts contexts);
        
        /*
         * 如果为true就不进行下一步
         */
        protected abstract bool Filter(TEntity entity);
        protected abstract void OnGamePlay(TEntity entity);
        private IGroup<TEntity> _group;

        protected AbstractGamePlaySystem(Contexts contexts)
        {
            _group = GetIGroup(contexts);
        }

        protected virtual void BeforeOnGamePlay()
        {
            
        }
        protected virtual void AfterOnGamePlay()
        {
            
        }
      

        public void OnGamePlay()
        {
            BeforeOnGamePlay();
            foreach (var entity in _group.GetEntities())
            {
                try
                {
                    if (Filter(entity))
                    {
                        OnGamePlay(entity);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Exception {0}", e);
                }
            }

            AfterOnGamePlay();
        }
    }
}