using System;
using Core.SessionState;
using Core.SyncLatest;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class SyncLatestSystem : AbstractStepExecuteSystem
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(SyncLatestSystem));

        private SyncLastestManager _netSyncManager;

        public SyncLatestSystem(SyncLastestManager netSyncManager)
        {
            _logger.Info("start");
            _netSyncManager = netSyncManager;
        }


        protected override void InternalExecute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SyncLatest);
                _netSyncManager.SyncLatest();
            }
            catch (Exception e)
            {

                _logger.ErrorFormat("FreeGameRuleSystem error executing {0}", e);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SyncLatest);
            }
        }
    }
}