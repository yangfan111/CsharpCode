using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;


namespace App.Shared.GameModules.Weapon
{
    public class PlayerWeapnStateUpdateSystem: IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeapnStateUpdateSystem));
        private Contexts _contexts;

        public PlayerWeapnStateUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = owner.OwnerEntity as PlayerEntity;
            var weaponController = playerEntity.WeaponController();
            weaponController.InternalUpdate(playerEntity);
            if (playerEntity.playerWeaponClientUpdate.UIOpenFrame)
            {
                playerEntity.playerWeaponClientUpdate.UIOpenFrame= false;
                if (weaponController.RelatedThrowActionInfo.IsReady && !weaponController.RelatedThrowActionInfo.IsThrow)
                {
                    var throwing = _contexts.throwing.GetEntityWithEntityKey(weaponController.RelatedThrowActionInfo.ThrowingEntityKey);
                    if (null != throwing) throwing.isFlagDestroy = true;

                    weaponController.RelatedStateInterface.ForceFinishGrenadeThrow();
                    weaponController.RelatedThrowActionInfo.ClearState();
                }
            }
        }
    }
}