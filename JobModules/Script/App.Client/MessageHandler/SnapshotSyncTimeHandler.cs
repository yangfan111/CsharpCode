using App.Client.Console.MessageHandler;
using Core.GameTime;
using Core.Replicaton;

namespace App.Client.Console
{
    public class SnapshotSyncTimeHandler : AbstractClientMessageHandler<Snapshot>
    {
        private ITimeManager _timeManager;

        public SnapshotSyncTimeHandler(ITimeManager timeManager)
        {
            _timeManager = timeManager;
        }

        public override void DoHandle(int messageType, Snapshot messageBody)
        {
            _timeManager.SyncWithServer(messageBody.ServerTime);
        }
    }

    
}