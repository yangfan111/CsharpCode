using System;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Player
{
    public abstract class AbstractRenderSystem<TEntity> : IRenderSystem where TEntity : class, IEntity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractRenderSystem<TEntity>));
        protected abstract  IGroup<TEntity> GetIGroup(Contexts contexts);
        
      
        protected virtual bool Filter(TEntity entity)
        {
            return true;
        }
        protected abstract void OnRender(TEntity entity);
        private IGroup<TEntity> _group;

        protected AbstractRenderSystem(Contexts contexts)
        {
            _group = GetIGroup(contexts);
        }

        protected virtual void BeforeOnRender()
        {
            
        }
        protected virtual void AfterOnRender()
        {
            
        }
        public void OnRender()
        {
            BeforeOnRender();
            foreach (var entity in _group.GetEntities())
            {
                try
                {
                    if (Filter(entity))
                    {
                        OnRender(entity);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Exception {0} {1}", e,entity);
                }
            }

            AfterOnRender();
        }
    }
}