using App.Shared.Components.Player;
using Core;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    class PlayerAvatarSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerAvatarSystem));
        public PlayerAvatarSystem()
        {
        }
        
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (null == player)
            {
                Logger.Error("owner entity is not player entity ");
                return;
            }
            player.appearanceInterface.Appearance.UpdateAvatar();
        }
    }
}
