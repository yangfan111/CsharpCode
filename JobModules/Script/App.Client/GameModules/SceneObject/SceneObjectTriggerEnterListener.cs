using App.Shared.GameModules.Common;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class SceneObjectTriggerEnterListener : MonoBehaviour
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectTriggerEnterListener));
        private int _entityId;
        public void SetEntityId(int entityId)
        {
            _entityId = entityId;
        }

        void OnTriggerEnter(Collider other)
        {
            var reference = other.GetComponent<EntityReference>();
            if(null == reference)
            {
                return;
            }
            var entity = reference.Reference;
            var playerEntity = entity as PlayerEntity;
            if(null == playerEntity)
            {
                return;
            }
            if(!playerEntity.isFlagSelf)
            {
                return;
            }
            if(!playerEntity.hasModeLogic)
            {
                Logger.ErrorFormat("player has no modelogic component");
                return;
            }
            playerEntity.modeLogic.ModeLogic.SendAutoPickupWeapon(_entityId);
        }
    }
}
