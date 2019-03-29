using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using App.Shared.Util;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Configuration;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="CommonPickupDropHandler" />
    /// </summary>
    public class CommonPickupDropHandler : PickupDropHandlerAdapter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonPickupDropHandler));

        private int sceneWeaponLifeTime;

        private readonly RuntimeGameConfig runtimeGameConfig;

        public CommonPickupDropHandler(Contexts contexts,int modelId) : base(contexts)
        {
            sceneWeaponLifeTime = SingletonManager.Get<GameModeConfigManager>().GetWepaonStayTime(modelId);
            runtimeGameConfig = contexts.session.commonSession.RuntimeGameConfig;
        }

        public override void DoPickup(PlayerEntity player, int sceneKey)
        {
            var sceneEntity = sceneObjectEntityFactory.GetSceneEntity(sceneKey) as SceneObjectEntity;
            if (sceneEntity == null|| sceneEntity.hasThrowing || !sceneEntity.hasWeaponObject || !sceneEntity.IsCanPickUpByPlayer(player))
            {
                Logger.ErrorFormat("only weapon is supported in normal mode");
                return;

            }
            var controller = player.WeaponController();
            //销毁场景武实体
            sceneObjectEntityFactory.DestroySceneWeaponObjectEntity(sceneEntity.entityKey.Value);
            var lastWeaponScan = controller.GetWeaponAgent(sceneEntity.weaponObject.ConfigId).ComponentScan;
            var newWeaponScan = (WeaponScanStruct)sceneEntity.weaponObject;
            bool pickupSuccess = false;
            bool generateSceneObj = controller.PickUpWeapon(newWeaponScan, ref pickupSuccess);
            IEventArgs args = commonSession.FreeArgs as IEventArgs;
            if (pickupSuccess && null != args)
            {
                TriggerArgs ta = new TriggerArgs();
                ta.AddPara(new IntPara("weaponId", newWeaponScan.ConfigId));
                ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                args.Trigger(FreeTriggerConstant.WEAPON_PICKUP, ta);
            }
            
            if (!generateSceneObj || lastWeaponScan.IsUnSafeOrEmpty())
                return;
            //  WeaponEntity lastEntity = WeaponEntityFactory.CreateEntity(lastWeaponScan);
            //newWeaponScan = WeaponUtil.CreateScan(lastEntity);
            sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(lastWeaponScan, player.position.Value, sceneWeaponLifeTime) ;
            if (null != args)
            {
                TriggerArgs ta = new TriggerArgs();
                ta.AddPara(new IntPara("weaponId", lastWeaponScan.ConfigId));
                ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                ta.AddPara(new FloatPara("weaponx", player.position.Value.x));
                ta.AddPara(new FloatPara("weapony", player.position.Value.y));
                ta.AddPara(new FloatPara("weaponz", player.position.Value.z));
                args.Trigger(FreeTriggerConstant.WEAPON_DROP, ta);
            }
        }

        protected virtual void DoDropGrenade(PlayerEntity playerEntity)
        {
        }

        public override void Drop(PlayerEntity player, EWeaponSlotType slot, IUserCmd cmd)
        {
            switch (slot)
            {
                case EWeaponSlotType.ThrowingWeapon:
                    DoDropGrenade(player);
                    return;
            }
            var heldAgent = player.WeaponController().HeldWeaponAgent;
            if (heldAgent.IsValid())
            {
                var dropPos = player.GetHandWeaponPosition();
                var playerTrans = player.characterContoller.Value.transform;
                var forward = playerTrans.forward;
                var pos = dropPos + forward * runtimeGameConfig.WeaponDropOffset;
                var weaponScacn = heldAgent.ComponentScan;
                bool generateSceneObj = player.WeaponController().DropWeapon(slot);
                if (!generateSceneObj || weaponScacn.IsUnSafeOrEmpty()) return;
           //     DebugUtil.LogInUnity(weaponScacn.ToString(), DebugUtil.DebugColor.Black);
                RaycastHit hhit;
                SceneObjectEntity sceneObjectEntity;
                if (Physics.Raycast(dropPos, forward, out hhit, runtimeGameConfig.WeaponDropOffset, UnityLayers.SceneCollidableLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hhit.point, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, playerTrans.position, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(pos, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, playerTrans.position, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
        
                IEventArgs args = commonSession.FreeArgs as IEventArgs;
                if (null != args && null != sceneObjectEntity)
                {
                    TriggerArgs ta = new TriggerArgs();
                    ta.AddPara(new IntPara("weaponId", weaponScacn.ConfigId));
                    ta.AddPara(new FloatPara("weaponx", sceneObjectEntity.position.Value.x));
                    ta.AddPara(new FloatPara("weapony", sceneObjectEntity.position.Value.y));
                    ta.AddPara(new FloatPara("weaponz", sceneObjectEntity.position.Value.z));
                    ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                    args.Trigger(FreeTriggerConstant.WEAPON_DROP, ta);
                }
            }
        }
    }
}
