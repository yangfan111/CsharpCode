using System.Collections.Generic;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using Common;
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
        
            _sessionObjects.TimeManager.Tick(MyGameTime.time*1000f);
           
        }
    }
    public  class PrepareSnapshotPairSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PrepareSnapshotPairSystem));

        private ClientSessionObjectsComponent _sessionObjects;

        public PrepareSnapshotPairSystem(Contexts contexts)
        {
            _sessionObjects = contexts.session.clientSessionObjects;
        }
      // _sessionObjects.TimeManager.RenderTime即为当前服务器时间rednerTime

        protected override void InternalExecute()
        {
            var snapshotPool = _sessionObjects.SnapshotSelctor;
            var snapshotPair = snapshotPool.SelectSnapshot(_sessionObjects.TimeManager.RenderTime);
            _sessionObjects.PlaybackInfoProvider.Update(snapshotPair);


            if (snapshotPair != null)
            {

                _sessionObjects.TimeManager.UpdateFrameInterpolation(snapshotPair.LeftSnapshot.ServerTime, snapshotPair.RightSnapshot.ServerTime);
            }

        }
    }
    
}
