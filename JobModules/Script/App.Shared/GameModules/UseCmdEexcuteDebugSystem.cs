using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules
{
    public class UseCmdEexcuteDebugSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UseCmdEexcuteDebugSystem));
        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            PlayerEntity player = getter.OwnerEntity as PlayerEntity;
            if (player.hasNetworkAnimator)
            {
                Logger.ErrorFormat("seq:{0} NetworkAnimator:{1}",cmd.Seq, player.networkAnimator);
            }
        }
    }
}