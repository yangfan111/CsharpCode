using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.SceneTriggerObject;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.SceneObject
{
    public class ClientDestructibleObjectUpdateSystem : IGamePlaySystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientDestructibleObjectUpdateSystem));

        private ClientDestructibleObjectListener _destructObjListener;
        private ClientGlassyObjectListener _glassyObjListener;
        private MapObjectContext _context;
        private IGroup<MapObjectEntity> _destructibleObjects;
        private IGroup<MapObjectEntity> _glassyObjects;
        private IGroup<MapObjectEntity> _eventObjects;
        private ClientSessionObjectsComponent _sessionObjects;
        public ClientDestructibleObjectUpdateSystem(Contexts contexts)
        {
            _destructObjListener = new ClientDestructibleObjectListener(contexts);
            _glassyObjListener = new ClientGlassyObjectListener(contexts);
            _context = contexts.mapObject;
            _destructibleObjects = _context.GetGroup(MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.DestructibleData,
                MapObjectMatcher.RawGameObject));
            _glassyObjects = _context.GetGroup(MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.GlassyData,
                MapObjectMatcher.RawGameObject));
            _eventObjects = _context.GetGroup(MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.TriggerObjectEvent,
                MapObjectMatcher.TriggerObjectEventFlag,
                MapObjectMatcher.RawGameObject));
            _sessionObjects = contexts.session.clientSessionObjects;
           
        }

        public void OnGamePlay()
        {
            _destructObjListener.Update();
            _glassyObjListener.Update();
            if (!SharedConfig.IsOffline)
            {
                SendSyncEvents();
                SyncDestructibleObjects();
                SyncGlassyObjects();
            }
        }

        private void SendSyncEvents()
        {
            var channel = _sessionObjects.NetworkChannel;
            if (channel != null)
            {
                var objs = _eventObjects.GetEntities();
                var objCount = objs.Length;
                for (int i = 0; i < objCount; ++i)
                {
                    var obj = objs[i];
                    var syncEvents = obj.triggerObjectEvent.SyncEvents;
                    while (syncEvents.Count > 0)
                    {
                        var evt = syncEvents.Dequeue();
                        channel.SendReliable((int)EClient2ServerMessage.TriggerObjectEvent, evt);
                        evt.ReleaseReference();
                    }

                    obj.isTriggerObjectEventFlag = false;
                }
            }
        }

        private void SyncDestructibleObjects()
        {
            var objs = _destructibleObjects.GetEntities();
            for (int i = 0; i < objs.Length; ++i)
            {
                var obj = objs[i];
                var data = obj.destructibleData;
                if (data.SyncDelay > 0)
                {
                    data.SyncDelay -= Time.deltaTime;
                    continue;
                }

                var sync = data.SyncDestructionState();
                if (sync.Reset)
                {
                    var fracturedObject = obj.rawGameObject.Value.GetComponent<FracturedObject>();
                    if (fracturedObject != null)
                    {
                        fracturedObject.ResetChunks();
                    }
                }

                var stateDiff = sync.StateDiff;
                if (stateDiff != 0)
                {
                    var fracturedObject = obj.rawGameObject.Value.GetComponent<FracturedObject>();
                    if (fracturedObject == null)
                    {
                        _logger.ErrorFormat("Can not find 'FracturedObject' in entity {0} gameobject {1}", obj.entityKey.Value,
                            obj.rawGameObject.Value.name);
                        return;
                    }
                    if (data.StartAsWhole)
                    {
                        if (data.HasAnyChunkDetached())
                        {
                            fracturedObject.CollapseChunks();
                        }
                    }
                    else
                    {
                        int chunkId = 0;
                        var localState = data.DestructionState;
                        while (stateDiff != 0)
                        {
                            if ((stateDiff & 0x1) != 0 && (localState & 0x1) == 0)
                            {
                                fracturedObject.CollapseChunk(chunkId);
                            }

                            stateDiff = stateDiff >> 1;
                            localState = localState >> 1;

                            chunkId++;
                        }
                    }
                }
            }
        }

        private void SyncGlassyObjects()
        {
            var objs = _glassyObjects.GetEntities();
            for (int i = 0; i < objs.Length; ++i)
            {
                var obj = objs[i];
                var data = obj.glassyData;
                if (data.HasChangedState())
                {
                    var fo = obj.rawGameObject.Value.GetComponent<FracturedGlassyObject>();

                    if (data.HasAnyChunkBroken())
                    {
                        var chunkId = data.GetNextBrokenChunkId();
                        while (chunkId >= 0)
                        {
                            fo.MakeBroken(chunkId);
                            chunkId = data.GetNextBrokenChunkId(chunkId);
                            data.HasLocalBrokenChunks = true;
                        }
                    }
                    else
                    {
                        fo.ResetChunks();
                        data.HasLocalBrokenChunks = false;
                    }

                    data.ClearChangedState();
                }
            }
        }
    }
}
