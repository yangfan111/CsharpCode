using App.Shared.SceneManagement;
using Core.GameModule.Interface;
using Entitas;

namespace App.Client.GameModules.Player
{
    public class PositionRelatedEffectUpdateSystem : AbstractRenderSystem<PlayerEntity>
    {
        private readonly IGroup<PlayerEntity> _players;
        private readonly IUpdatePositionRelatedEffect _effectUpdater;

        public PositionRelatedEffectUpdateSystem(Contexts contexts, IUpdatePositionRelatedEffect effectUpdater) :
            base(contexts)
        {
            _effectUpdater = effectUpdater;
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.StateInterface, PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return _effectUpdater != null;
        }

        protected override void OnRender(PlayerEntity entity)
        {
            _effectUpdater.UpdatePlayerPosition(entity.position.Value);
        }
    }
}