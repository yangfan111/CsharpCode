using System.Runtime.Remoting.Contexts;
using App.Shared.Components.SceneObject;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules
{
    public class ServerSpecialZoneEventSystem: IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerSpecialZoneEventSystem));

        public ServerSpecialZoneEventSystem(Contexts contexts)
        {
        }
        
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (null == player)
            {
                Logger.Error("Owner is not player");
                return;
            }
   
            if (player.triggerEvent.NeedUnmountWeapon)
            {
                player.WeaponController().ForceUnArmHeldWeapon();
            }
        }

    }
}