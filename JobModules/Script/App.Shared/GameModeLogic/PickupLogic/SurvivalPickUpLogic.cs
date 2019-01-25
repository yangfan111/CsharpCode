﻿using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.WeaponLogic;
using Core;
using Core;
using Core.Configuration;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModeLogic.PickupLogic
{
    public class SurvivalPickupLogic : DummyPickupLogic 
    {
        private PlayerContext _playerContext;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;
        private RuntimeGameConfig _runtimeGameConfig;
        private AutoPickupLogic _autoPickupLogic;
        private Contexts _contexts;

        public SurvivalPickupLogic(Contexts contexts,
            ISceneObjectEntityFactory sceneObjectEntityFactory,
            RuntimeGameConfig runtimeGameConfig)
        {
            _contexts = contexts;
            _playerContext = contexts.player;
            _runtimeGameConfig = runtimeGameConfig;
            _sceneObjectEntityFactory = sceneObjectEntityFactory;
            _autoPickupLogic = new AutoPickupLogic(contexts, sceneObjectEntityFactory);
        }

        public override void Dorp(int playerEntityId, EWeaponSlotType slot)
        {
            //使用服务器操作
            var player = _playerContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(playerEntityId, (short)EEntityType.Player));
            var weapon = player.GetWeaponEntity(_contexts, slot);
            if(null != weapon)
            {
                weapon.isFlagDestroy = true;
            }
            return;
            var weaponAchive = player.GetController<PlayerWeaponController>();
           var curWeapon = weaponAchive.GetSlotWeaponInfo(_contexts, slot);
            if (curWeapon.Id > 0)
            {
                var dropPos = player.GetHandWeaponPosition();
                var playerTrans = player.characterContoller.Value.transform;
                var forward = playerTrans.forward;
                var pos = dropPos + forward * _runtimeGameConfig.WeaponDropOffset;
                RaycastHit hhit;
                SceneObjectEntity sceneObjectEntity;
                if(Physics.Raycast(dropPos, forward, out hhit, _runtimeGameConfig.WeaponDropOffset, UnityLayers.DefaultLayerMask | UnityLayers.TerrainlayerMask))
                {
                    RaycastHit vhit;
                    if(Physics.Raycast(hhit.point, Vector3.down, out vhit, 100, UnityLayers.DefaultLayerMask | UnityLayers.TerrainlayerMask))
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateWeaponEntity(curWeapon, vhit.point) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateWeaponEntity(curWeapon, playerTrans.position) as SceneObjectEntity;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if(Physics.Raycast(pos, Vector3.down, out vhit, 100, UnityLayers.DefaultLayerMask | UnityLayers.TerrainlayerMask))
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateWeaponEntity(curWeapon, vhit.point) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = _sceneObjectEntityFactory.CreateWeaponEntity(curWeapon, playerTrans.position) as SceneObjectEntity;
                    }
                }
                player.GetController<PlayerWeaponController>().DropSlotWeapon(_contexts, slot);
            }
        }

        public override void AutoPickupWeapon(int playerEntityId, int entityId)
        {
            _autoPickupLogic.AutoPickupWeapon(playerEntityId, entityId);
        }
    }
}