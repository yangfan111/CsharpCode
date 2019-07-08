using App.Client.GameModules.Player;
using App.Shared;
using Core.EntityComponent;
using Entitas;

namespace App.Client.GameModules.Ui.System
{
    public class ObserveUISystem : AbstractGamePlaySystem<PlayerEntity>
    {
        private PlayerEntity _observePlayer;
        private int cameraId;
        private Contexts _contexts;

        public ObserveUISystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(
                PlayerMatcher.GamePlay, PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.isFlagSelf && cameraId != entity.gamePlay.CameraEntityId;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
            cameraId = entity.gamePlay.CameraEntityId;

            if (cameraId == 0)
            {
                _contexts.ui.uI.Player = _contexts.player.flagSelfEntity;
                return;
            }

            var observedPlayer =
                _contexts.player.GetEntityWithEntityKey(new EntityKey(cameraId, (short) EEntityType.Player));
            if (observedPlayer != null)
            {
                _contexts.ui.uI.Player = observedPlayer;
            }
            else
            {
                _contexts.ui.uI.Player = _contexts.player.flagSelfEntity;
            }
        }
    }
}