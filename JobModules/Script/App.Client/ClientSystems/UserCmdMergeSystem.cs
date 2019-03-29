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
    public class UserCmdMergeSystem : AbstractStepExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UserCmdMergeSystem));
        private IVehicleCmdGenerator _vehicleCmdGenerator;
        private PlayerContext _playerContext;
        private ISimulationTimer _simulationTimer;
        private int _userCmdSeq;

        private Contexts _contexts;

        public UserCmdMergeSystem(Contexts contexts)
        {
            _vehicleCmdGenerator = new UnityVehicleCmdGenerator(contexts.userInput.userInputManager.Instance);
            _playerContext = contexts.player;

            _simulationTimer = contexts.session.clientSessionObjects.SimulationTimer;

            _contexts = contexts;
        }

        protected override void InternalExecute()
        {
            var player = _playerContext.flagSelfEntity;
            if (player != null && player.hasFirstPersonModel && player.hasThirdPersonModel && player.hasUserCmd)
            {
                CreateVehicleCmd(player);
                MergeUserCmd(player);

                MergeVehicleCmd(player);
            }
        }

        private void CreateVehicleCmd(PlayerEntity player)
        {
            var controlledVehicle = PlayerVehicleUtility.GetControlledVehicle(player, _contexts.vehicle);
            if (controlledVehicle != null)
            {
                var vehicleCmd = VehicleEntityUtility.CreateVehicleCmd(_vehicleCmdGenerator, _contexts.vehicle, player,
                    _simulationTimer.CurrentTime);

                if (vehicleCmd != null)
                {
                    controlledVehicle.vehicleCmd.AddStepTemp(vehicleCmd);
                }
            }
        }

        private int vehicleCmdSeq;

        private void MergeVehicleCmd(PlayerEntity player)
        {
            var controlledVehicle = PlayerVehicleUtility.GetControlledVehicle(player, _contexts.vehicle);
            if (controlledVehicle != null)
            {
                var tempList = controlledVehicle.vehicleCmd.UserCmdStepList;
                if (tempList.Count == 0) return;
                var last = tempList.Last();
                last.AcquireReference();
                foreach (var cmd in tempList)
                {
                    cmd.ReleaseReference();
                }

                tempList.Clear();
                controlledVehicle.vehicleCmd.AddLatest(last);

                last.ReleaseReference();
            }
        }

        class DummyPlayerEntityUserCmdOwnApapter : IUserCmdOwnAdapter
        {
            private PlayerEntity playerEntity;

            public DummyPlayerEntityUserCmdOwnApapter(PlayerEntity playerEntity)
            {
                this.playerEntity = playerEntity;
            }

            public float Yaw
            {
                get { return playerEntity.orientation.Yaw; }
            }

            public float Pitch
            {
                get { return playerEntity.orientation.Pitch; }
            }

            public Vector3 Position
            {
                get { return playerEntity.position.Value; }
            }

            public Transform PlayerTransform
            {
                get { return playerEntity.RootGo().transform; }
            }
        }


        private void MergeUserCmd(PlayerEntity player)
        {
            var tempList = player.userCmd.UserCmdStepList;


            UserCmd last = UserCmd.Allocate();
            last.ReInit();

            ((UserCmd) tempList.Last()).CopyTo(last);
            last.Seq = _userCmdSeq++;
            last.FrameInterval = 0;
            last.DeltaYaw = 0;
            last.DeltaPitch = 0;

            foreach (var cmd in tempList)
            {
                last.DeltaYaw += cmd.DeltaYaw;
                last.DeltaPitch += cmd.DeltaPitch;
                last.FrameInterval += cmd.FrameInterval;
                last.Buttons |= cmd.Buttons;
                last.RenderTime = cmd.RenderTime;
                last.ClientTime = cmd.ClientTime;
                last.SnapshotId = cmd.SnapshotId;
                last.PredicatedOnce = cmd.PredicatedOnce;
                last.MoveHorizontal = cmd.MoveHorizontal;
                last.MoveVertical = cmd.MoveVertical;
                last.MoveUpDown = cmd.MoveUpDown;

                last.ChangedSeat = cmd.ChangedSeat > 0 ? cmd.ChangedSeat : last.ChangedSeat;
                last.ChangeChannel = cmd.ChangeChannel > 0 ? cmd.ChangeChannel : last.ChangeChannel;
                last.CurWeapon = cmd.CurWeapon == 0 ? last.CurWeapon : cmd.CurWeapon;
                last.PickUpEquip = cmd.PickUpEquip > 0 ? cmd.PickUpEquip : last.PickUpEquip;
                last.UseEntityId = cmd.UseEntityId > 0 ? cmd.UseEntityId : last.UseEntityId;
                last.UseVehicleSeat = cmd.UseVehicleSeat > 0 ? cmd.UseVehicleSeat : last.UseVehicleSeat;
                last.UseType = cmd.UseType > 0 ? cmd.UseType : last.UseType;
                last.BagIndex = cmd.BagIndex > 0 ? cmd.BagIndex : last.BagIndex;

                //_logger.ErrorFormat("cmd;{0}   {1}",cmd.CurWeapon,last.CurWeapon);
                cmd.ReleaseReference();
            }

            tempList.Clear();

            player.userCmd.AddLatest(last);

            last.ReleaseReference();
        }
    }
}