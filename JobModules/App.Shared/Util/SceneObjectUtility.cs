using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Common;
using App.Shared.Player;
using Core.EntityComponent;
using Core.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Shared.Util
{
    public static class SceneObjectUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SceneObjectUtility));

        public static void AddRawGameObject<T>(IEntity obj, GameObject go,
            Action<object> detachCallback) where T : FracturedBaseObject
        {
            AddRawGameObject(obj, go);

            var fracturedObject = go.GetComponent<T>();
            if (fracturedObject != null)
            {
                fracturedObject.EventDetachCallback = detachCallback;
            }
        }

        private static void AddRawGameObject(IEntity obj, GameObject gameObject)
        {
            SceneObjectEntity sceneObject = obj as SceneObjectEntity;
            sceneObject.AddRawGameObject(gameObject);
            var entityReference = gameObject.GetComponent<EntityReference>();
            if (entityReference == null)
            {
                entityReference = gameObject.AddComponent<EntityReference>();
            }
            entityReference.Init(sceneObject.entityAdapter);
        }

        public static bool IsCanPickUpByPlayer(this SceneObjectEntity sceneObjectEntity, PlayerEntity playerEntity)
        {
            if (!sceneObjectEntity.hasCastFlag)
            {
                return true;
            }
            return PlayerStateUtil.HasCastState((EPlayerCastState)sceneObjectEntity.castFlag.Flag, playerEntity.gamePlay); 
        }
        
    }
}
