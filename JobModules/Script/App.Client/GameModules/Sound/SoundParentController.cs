using App.Shared;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Sound
{
    public class SoundParentController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SoundParentController));
        private Contexts _contexts;

        public SoundParentController(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void CleanUpWithParent(SoundEntity soundEntity)
        {
            if(!soundEntity.hasParent)
            {
                Logger.Error("sound entity has no parent");
                return;
            }
            var parentKey = soundEntity.parent.Value;
            switch((EEntityType)parentKey.EntityType)
            {
                case EEntityType.Player:
                    var player = _contexts.GetEntityWithEntityKey(parentKey);
                    if(null == player)
                    {
                        soundEntity.isFlagDestroy = true;
                    }
                    break;
                case EEntityType.Bullet:
                    var bullet = _contexts.GetEntityWithEntityKey(parentKey);
                    if(null == bullet)
                    {
                        soundEntity.isFlagDestroy = true;
                    }
                    break;
            }
        }

        public void AttachParent(SoundEntity soundEntity)
        {
            if(!soundEntity.hasParent)
            {
                Logger.Error("sound entity has no parent");
                return;
            }
            if(!soundEntity.hasUnityObj)
            {
                Logger.Error("sound entity has no unity obj ");
                return;
            }
            var go = soundEntity.unityObj.UnityObject.AsGameObject;
            if(null == go)
            {
                Logger.Error("unity obj of sound entity is null ");
                return;
            }
            var parentKey = soundEntity.parent.Value;
            switch ((EEntityType)parentKey.EntityType)
            {
                case EEntityType.Player:
                    var player = _contexts.player.GetEntityWithEntityKey(parentKey);
                    if(player.hasCharacterContoller)
                    {
                        if(null != player && null != player.characterContoller.Value.gameObject)
                        {
                            go.transform.parent = player.characterContoller.Value.gameObject.transform;
                        }
                        else
                        {
                            Logger.Error("player's gameobject is null");
                        }
                    }
                    break;
                case EEntityType.Bullet:
                    var bullet = _contexts.bullet.GetEntityWithEntityKey(parentKey);
                    if(null != bullet && bullet.hasBulletGameObject)
                    {
                        if(null != bullet.bulletGameObject.UnityObject)
                        {
                            go.transform.parent = bullet.bulletGameObject.UnityObject.AsGameObject.transform;
                        }
                        else
                        {
                            Logger.Error("bullet's gameobject is null");
                        }
                    }
                    break;
            }
            go.transform.localPosition = Vector3.zero;
        }
    }
}
