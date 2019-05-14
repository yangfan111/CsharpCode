using App.Shared.Util;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;


namespace App.Shared.GameModules.Weapon
{
    public class PlayerWeaponUpdateSystem : AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponUpdateSystem));
        private                 Contexts      _contexts;

        public PlayerWeaponUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return true;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var weaponController = playerEntity.WeaponController();
            weaponController.InternalUpdate(playerEntity);
            if (!playerEntity.playerClientUpdate.OpenUIFrame)
                return;
            playerEntity.playerClientUpdate.OpenUIFrame = false;
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