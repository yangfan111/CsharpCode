using Core.Configuration;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Singleton;

namespace App.Shared.GameModules.Camera
{
    public class ClientPrepareObserveSystem:IUserCmdExecuteSystem
    {
        private Contexts _contexts;
        
        public ClientPrepareObserveSystem(Contexts contexts)
        {
            _contexts = contexts;
        }
        
        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var player = getter.OwnerEntity as PlayerEntity;
            if (player == null) return;
            
            player.observeCamera.ObservedPlayer = null;
            
            if (!player.gamePlay.IsObserving()) return;
            var observedPlayer =
                _contexts.player.GetEntityWithEntityKey(new EntityKey(player.gamePlay.CameraEntityId,
                    (short) EEntityType.Player));
            if (observedPlayer == null) return;
            
            if (!observedPlayer.hasCameraArchor)
                observedPlayer.AddCameraArchor();
            if (!observedPlayer.hasTime)
                observedPlayer.AddTime(0);
            if (!observedPlayer.hasCameraStateOutputNew)
                observedPlayer.AddCameraStateOutputNew();
            if(!observedPlayer.hasCameraFinalOutputNew)
                observedPlayer.AddCameraFinalOutputNew();
            if(!observedPlayer.hasCameraStateNew)
                observedPlayer.AddCameraStateNew();
            if (!observedPlayer.hasCameraConfigNow)
                observedPlayer.AddCameraConfigNow();
            
            player.observeCamera.ObservedPlayer = observedPlayer;
            
            if (observedPlayer.cameraStateNew.MainNowMode != player.observeCamera.MainNowMode)
            {
                observedPlayer.cameraStateNew.MainLastMode = observedPlayer.cameraStateNew.MainNowMode;
                observedPlayer.cameraStateNew.MainNowMode = player.observeCamera.MainNowMode;
                observedPlayer.cameraStateNew.MainModeTime = player.time.ClientTime;
            }
            if (observedPlayer.cameraStateNew.PeekNowMode != player.observeCamera.PeekNowMode)
            {
                observedPlayer.cameraStateNew.PeekLastMode = observedPlayer.cameraStateNew.PeekLastMode;
                observedPlayer.cameraStateNew.PeekNowMode = player.observeCamera.PeekNowMode;
                observedPlayer.cameraStateNew.PeekModeTime = player.time.ClientTime;
            }
            if (observedPlayer.cameraStateNew.FreeNowMode != player.observeCamera.FreeNowMode)
            {
                observedPlayer.cameraStateNew.FreeLastMode = observedPlayer.cameraStateNew.FreeLastMode;
                observedPlayer.cameraStateNew.FreeNowMode = player.observeCamera.FreeNowMode;
                observedPlayer.cameraStateNew.FreeModeTime = player.time.ClientTime;
            }
            observedPlayer.cameraStateNew.FreeYaw = player.observeCamera.FreeYaw;
            observedPlayer.cameraStateNew.FreePitch = player.observeCamera.FreePitch;
        }
    }
}