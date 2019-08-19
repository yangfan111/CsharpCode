using App.Shared.Player;
using Core.GameModule.Interface;
using Core.Utils;
using Utils.AssetManager;

namespace App.Client.GameModules.ClientInit
{
    public class RewindFirstSnapshotInitSystem: IModuleInitSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RewindFirstSnapshotInitSystem));
        private readonly Contexts _contexts;

        public RewindFirstSnapshotInitSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            var player =  _contexts.player.flagSelfEntity;
            _contexts.session.clientSessionObjects.UserPredictionManager.RewindFirstSnapshot(player.entityKey.Value);
           
            _logger.InfoFormat("RewindFirstSnapshotInitSystem:{0}", player.position.Value);
            player.RootGo().transform.SetPositionAndRotation(player.position.Value, player.orientation.ModelView);
        }
    }
}