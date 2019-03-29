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
            (owner.OwnerEntity as PlayerEntity).WeaponController().LateUpdate(owner.OwnerEntity as PlayerEntity);
        }
    }
}