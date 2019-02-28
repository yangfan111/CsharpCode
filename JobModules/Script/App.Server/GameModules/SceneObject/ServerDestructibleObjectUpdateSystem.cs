using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.GameTime;
using Entitas;
using UltimateFracturing;
using Core.SceneTriggerObject;

namespace App.Server.GameModules.SceneObject
{
    public class ServerDestructibleObjectUpdateSystem : IGamePlaySystem
    {
        private ServerDestructibleObjectListener _destructObjListener;
        private ServerGlassyObjectListener _glassyObjListener;
        private MapObjectContext _context;
        private IGroup<MapObjectEntity> _eventObjects;
        private ICurrentTime _currentTime;
        public ServerDestructibleObjectUpdateSystem(Contexts contexts)
        {
            _destructObjListener = new ServerDestructibleObjectListener(contexts);
            _glassyObjListener = new ServerGlassyObjectListener(contexts);
            _context = contexts.mapObject;
            _eventObjects = _context.GetGroup(MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.TriggerObjectEvent,
                MapObjectMatcher.TriggerObjectEventFlag,
                MapObjectMatcher.RawGameObject));

            _currentTime = contexts.session.currentTimeObject;
        }

        void IGamePlaySystem.OnGamePlay()
        {
            var objs = _eventObjects.GetEntities();
            var objCount = _eventObjects.Count();

            for (int i = 0; i < objCount; ++i)
            {
                var obj = objs[i];

                var syncEvents = obj.triggerObjectEvent.SyncEvents;
                
                while (syncEvents.Count > 0)
                {
                    var evt = syncEvents.Dequeue();

                    switch (evt.EType)
                    {
                        case TriggerObjectSyncEventType.DetachChunk:
                            ProcessDetachChunkEvent(obj, (ChunkSyncEvent) evt); 
                            break;
                        case TriggerObjectSyncEventType.BreakChunk:
                            ProcessBreakChunkEvent(obj, (ChunkSyncEvent) evt);
                            break;                    
                    }

                    evt.ReleaseReference();
                }

                obj.isTriggerObjectEventFlag = false;
            }
        }

        private void ProcessBreakChunkEvent(MapObjectEntity mapObject, ChunkSyncEvent evt)
        {
            var go = mapObject.rawGameObject.Value;
            var fracturedGlassyObject = go.GetComponent<FracturedGlassyObject>();
            if (fracturedGlassyObject != null)
            {
                var data = mapObject.glassyData;
                var chunkId = evt.ChunkId;
                if (!data.IsBroken(chunkId))
                {
                    fracturedGlassyObject.PutToBrokenState(chunkId);
                    data.SetBroken(chunkId);
                    mapObject.flagImmutability.LastModifyServerTime = _currentTime.CurrentTime;
                }
            }
        }

        private void ProcessDetachChunkEvent(MapObjectEntity sceneObject, ChunkSyncEvent evt)
        {
            var go = sceneObject.rawGameObject.Value;
            var fracturedObject = go.GetComponent<FracturedObject>();
            if (fracturedObject != null)
            {
                var data = sceneObject.destructibleData;
                if (!data.IsInDestructionState(evt.ChunkId))
                {
                    fracturedObject.CollapseChunk(evt.ChunkId);
                    sceneObject.flagImmutability.LastModifyServerTime = _currentTime.CurrentTime;
                }
            }
        }
    }
}
