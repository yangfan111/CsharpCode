using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using App.Shared.Util;
using Core;
using Core.GameTime;
using App.Shared.SceneTriggerObject;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Server.GameModules.SceneObject
{
    public class ServerGlassyObjectListener : ServerMapTriggerObjectListener
    {
        private ICurrentTime _currentTime;
        public ServerGlassyObjectListener(Contexts contexts): base(contexts, ETriggerObjectType.GlassyObject,
            MapObjectMatcher.AllOf(
                MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.GlassyData))
        {
            _currentTime = contexts.session.currentTimeObject;
        }

        public override IEntity CreateMapObj(int id)
        {
            var gameObj = _objectManager.Get(id);
            if (gameObj == null) return null;
            var glass = (MapObjectEntity)MapObjectEntityFactory.CreateGlassyObject(id, gameObj);
            MapObjectUtility.RecordMapObj(id, (int)_triggerType, glass);
            MapObjectUtility.FetchFractruedState(gameObj);
            return glass;
        }

        public override void OnTriggerObjectLoaded(int id, GameObject gameObject)
        {
            MapObjectUtility.RecordGameObjId(gameObject, (int)_triggerType, id);
            MapObjectUtility.AttachRawObjToFracture(gameObject);
            MapObjectUtility.AddCallBack<FracturedGlassyObject>(gameObject, OnBroken);
        }

        private void OnBroken(object o)
        {
            var chunk = o as FracturedGlassyChunk;
            var mapObject = MapObjectUtility.GetMapObjectOfFracturedChunk(chunk);
            if (mapObject != null)
            {
                mapObject.glassyData.SetBroken(chunk.ChunkId);
                mapObject.flagImmutability.LastModifyServerTime = _currentTime.CurrentTime;
            }
        }
    }
}
