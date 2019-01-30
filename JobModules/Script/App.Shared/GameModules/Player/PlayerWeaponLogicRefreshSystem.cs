using App.Client.GameModules.Player;
using App.Shared.EntityFactory;
using Core;
using Core.GameModule.Interface;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponLogicRefreshSystem : AbstractGamePlaySystem<PlayerEntity>
    {
        public PlayerWeaponLogicRefreshSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.WeaponLogicInfo,PlayerMatcher.WeaponFactory));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.weaponLogicInfo.LastWeaponId != entity.weaponLogicInfo.WeaponId;
        }

        protected override void OnGamePlay(PlayerEntity player)
        {
            player.RefreshPlayerWeaponLogic(player.weaponLogicInfo.WeaponId);
        }
    }
}