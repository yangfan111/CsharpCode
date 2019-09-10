using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
namespace App.Shared.GameModules.Player
{
    public class PlayerClientTimeSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAttackSystem));

       
        public PlayerClientTimeSystem()
        {
            
        }

      
        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)getter.OwnerEntity;
            playerEntity.time.ClientTime += cmd.FrameInterval;
          
        }
    }
}