using System;
using Core.GameModule.System;
using Core.SessionState;
using Core.Utils;
using Entitas;
using Utils.AssetManager;
using Utils.Singleton;

namespace Core.AssetManager
{
    public class LoadRequestManagerSystem : AbstractStepExecuteSystem
    {
        
        private  static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(LoadRequestManagerSystem));
        private LoadRequestManagerDriver _driver;
        public LoadRequestManagerSystem(
            ICommonSessionObjects commonSesionObjects)
        {
            _driver = new LoadRequestManagerDriver(
                commonSesionObjects.LoadRequestManager, 
                commonSesionObjects.GameObjectPool, 
                commonSesionObjects.AssetPool);
        }


        protected override void InternalExecute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.LoadRequestManager);
                _driver.Execute();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("LoadRequestManagerSystem Exception:{0}", e);

            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.LoadRequestManager);
            }
        }

       
    }
}
