using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.Util;
using Core.Components;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using log4net.Repository.Hierarchy;

namespace App.Shared.GameModules.Attack
{
    public class CameraFireInfoSyncSystem:AbstractUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private LoggerAdapter Logger = new LoggerAdapter(typeof(CameraFireInfoSyncSystem));
        public CameraFireInfoSyncSystem(Contexts contexts)
        {
            _contexts = contexts;
        }
        protected override bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasCameraFireInfo;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            SyncCameraFireInfo(playerEntity);
        //   DebugUtil.AppendShootText(cmd.Seq,"seq:{0} cmd:{1} player.position:{1}",cmd.Seq,cmd.MoveHorizontal,playerEntity.position.Value); 
        }
        
        private void SyncCameraFireInfo(PlayerEntity player)
        {
            if (SharedConfig.IsServer)
            {
                player.cameraFinalOutputNew.PlayerFocusPosition = player.GetOrigCameraPos(player.cameraFireInfo.PlayerFocusPosition);
            }
            else
            {
                player.cameraFireInfo.PlayerFocusPosition = player.GetShiftCameraPos(player.cameraFinalOutputNew.PlayerFocusPosition);
            }
        }
    }
}