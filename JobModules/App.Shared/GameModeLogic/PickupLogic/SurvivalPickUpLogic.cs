using App.Shared.GameModules.Player;
using App.Shared.WeaponLogic;
using Core;
using Core.Bag;
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

        public SurvivalPickupLogic(PlayerContext playerContext, 
            SceneObjectContext sceneObjectContext,
            ISceneObjectEntityFactory sceneObjectEntityFactory,
            RuntimeGameConfig runtimeGameConfig)
        {
            _playerContext = playerContext;
            _runtimeGameConfig = runtimeGameConfig;
            _sceneObjectEntityFactory = sceneObjectEntityFactory;
            _autoPickupLogic = new AutoPickupLogic(sceneObjectContext, playerContext, sceneObjectEntityFactory);
        }

        public override void Dorp(int playerEntityId, EWeaponSlotType slot)
        {
            //使用服务器操作
            return;
            var player = _playerContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(playerEntityId, (short)EEntityType.Player));
            var bagImp = player.bag.Bag as PlayerWeaponComponentAgent;
           var curWeapon = bagImp.GetSlot_WeaponInfo(slot);
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
                player.playerAction.Logic.DropSlotWeapon(slot);
            }
        }

        public override void AutoPickupWeapon(int playerEntityId, int entityId)
        {
            _autoPickupLogic.AutoPickupWeapon(playerEntityId, entityId);
        }
    }
}
