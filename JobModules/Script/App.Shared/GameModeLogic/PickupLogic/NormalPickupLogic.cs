using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModeLogic;
using Core.Utils;
using System;
using UnityEngine;

namespace App.Shared.GameModeLogic.PickupLogic
{
    public class NormalPickupLogic : IPickupLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NormalPickupLogic));
        protected SceneObjectContext _sceneObjectContext;
        protected PlayerContext _playerContext;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;
        private int _sceneWeaponLifeTime;
        private RuntimeGameConfig _runtimeGameConfig;
        private AutoPickupLogic _autoPickupLogic;
        public NormalPickupLogic(SceneObjectContext sceneObjectContext,
            PlayerContext playerContext,
            ISceneObjectEntityFactory sceneObjectEntityFactory,
            RuntimeGameConfig runtimeGameConfig,
            int sceneWeaponLifetime)
        {
            _sceneObjectContext = sceneObjectContext;
            _playerContext = playerContext;
            _sceneObjectEntityFactory = sceneObjectEntityFactory;
            _sceneWeaponLifeTime = sceneWeaponLifetime;
            _runtimeGameConfig = runtimeGameConfig;
            _autoPickupLogic = new AutoPickupLogic(sceneObjectContext, playerContext, sceneObjectEntityFactory);
        }

        public virtual void DoPickup(int playerEntityId, int weaponEntityId)
        {
            var entity = _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(weaponEntityId, (short)EEntityType.SceneObject));
            if (null == entity)
            {
                Logger.ErrorFormat("{0} doesn't exist in scene object context ", weaponEntityId);
                return;
            }
            if(entity.hasThrowing)
            {
                return;
            }
            var player = _playerContext.GetEntityWithEntityKey(new EntityKey(playerEntityId, (short)EEntityType.Player));
            if (null == player)
            {
                Logger.ErrorFormat("{0} doesn't exist in player context ", playerEntityId);
                return;
            }
            if (!entity.hasWeapon)
            {
                Logger.ErrorFormat("only weapon is supported in normal mode");
                return;
            }
            if (!entity.IsCanPickUpByPlayer(player))
            {
                return;
            }
            _sceneObjectEntityFactory.DestroyEquipmentEntity(entity.entityKey.Value.EntityId);
            var last = player.GetController<PlayerWeaponController>().PickUpWeapon(entity.weapon.ToWeaponInfo());
            if (last.Id > 0)
            {
                _sceneObjectEntityFactory.CreateDropWeaponEntity(last, player.position.Value, _sceneWeaponLifeTime);
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
                case EWeaponSlotType.GrenadeWeapon:
                    DoDropGrenade(player);
                    return; 
            }
            var curWeapon = player.GetController<PlayerWeaponController>().GetSlotWeaponInfo(slot);
            if (curWeapon.Id > 0)
            {
                var dropPos = player.GetHandWeaponPosition();
                var playerTrans = player.characterContoller.Value.transform;
                var forward = playerTrans.forward;
                var pos = dropPos + forward * _runtimeGameConfig.WeaponDropOffset;
                RaycastHit hhit;
                SceneObjectEntity sceneObjectEntity;
                if (Physics.Raycast(dropPos, forward, out hhit, _runtimeGameConfig.WeaponDropOffset, UnityLayers.SceneCollidableLayerMask))
                {
                    RaycastHit vhit;
                    if(Physics.Raycast(hhit.point, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropWeaponEntity(curWeapon, vhit.point, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropWeaponEntity(curWeapon, playerTrans.position, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if(Physics.Raycast(pos, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropWeaponEntity(curWeapon, vhit.point, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateDropWeaponEntity(curWeapon, playerTrans.position, _sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                player.GetController<PlayerWeaponController>().DropSlotWeapon(slot);
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
