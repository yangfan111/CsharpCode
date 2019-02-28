using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.GameModules.Player;
using App.Shared.Util;
using Assets.App.Shared.EntityFactory;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.Free;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModeLogic.PickupLogic
{
    /// <summary>
    /// Defines the <see cref="NormalPickupLogic" />
    /// </summary>
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
            var lastWeaponScan = controller.HeldWeaponAgent.BaseComponentScan; 
            var newWeaponScan = (WeaponScanStruct)entity.weaponObject;
            bool generateSceneObj = controller.PickUpWeapon(newWeaponScan);
            //创建/获取场景掉落对应武器实体
            if (!generateSceneObj || lastWeaponScan.IsUnSafeOrEmpty())
                return;
              //  WeaponEntity lastEntity = WeaponEntityFactory.CreateEntity(lastWeaponScan);
                //newWeaponScan = WeaponUtil.CreateScan(lastEntity);
            _sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(lastWeaponScan, player.position.Value, _sceneWeaponLifeTime);
            IEventArgs args = _contexts.session.commonSession.FreeArgs as IEventArgs;
            if (null != args)
            {
                TriggerArgs ta = new TriggerArgs();
                ta.AddPara(new IntPara("weaponId", newWeaponScan.ConfigId));
                ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                args.Trigger(FreeTriggerConstant.WEAPON_PICKUP, ta);
            }
        }

        public virtual void SendPickup(int entityId, int itemId, int category, int count)
        {
        }

        protected virtual void DoDropGrenade(PlayerEntity playerEntity)
        {
        }

        public virtual void Drop(int playerEntityId, EWeaponSlotType slot)
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
            var heldAgent = player.WeaponController().HeldWeaponAgent;
            if (heldAgent.IsVailed())
            {
                var dropPos = player.GetHandWeaponPosition();
                var playerTrans = player.characterContoller.Value.transform;
                var forward = playerTrans.forward;
                var pos = dropPos + forward * _runtimeGameConfig.WeaponDropOffset;
                var weaponScacn = heldAgent.BaseComponentScan;
                bool generateSceneObj = player.WeaponController().DropWeapon(slot);
                if (!generateSceneObj || weaponScacn.IsUnSafeOrEmpty()) return;
                DebugUtil.LogInUnity(weaponScacn.ToString(),DebugUtil.DebugColor.Black);
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

                IEventArgs args = _contexts.session.commonSession.FreeArgs as IEventArgs;
                if (null != args && null != sceneObjectEntity)
                {
                    TriggerArgs ta = new TriggerArgs();
                    ta.AddPara(new IntPara("weaponId", heldAgent.ConfigId));
                    ta.AddPara(new FloatPara("weaponx", sceneObjectEntity.position.Value.x));
                    ta.AddPara(new FloatPara("weapony", sceneObjectEntity.position.Value.y));
                    ta.AddPara(new FloatPara("weaponz", sceneObjectEntity.position.Value.z));
                    ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                    args.Trigger(FreeTriggerConstant.WEAPON_DROP, ta);
                }
            }
        }

        public virtual void SendAutoPickupWeapon(int entityId)
        {
        }

        public virtual void AutoPickupWeapon(int playerEntityId, int weaponEntityId)
        {
            _autoPickupLogic.AutoPickupWeapon(playerEntityId, weaponEntityId);
        }
    }
}
