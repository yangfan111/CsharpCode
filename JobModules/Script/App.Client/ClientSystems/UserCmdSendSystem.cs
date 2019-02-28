using System.Linq;
using App.Client.Cmd;
using App.Client.GameModules.Vehicle;
using App.Protobuf;
using App.Server;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.GameModules.Vehicle;
using App.Shared.Network;
using App.Shared.Player;
using Core.GameTime;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace App.Client.ClientSystems
{
    public class UserCmdSendSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UserCmdSendSystem));
        private IUserCmdGenerator _generator;
        private IVehicleCmdGenerator _vehicleCmdGenerator;


        private ITimeManager _timeManager;
      
        private PlayerContext _playerContext;
        private ISimulationTimer _simulationTimer;

        private Contexts _contexts;
     

        public UserCmdSendSystem(Contexts contexts)
        {
           

            _playerContext = contexts.player;

            _contexts = contexts;
        }

        protected override void InternalExecute()
        {

           

            var player = _playerContext.flagSelfEntity;
            if (player != null && player.hasFirstPersonModel && player.hasThirdPersonModel && player.hasUserCmd && player.hasNetwork)
            {
                var channel = player.network.NetworkChannel;
                /*命令使用usercmdsendsystem*/
            //    SendUserCmd(player, channel);
                SendVehicleCmd(player, channel);
            }
        }
       

        private void SendVehicleCmd(PlayerEntity player, INetworkChannel channel)
        {
            var controlledVehicle = PlayerVehicleUtility.GetControlledVehicle(player, _contexts.vehicle);
            if (controlledVehicle != null)
            {
             

                var list = controlledVehicle.vehicleCmd.CreateLatestList();

                if (channel != null && list.Value.Count > 0)
                {
                    channel.SendRealTime((int) EClient2ServerMessage.VehicleCmd, list);
                }

                list.ReleaseReference();
            }
        }

       
        private void SendUserCmd(PlayerEntity player, INetworkChannel channel)
        {
          
            var list = player.userCmd.CreateLatestList();

            if (channel != null)
            {
                channel.SendRealTime((int) EClient2ServerMessage.UserCmd, list);
            }

            list.ReleaseReference();

        }
    }
}
         