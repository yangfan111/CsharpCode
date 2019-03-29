using App.Shared.GameModules.Weapon;
using Core;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerGrenadeInventorySyncSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerGrenadeInventorySyncSystem));
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if(null == player)
            {
                Logger.Error("player entity is null");
                return;
            }
            IGrenadeCacheHelper helper = player.WeaponController().GrenadeHelper;
            helper.Rewind();
        }
    }
}
