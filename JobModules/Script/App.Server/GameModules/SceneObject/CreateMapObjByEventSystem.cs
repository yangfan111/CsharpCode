using App.Server.GameModules.SceneObject;
using App.Shared.Player.Events;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.Event;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.SceneObject
{
    public class CreateMapObjByEventSystem:IUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private ITriggerObjectListener _destructibleListener;
        private ITriggerObjectListener _glassListener;
        private ITriggerObjectListener _doorListener;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(CreateMapObjByEventSystem));
        
        public CreateMapObjByEventSystem(Contexts contexts)
        {
            _contexts = contexts;
            _destructibleListener = new ServerDestructibleObjectListener(contexts);
            _glassListener = new ServerGlassyObjectListener(contexts);
            _doorListener = new ServerDoorListener(contexts);
        }
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (player == null) return;
            var evts = player.uploadEvents.Events.Events[EEventType.CreateMapObj];
            if(evts!=null)
                foreach (var evt in evts)
                {
                    HandleEvt(evt);
                }

            var localEvts = player.uploadEvents.StoreEvents.Events[EEventType.CreateMapObj];
            if(localEvts!=null)
                foreach (var evt in localEvts)
                {
                    HandleEvt(evt);
                }
        }

        private void HandleEvt(IEvent evt)
        {
            var createEvt = evt as CreateMapObjEvent;
            if (createEvt != null)
            {
                var mapObj = MapObjectUtility.GetMapObjByRawId(createEvt.Id, createEvt.Type);
                if (mapObj != null) return;
                _logger.InfoFormat("CreateMapObj: {0}, {1}", createEvt.Id, createEvt.Type);
                switch (createEvt.Type)
                {
                    case (int) ETriggerObjectType.Door:
                        _doorListener.CreateMapObj(createEvt.Id);
                        break;
                    case (int) ETriggerObjectType.DestructibleObject:
                        _destructibleListener.CreateMapObj(createEvt.Id);
                        break;
                    case (int) ETriggerObjectType.GlassyObject:
                        _glassListener.CreateMapObj(createEvt.Id);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}