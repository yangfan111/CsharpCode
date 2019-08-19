using System.Collections.Generic;
using App.Client.GameModules.Player;
using App.Shared.Components.Player;
using BehaviorDesigner.Runtime.Tasks;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using Core.Components;
using Core.EntityComponent;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.GameModules.GamePlay
{
    public class ObserveSystem : AbstractGamePlaySystem<PlayerEntity>
    {

        private LoggerAdapter Logger = new LoggerAdapter(typeof(ObserveSystem));
        private Contexts _contexts;

        public ObserveSystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.CameraFinalOutputNew,
                PlayerMatcher.Position, PlayerMatcher.GamePlay));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }
        
        protected override void OnGamePlay(PlayerEntity player)
        {
            int ObserveEntityId = player.gamePlay.CameraEntityId;
            if (ObserveEntityId == 0) return;
            var observedPlayer =
                _contexts.player.GetEntityWithEntityKey(new EntityKey(ObserveEntityId, (short) EEntityType.Player));
            if (observedPlayer == null) return;

            player.observeCamera.MainNowMode = observedPlayer.cameraStateNew.MainNowMode;
            player.observeCamera.FreeNowMode = observedPlayer.cameraStateNew.FreeNowMode;
            player.observeCamera.PeekNowMode = observedPlayer.cameraStateNew.PeekNowMode;

            player.observeCamera.FreeYaw = observedPlayer.cameraStateUpload.FreeYaw;
            player.observeCamera.FreePitch = observedPlayer.cameraStateUpload.FreePitch;

            if (observedPlayer.cameraArchor.ArchorType == ECameraArchorType.Car)
            {
                player.observeCamera.VehicleId = observedPlayer.controlledVehicle.EntityKey.EntityId;
            }
            else
            {
                player.observeCamera.VehicleId = -1;
            }
            player.observeCamera.ObservedPlayerPosition = observedPlayer.cameraFinalOutputNew.Position;
        }
    }
}