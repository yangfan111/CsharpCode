using Core.Event;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Client.GameModules.SceneObject
{
    public class InitUploadEventSystem: IUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(InitUploadEventSystem));
        public InitUploadEventSystem(Contexts contexts)
        {
            _contexts = contexts;
        }
        
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            
            player.uploadEvents.Events.ReInit();
            
            foreach (var serials in player.uploadEvents.StoreEvents.Events)
            {
                foreach (var evt in serials.Value)
                {
                    player.uploadEvents.Events.AddEvent(evt);
                }
            }
            player.uploadEvents.StoreEvents.ReInit();
        }
    }
}