using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;


namespace App.Shared.GameModules.Weapon
{
    public class PlayerWeaponUpdateSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponUpdateSystem));
        private                 Contexts      _contexts;

        public PlayerWeaponUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity     = owner.OwnerEntity as PlayerEntity;
            var          weaponController = playerEntity.WeaponController();
            weaponController.InternalUpdate(playerEntity);
            if (playerEntity.playerClientEventsUpdate.OpenUIFrame)
            {
                playerEntity.playerClientEventsUpdate.OpenUIFrame = false;
                if (weaponController.RelatedThrowAction.IsReady && !weaponController.RelatedThrowAction.IsThrow)
                {
                    if (weaponController.RelatedThrowAction.IsReady && !weaponController.RelatedThrowAction.IsThrow)
                    {
                        var throwing =
                            _contexts.throwing.GetEntityWithEntityKey(weaponController
                                                                      .RelatedThrowAction.ThrowingEntityKey);
                        if (null != throwing) throwing.isFlagDestroy = true;

                        weaponController.RelatedCharState.ForceFinishGrenadeThrow();
                        weaponController.RelatedThrowAction.ClearState();
                    }
                }
            }
        }
    }
}