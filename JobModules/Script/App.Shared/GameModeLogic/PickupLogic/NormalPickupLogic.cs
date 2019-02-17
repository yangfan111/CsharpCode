using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Player;
using App.Shared.Util;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModeLogic;
using Core.Utils;
using UnityEngine;
using Assets.App.Shared.EntityFactory;

namespace App.Shared.GameModeLogic.PickupLogic
{
    public class NormalPickupLogic : IPickupLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NormalPickupLogic));
        protected SceneObjectContext _sceneObjectContext;
        protected PlayerContext _playerContext;
        protected Contexts _contexts;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;
        private int _sceneWeaponLifeTime;
        private RuntimeGameConfig _runtimeGameConfig;
        private AutoPickupLogic _autoPickupLogic;
        public NormalPickupLogic(
            Contexts contexts,
            ISceneObjectEntityFactory sceneObjectEntityFactory,
            RuntimeGameConfig runtimeGameConfig,
            int sceneWeaponLifetime)
        {
            _contexts = contexts;
            _sceneObjectContext = contexts.sceneObject;
            _playerContext = contexts.player;
            _sceneObjectEntityFactory = sceneObjectEntityFactory;
            _sceneWeaponLifeTime = sceneWeaponLifetime;
            _runtimeGameConfig = runtimeGameConfig;
            _autoPickupLogic = new AutoPickupLogic(contexts, sceneObjectEntityFactory);
        }

        public virtual void DoPickup(int playerEntityId, int weaponEntityId)
        {
            var entity = _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(weaponEntityId, (short)EEntityType.SceneObject));
            if (null == entity)
            {
                Logger.ErrorFormat("{0} doesn't exist in scene object context ", weaponEntityId);
                return;
            }
            if (entity.hasThrowing)
            {
                return;
            }
            var player = _playerContext.GetEntityWithEntityKey(new EntityKey(playerEntityId, (short)EEntityType.Player));
            if (null == player)
            {
                Logger.ErrorFormat("{0} doesn't exist in player context ", playerEntityId);
                return;
            }
            if (!entity.hasWeaponObject)
            {
                Logger.ErrorFormat("only weapon is supported in normal mode");
                return;
            }
            if (!entity.IsCanPickUpByPlayer(player))
            {
                return;
            }
            var controller = player.WeaponController();
            //销毁场景武实体
            _sceneObjectEntityFactory.DestroySceneWeaponObjectEntity(entity.entityKey.Value.EntityId);
            //创建武器物体
            var weaponScan = WeaponUtil.CreateScan(entity.weaponObject);
            EntityKey lastKey = controller.PickUpWeapon(weaponScan);
            //创建场景掉落实体
            if (WeaponUtil.IsWeaponKeyVaild(lastKey))
            {

                WeaponEntity lastEntity = WeaponEntityFactory.GetWeaponEntity(_contexts.weapon, lastKey);
                lastEntity.SetFlagNoOwner();
                weaponScan = WeaponUtil.CreateScan(lastEntity);
                _sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScan, player.position.Value, _sceneWeaponLifeTime);
            }
        }

        public virtual void SendPickup(int entityId, int itemId, int category, int count)
        {
            //DO NOTHING
        }

        protected virtual void DoDropGrenade(PlayerEntity playerEntity)
        {
            //DO NOTHING
        }

        public virtual void Dorp(int playerEntityId, EWeaponSlotType slot)
        {
            var player = _playerContext.GetEntityWithEntityKey(new EntityKey(playerEntityId, (short)EEntityType.Player));
            if (null == player)
            {
                Logger.ErrorFormat("{0} doesn't exist in player context ", playerEntityId);
                return;
            }
            switch (slot)
            {
                case EWeaponSlotType.ThrowingWeapon:
                    DoDropGrenade(player);
                    return;
            }
            var weaponController = player.WeaponController();
            var heldAgent = player.WeaponController().HeldWeaponAgent;
            if (heldAgent.IsVailed())
            {
                var dropPos = player.GetHandWeaponPosition();
                var playerTrans = player.characterContoller.Value.transform;
                var forward = playerTrans.forward;
                var pos = dropPos + forward * _runtimeGameConfig.WeaponDropOffset;
                var weaponScacn = heldAgent.BaseComponentScan.Value;
                RaycastHit hhit;
                SceneObjectEntity sceneObjectEntity;
                if (Physics.Raycast(dropPos, forward, out hhit, _runtimeGameConfig.WeaponDropOffset, UnityLayers.SceneCollidableLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hhit.point, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, playerTrans.position, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(pos, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, playerTrans.position, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                weaponController.DropSlotWeapon(slot);
            }
        }

        public virtual void SendAutoPickupWeapon(int entityId)
        {
            //DO NOTHING
        }

        public virtual void AutoPickupWeapon(int playerEntityId, int weaponEntityId)
        {
            _autoPickupLogic.AutoPickupWeapon(playerEntityId, weaponEntityId);
        }
    }
}
