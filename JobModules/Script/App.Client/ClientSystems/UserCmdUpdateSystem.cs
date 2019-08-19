using App.Shared;
using App.Shared.Components;
using App.Shared.Components.Player;
using Common;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SessionState;
using Core.UpdateLatest;
using Core.Utils;

namespace App.Client.ClientSystems
{


    public class UserCmdUpdateSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UserCmdUpdateSystem));
        private IUserCmdGenerator _generator;
        private IVehicleCmdGenerator _vehicleCmdGenerator;

        private IUpdateLatestManager _updateLatestManager;
      
        private PlayerContext _playerContext;

        private Contexts _contexts;
        private ClientSessionObjectsComponent _sessionObjects;

        public UserCmdUpdateSystem(Contexts contexts)
        {
            _sessionObjects = contexts.session.clientSessionObjects;

            _playerContext = contexts.player;
            _updateLatestManager = new UpdateLatestManager(_sessionObjects.UpdateLatestHandler);
            _contexts = contexts;
        }

        protected override void InternalExecute()
        {

            var player = _playerContext.flagSelfEntity;

            if (player != null && player.hasFirstPersonModel && player.hasThirdPersonModel && player.hasUserCmd && player.hasNetwork)
            {
                var channel = player.network.NetworkChannel;
                SyncUpdateMessage(player);
                SendUpdateMessage(player, channel);
                player.uploadEvents.ReInit();
            }
        }

        private void SyncUpdateMessage(PlayerEntity player)
        {
            if (!player.hasSendUserCmd)
            {
                player.AddSendUserCmd();
            }
            if(player.userCmd.Latest!=null)

            player.sendUserCmd.CopyForm(player.userCmd.Latest);
        }


        private void SendUpdateMessage(PlayerEntity player, INetworkChannel channel)
        {
            if (player.userCmd.Latest == null) return;
            _sessionObjects.UpdateLatestHandler.UserCmd = player.userCmd.Latest.Seq;

            var pacakge = _updateLatestManager.CreateUpdateSnapshot(player.entityKey.Value);
            if (channel != null)
            {
                channel.SendRealTime((int) EClient2ServerMessage.UpdateMsg, pacakge);
            }
            _logger.DebugFormat("SendUserCmd:seq:{0} msg:{1} last;{2}", MyGameTime.seq, pacakge.Head.UserCmdSeq,  player.userCmd.Latest.Seq);
            pacakge.ReleaseReference();
        }
    }
}