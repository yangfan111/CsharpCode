using App.Client.GameModules.Player;
using App.Shared.EntityFactory;
using App.Shared.Util;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Weapon
{
    
    public class PlayerWeaponGamePlayUpdateSystem : AbstractGamePlaySystem<PlayerEntity>
    {
        public PlayerWeaponGamePlayUpdateSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.PlayerClientUpdate, PlayerMatcher.ThrowingAction));
        }

       

        protected override void OnGamePlay(PlayerEntity entity)
        {
            var throwAction = entity.throwingAction.ActionData;
            if (throwAction.ThrowingPrepare &&  entity.playerClientUpdate.DestoryPreparedThrowingEntity)
            {
                var throwing                                 = ThrowingEntityFactory.GetEntityWithEntityKey(throwAction.ThrowingEntityKey);
                if (null != throwing) throwing.isFlagDestroy = true;
                entity.stateInterface.State.ForceFinishGrenadeThrow();
                throwAction.InternalCleanUp();
            }
        }
    }
}