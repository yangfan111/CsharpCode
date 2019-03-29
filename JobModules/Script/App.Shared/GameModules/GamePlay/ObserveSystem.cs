using System.Collections.Generic;
using App.Client.GameModules.Player;
using App.Shared.Components.Player;
using BehaviorDesigner.Runtime.Tasks;
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

        protected override void BeforeOnGamePlay()
        {
            foreach (var entity in GetIGroup(_contexts).GetEntities())
            {
                entity.gamePlay.BeingObserved = false;
            }
        }

        protected override void AfterOnGamePlay()
        {
            foreach (var entity in GetIGroup(_contexts).GetEntities())
            {
                if (!entity.gamePlay.BeingObserved)
                {
                    if (entity.hasObserveCamera)
                        entity.RemoveObserveCamera();
                }
            }
        }
        
        protected override void OnGamePlay(PlayerEntity entity)
        {
            UpdateValue(entity);
        }

        private void UpdateValue(PlayerEntity player)
        {
            int ObserveEntityId = player.gamePlay.CameraEntityId;
            if (ObserveEntityId == 0) return;
            var observedPlayer =
                _contexts.player.GetEntityWithEntityKey(new EntityKey(ObserveEntityId, (short) EEntityType.Player));
            if (observedPlayer == null) return;
            observedPlayer.gamePlay.BeingObserved = true;
            
            if (!observedPlayer.hasObserveCamera)
            {
                observedPlayer.AddObserveCamera();
            }

            observedPlayer.observeCamera.Fov = observedPlayer.cameraFinalOutputNew.Fov;
            observedPlayer.observeCamera.CameraPosition =
                observedPlayer.cameraStateUpload.ThirdPersonCameraPostion;
            observedPlayer.observeCamera.CameraEularAngle = observedPlayer.cameraFinalOutputNew.EulerAngle;
            observedPlayer.observeCamera.PlayerPosition = observedPlayer.position.Value; 
        }
    }
}