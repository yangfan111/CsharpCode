using System.Collections.Generic;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.ClientSystems
{
    public  class DriveTimeSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(DriveTimeSystem));

        private ClientSessionObjectsComponent _sessionObjects;

        public DriveTimeSystem(Contexts contexts)
        {
            _sessionObjects = contexts.session.clientSessionObjects;
        }
      
        protected override void InternalExecute()
        {
        
            _sessionObjects.TimeManager.Tick(Time.time*1000f);
            var snapshotSelector = _sessionObjects.SnapshotSelectorContainer.SnapshotSelector;
            var snapshotPair = snapshotSelector.SelectSnapshot(_sessionObjects.TimeManager.RenderTime);
            _sessionObjects.PlaybackInfoProvider.Update(snapshotPair, snapshotSelector.LatestSnapshot);


            if (snapshotPair != null)
            {
                if (!SharedConfig.IsOffline && snapshotPair.RightSnapshot.ServerTime < _sessionObjects.TimeManager.RenderTime)
                {
                    SingletonManager.Get<DurationHelp>().IncDriveTimeCount();
                    _logger.InfoFormat("The Client Render Time {0} is Larger than Server Time {1}.",
                    _sessionObjects.TimeManager.RenderTime, snapshotPair.RightSnapshot.ServerTime);
                   
                }

                _sessionObjects.TimeManager.UpdateFrameInterpolation(snapshotPair.LeftSnapshot.ServerTime, snapshotPair.RightSnapshot.ServerTime);
            }

        }
    }
    
}
