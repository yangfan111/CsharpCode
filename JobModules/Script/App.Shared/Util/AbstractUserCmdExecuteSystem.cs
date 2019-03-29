using Assets.Utils.Configuration;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Singleton;

namespace App.Shared.Util
{
    public abstract class AbstractUserCmdExecuteSystem : IUserCmdExecuteSystem
    {
     

        protected abstract bool filter(PlayerEntity playerEntity);

        protected abstract void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd);
        
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (player != null && filter(player))
            {
                ExecuteUserCmd(player, cmd);
            }
        }
        protected WeaponConfigManagement UserWeaponConfigManagement { get { return SingletonManager.Get<WeaponConfigManagement>(); } }
    }
}