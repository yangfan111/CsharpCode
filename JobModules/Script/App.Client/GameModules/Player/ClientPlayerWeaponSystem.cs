using App.Shared;
using App.Shared.GameModules.Player;


namespace App.Client.GameModules.Player
{
    public class ClientPlayerWeaponSystem : AbstractSelfPlayerRenderSystem
    {
        private Contexts _contexts;

        public override void OnRender(PlayerEntity playerEntity)
        {
            if (!playerEntity.hasPlayerClientUpdate || !playerEntity.gamePlay.UIStateUpdate)
                return;
            playerEntity.gamePlay.UIStateUpdate = false;

            var hasUIState = PlayerStateUtil.HasUIState(playerEntity.gamePlay);
            playerEntity.playerClientUpdate.OpenUIFrame = hasUIState;
            if (hasUIState)
            {
                //playerEntity.ModeController().CallBeforeAction(playerEntity.WeaponController(), EPlayerState..UIOpen);
                var throwing =
                    _contexts.throwing.GetEntityWithEntityKey(playerEntity
                                                              .WeaponController().RelatedThrowAction
                                                              .ThrowingEntityKey);
                if (null != throwing) throwing.isFlagDestroy = true;
            }
        }

        public ClientPlayerWeaponSystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }
    }
}