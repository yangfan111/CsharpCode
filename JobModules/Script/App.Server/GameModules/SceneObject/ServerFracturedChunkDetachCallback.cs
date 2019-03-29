using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.GameModules.SceneObject;
using App.Shared.Util;
using Core;
using Core.GameTime;
using UltimateFracturing;
using UnityEngine;

namespace App.Server.GameModules.SceneObject
{
    public class ServerFracturedChunkDetachCallback 
    {
        private SceneObjectContext _sceneObjectContext;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;
        private ICurrentTime _currentTime;
        public ServerFracturedChunkDetachCallback(Contexts contexts)
        {
            _sceneObjectContext = contexts.sceneObject;
            _sceneObjectEntityFactory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;
            _currentTime = contexts.session.currentTimeObject;
        }

        public void OnDetach(object o)
        {
            FracturedChunk chunk = o as FracturedChunk;
            var destructibleObject = MapObjectUtility.GetMapObjectOfFracturedChunk(chunk);
            if (destructibleObject != null)
            {
                var data = destructibleObject.destructibleData;
                data.SetDestruction(chunk.ChunkId);
                data.LastSyncDestructionState = data.DestructionState;
                destructibleObject.flagImmutability.LastModifyServerTime = _currentTime.CurrentTime;
            }
        }
    }
}
