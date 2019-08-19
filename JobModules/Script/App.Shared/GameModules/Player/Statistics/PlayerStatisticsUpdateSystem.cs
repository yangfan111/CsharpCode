using Core;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using XmlConfig;


namespace App.Shared.GameModules.Weapon
{
    public class PlayerStatisticsUpdateSystem: IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponGamePlayUpdateSystem));

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            owner.OwnerEntityKey.StatisticsController().Update(cmd);
        }
    }
}