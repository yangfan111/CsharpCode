using App.Shared;
using App.Shared.Components;
using App.Shared.Components.Player;
using Common;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SessionState;
using Core.Utils;

namespace App.Client.ClientSystems
{
    /// <summary>
    ///     在帧执行末尾同步
    /// </summary>
    public class UserCmdUpdateSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UserCmdUpdateSystem));
        private IUserCmdGenerator _generator;


        private PlayerContext _playerContext;

        private ClientSessionObjectsComponent _sessionObjects;
        private IVehicleCmdGenerator _vehicleCmdGenerator;

        public UserCmdUpdateSystem(Contexts contexts)
        {
            _sessionObjects = contexts.session.clientSessionObjects;

            _playerContext = contexts.player;
        }

        protected override void InternalExecute()
        {
            var player = _playerContext.flagSelfEntity;

            if (player != null && player.hasFirstPersonModel && player.hasThirdPersonModel && player.hasUserCmd &&
                player.hasNetwork)
            {
                var channel = player.network.NetworkChannel;
                SyncUserComponent(player);
                SendUpdateMessage(player, channel);
                player.uploadEvents.ReInit();
            }
        }

        private void SyncUserComponent(PlayerEntity player)
        {
            if (!player.hasSendUserCmd)
            {
                player.AddSendUserCmd();
            }

            if (player.userCmd.Latest != null)
                player.sendUserCmd.CopyForm(player.userCmd.Latest);
        }

        //只同步自己Entity
        private void SendUpdateMessage(PlayerEntity player, INetworkChannel channel)
        {
            if (player.userCmd.Latest == null) return;
            _sessionObjects.ClientUpdateLatestMgr.LastLocalUserCmd = player.userCmdSeq.LastCmdSeq;
            var pacakge = _sessionObjects.ClientUpdateLatestMgr.CreateClientUpdateLatestPacakge(player.entityKey.Value);
            if (channel != null)
            {
                channel.SendRealTime((int) EClient2ServerMessage.UpdateMsg, pacakge);
            }

            _logger.DebugFormat("SendUserCmd:seq:{0} msg:{1} last;{2}", MyGameTime.seq, pacakge.Head.LastUserCmdSeq,
                player.userCmd.Latest.Seq);
            pacakge.ReleaseReference();
        }
    }
}