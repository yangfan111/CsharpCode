using App.Client.GameModules.Player;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class ClientPlayerDebugDrawBoxSystem: AbstractGamePlaySystem<PlayerEntity>
    {
        public ClientPlayerDebugDrawBoxSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.CharacterContoller, PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return SharedConfig.ShowCharacterBoundingBox || SharedConfig.ShowGroundInfo;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
            if (SharedConfig.ShowCharacterBoundingBox)
            {
                entity.characterContoller.Value.DrawBoundingBox();
                entity.characterContoller.Value.DrawLastGroundHit();
            }

            if (SharedConfig.ShowGroundInfo)
            {
                entity.characterContoller.Value.DrawGround();
            }

        }
    }
}