using App.Client.GameModules.Player;
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using Core;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class ClientPlayerUpdateWeaponSystem: AbstractGamePlaySystem<PlayerEntity>
    {
        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.PlayerClientUpdate,
            PlayerMatcher.GamePlay,PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity != null && entity.gamePlay.UIStateUpdate;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
                entity.gamePlay.UIStateUpdate = false;
                entity.playerClientUpdate.OpenUIFrame = true;
        }

        public ClientPlayerUpdateWeaponSystem(Contexts contexts) : base(contexts)
        {
        }
    }
}