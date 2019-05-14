using App.Shared;
using App.Shared.Player.Events;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.Event;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Client.GameModules.SceneObject
{
    public class CreateMapObjSystem:IGamePlaySystem
    {
        private Contexts _contexts;
        private ITriggerObjectListener _destructibleListener;
        private ITriggerObjectListener _glassListener;
        private ITriggerObjectListener _doorListener;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(CreateMapObjSystem));
        
        public CreateMapObjSystem(Contexts contexts)
        {
            _contexts = contexts;
            _destructibleListener = new ClientDestructibleObjectListener(contexts);
            _glassListener = new ClientGlassyObjectListener(contexts);
            _doorListener = new ClientDoorListener(contexts);
        }
        public void OnGamePlay()
        {
            if (!SharedConfig.IsOffline) return;
            
            var player = _contexts.player.flagSelfEntity;
            if (player == null) return;
            var evts = player.uploadEvents.Events.Events[EEventType.CreateMapObj];
            foreach (var evt in evts)
            {
                var createEvt = evt as CreateMapObjEvent;
                if (createEvt != null)
                {
                    var mapObj = MapObjectUtility.GetMapObjByRawId(createEvt.Id, createEvt.Type);
                    if (mapObj != null) continue;
                    _logger.ErrorFormat("CreateMapObj: {0}, {1}", createEvt.Id,createEvt.Type);
                    switch (createEvt.Type)
                    {
                        case (int)ETriggerObjectType.Door:
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
}