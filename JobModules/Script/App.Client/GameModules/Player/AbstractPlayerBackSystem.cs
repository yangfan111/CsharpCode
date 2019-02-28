using System;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace App.Client.GameModules.Player
{
    public abstract class AbstractPlayerBackSystem<TEntity> : IPlaybackSystem where TEntity : class, IEntity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractPlayerBackSystem<TEntity>));
        protected abstract  IGroup<TEntity> GetIGroup(Contexts contexts);
        private CustomProfileInfo _info;
       

        /*
         * 如果为true就不进行下一步
         */
        protected virtual bool Filter(TEntity entity)
        {
            return true;
        }
        protected abstract void OnPlayBack(TEntity entity);
        protected IGroup<TEntity> _group;

        protected AbstractPlayerBackSystem(Contexts contexts)
        {
            _group = GetIGroup(contexts);
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("OnPlayBack");
        }

        protected virtual void BeforeOnPlayback()
        {
            
        }
        protected virtual void AfterOnPlayback()
        {
            
        }
        
        public void OnPlayback()
        {
            BeforeOnPlayback();
            try
            {
                foreach (var entity in _group.GetEntities())
                {
                    try
                    {
                        if (Filter(entity))
                        {
                            try
                            {
                                _info.BeginProfileOnlyEnableProfile();
                                OnPlayBack(entity);
                            }
                            finally
                            {
                                _info.EndProfileOnlyEnableProfile();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("Exception {0}", e);
                    }
                }
            }
            finally
            {
                AfterOnPlayback();
            }
        }
    }
}