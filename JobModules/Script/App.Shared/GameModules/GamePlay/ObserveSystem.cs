using System.Collections.Generic;
using App.Client.GameModules.Player;
using App.Shared.Components.Player;
using Core.CameraControl.NewMotor;
using Core.EntityComponent;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.GamePlay
{
    public class ObserveSystem:AbstractGamePlaySystem<PlayerEntity>
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
        
        protected override void OnGamePlay(PlayerEntity entity)
        {
            if (!entity.gamePlay.IsObserving())
            {
                if(entity.hasObserveCamera)
                    entity.RemoveObserveCamera();
            }
            else
            {   
                UpdateValue(entity);
            }
        }

        private void UpdateValue(PlayerEntity player)
        {
            int ObservedEntityId = player.gamePlay.CameraEntityId;
            if (ObservedEntityId == 0) return;
            var observePlayer =
                _contexts.player.GetEntityWithEntityKey(new EntityKey(ObservedEntityId, (short) EEntityType.Player));
            if (observePlayer == null) return;
            if (!observePlayer.hasObserveCamera)
            {
                observePlayer.AddObserveCamera();
            }
            
            if (observePlayer != null)
            {
                player.gamePlay.CameraEntityId = ObservedEntityId;

                observePlayer.observeCamera.Fov = observePlayer.cameraFinalOutputNew.Fov;
                observePlayer.observeCamera.CameraPosition = observePlayer.appearanceInterface.Appearance.IsFirstPerson
                    ? observePlayer.thirdPersonDataForObserving.ThirdPersonArchorPosition
                    : observePlayer.cameraFinalOutputNew.Position;
                observePlayer.observeCamera.CameraEularAngle = observePlayer.cameraFinalOutputNew.EulerAngle;
                
                observePlayer.observeCamera.PlayerPosition = observePlayer.position.Value;
                observePlayer.observeCamera.IsFirstAppearance = observePlayer.appearanceInterface.Appearance.IsFirstPerson;
            }
        }
        
    }
}