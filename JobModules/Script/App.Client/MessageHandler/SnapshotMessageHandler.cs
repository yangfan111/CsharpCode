using App.Client.Console.MessageHandler;
using Common;
using Core.GameTime;
using Core.Replicaton;
using Core.UpdateLatest;
using Core.Utils;

namespace App.Client.Console
{
    public class SnapshotMessageHandler : AbstractClientMessageHandler<Snapshot>
    {
        private ISnapshotSelector _pool;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SnapshotMessageHandler));
        private bool _first = true;
        private ITimeManager _timeManager;
        private ClientUpdateLatestManager _clientUpdateMgr;
        public SnapshotMessageHandler(ISnapshotSelector pool, ClientUpdateLatestManager clientUpdateMgr,
            ITimeManager timeManager)
        {
            _pool = pool;
            _timeManager = timeManager;
            _clientUpdateMgr = clientUpdateMgr;

        }

        public override void DoHandle(int messageType, Snapshot messageBody)
        {
            if (_first)
            {
                _first = false;
                _logger.InfoFormat("received first snapshot time{0}, seq {1}", messageBody.ServerTime, messageBody.SnapshotSeq);
            }

            
//                _logger.ErrorFormat("received snapshot seq {0}, simTime {3}, entity count {1}, self {2} ",
//                    messageBody.SnapshotSeq,
//                    messageBody.EntityMap.Count,
//                    messageBody.Self,
//                    messageBody.VehicleSimulationTime);
            

            if (_timeManager.RenderTime - messageBody.ServerTime > 0)
            {
                _logger.InfoFormat("The client render time {0} advances received server time {1}.",
                    _timeManager.RenderTime, messageBody.ServerTime);
            }

            _logger.DebugFormat("DoHandle :seq:{0} usercmd:{1} snapshotId:{2}", MyGameTime.seq, messageBody.LastUserCmdSeq, messageBody.SnapshotSeq);
              
            _clientUpdateMgr.LastAckUserCmdSeq = messageBody.LastUserCmdSeq;
            _clientUpdateMgr.LastAckSnapshotId = messageBody.SnapshotSeq;
            _pool.AddSnapshot(messageBody);
        }
    }

    
}