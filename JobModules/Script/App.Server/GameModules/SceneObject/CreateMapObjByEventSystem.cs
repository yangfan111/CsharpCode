using System.Collections.Generic;
using App.Server.GameModules.SceneObject;
using App.Shared.Player.Events;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.Event;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.SceneObject
{
    public class CreateMapObjByEventSystem : IUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private ITriggerObjectListener _destructibleListener;
        private ITriggerObjectListener _glassListener;
        private ITriggerObjectListener _doorListener;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(CreateMapObjByEventSystem));
        private readonly List<IEvent> evts = new List<IEvent>();

        public CreateMapObjByEventSystem(Contexts contexts)
        {
            _contexts = contexts;
            _destructibleListener = new ServerDestructibleObjectListener(contexts);
            _glassListener = new ServerGlassyObjectListener(contexts);
            _doorListener = new ServerDoorListener(contexts);
        }

        public void ExecuteUserCmd(IPlayerUserCmdGetter owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (player == null) return;
            evts.Clear();
            player.uploadEvents.Events.GetEvents(EEventType.CreateMapObj, evts);

            foreach (var evt in evts)
            {
                HandleEvt(evt);
            }

            foreach (var evt in MapObjectUtility.StoredCreateMapObjMsg)
            {
                HandleEvt(evt);
            }
            MapObjectUtility.StoredCreateMapObjMsg.Clear();
        }

        private void HandleEvt(IEvent evt)
        {
            var createEvt = evt as CreateMapObjEvent;
            if (createEvt != null)
            {
//                var mapObj = MapObjectUtility.GetMapObjByRawId(createEvt.Id, createEvt.Type);
                var mapObj = _contexts.mapObject.GetEntityWithEntityKey(new EntityKey(createEvt.Id, (int) EEntityType.MapObject));
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