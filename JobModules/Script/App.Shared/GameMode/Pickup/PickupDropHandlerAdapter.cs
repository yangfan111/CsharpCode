using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.GameModules.Player;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Configuration;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="PickupDropHandlerAdapter" />
    /// </summary>
    public class PickupDropHandlerAdapter : IPickupHandler
    {
        private static readonly LoggerAdapter Logger =
            new LoggerAdapter(typeof(PickupDropHandlerAdapter));

        protected readonly Components.CommonSessionComponent commonSession;

        protected readonly ISceneObjectEntityFactory sceneObjectEntityFactory;

        public  void AutoPickupWeapon(PlayerEntity player, List<int> sceneKeys)
        {
            for (int i = 0; i < sceneKeys.Count; i++)
            {
                SceneObjectEntity sceneEntity = sceneObjectEntityFactory.GetSceneEntity(sceneKeys[i]) as SceneObjectEntity;
                //sceneKeys.Remove(sceneKeys[i]);
                if (sceneEntity == null || !sceneEntity.hasWeaponObject || sceneEntity.isFlagDestroy)
                {
                    //Logger.Warn("sceneEntity null failed");
                    continue;
                }

                EWeaponType_Config configType = (EWeaponType_Config) SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(sceneEntity.weaponObject.ConfigId).Type;
                if (!configType.CanAutoPick())
                {
                    //Logger.Warn("CanAutoPick failed");
                    continue;
                }
                if (configType == EWeaponType_Config.TacticWeapon && !sceneEntity.IsCanPickUpByPlayer(player))
                {
                    //Logger.Warn("IsCanPickUpByPlayer failed");
                    continue;
                }

                var  newWeaponScan = (WeaponScanStruct) sceneEntity.weaponObject;
                bool pickupResult  = player.WeaponController().AutoPickUpWeapon(newWeaponScan);
                if (!pickupResult)
                {
                    //Logger.Warn("pickupResult failed");
                    continue;
                }
              
                sceneObjectEntityFactory.DestroySceneWeaponObjectEntity(sceneEntity.entityKey.Value);
                if(player.WeaponController().HeldConfigId == sceneEntity.weaponObject.ConfigId)
                {
                    player.stateInterface.State.PickUp();
                }

                IEventArgs args = commonSession.FreeArgs as IEventArgs;
                if (null != args)
                {
                    TriggerArgs ta = new TriggerArgs();
                    ta.AddPara(new IntPara("weaponId", sceneEntity.weaponObject.ConfigId));
                    ta.AddUnit("current", (FreeData) player.freeData.FreeData);
                    args.Trigger(FreeTriggerConstant.WEAPON_PICKUP, ta);
                }
              
            }
        }

        protected readonly RuntimeGameConfig runtimeGameConfig;
        protected          int               sceneWeaponLifeTime;

        public PickupDropHandlerAdapter(Contexts context, int modelId)
        {
            sceneObjectEntityFactory = context.session.entityFactoryObject.SceneObjectEntityFactory;
            commonSession            = context.session.commonSession;
            runtimeGameConfig        = context.session.commonSession.RuntimeGameConfig;
            sceneWeaponLifeTime      = SingletonManager.Get<GameModeConfigManager>().GetWepaonStayTime(modelId);
        }

        public virtual void DoPickup(PlayerEntity player, int sceneKey)
        {
        }
        protected virtual void DoDropGrenade(PlayerEntity playerEntity, EWeaponSlotType slot, IUserCmd cmd)
        {
        }
        public virtual void Drop(PlayerEntity player, EWeaponSlotType slot, IUserCmd cmd)
        {
            if (slot == EWeaponSlotType.ThrowingWeapon)
            {
                DoDropGrenade(player,slot,cmd);
                return;
            }
            var heldAgent = player.WeaponController().HeldWeaponAgent;
            if (heldAgent.IsValid())
            {
                var  handPos = player.GetHandWeaponPosition();
                var  dropPos = player.position.Value + player.characterContoller.Value.transform.forward * runtimeGameConfig.WeaponDropOffset;

                var  weaponScacn = heldAgent.ComponentScan;
                bool generateSceneObj = player.WeaponController().DropWeapon(slot);
                if (!generateSceneObj || weaponScacn.IsUnSafeOrEmpty()) return;

                RaycastHit hhit;
                SceneObjectEntity sceneObjectEntity;
                if (Physics.Raycast(handPos, dropPos - handPos, out hhit, Vector3.Distance(handPos, dropPos), UnityLayers.PickupObstacleLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hhit.point + new Vector3(0, 0.1f, 0), Vector3.down, out vhit, 100, UnityLayers.PickupObstacleLayerMask))
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, hhit.point, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(dropPos + new Vector3(0, 0.1f, 0), Vector3.down, out vhit, 100, UnityLayers.PickupObstacleLayerMask))
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity = sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, dropPos, sceneWeaponLifeTime) as SceneObjectEntity;
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
                    ta.AddUnit("current", (FreeData) player.freeData.FreeData);
                    args.Trigger(FreeTriggerConstant.WEAPON_DROP, ta);
                }
            }
        }

        public virtual void SendPickup(IPlayerWeaponProcessor weaponProcessor, int entityId, int itemId, int category, int count)
        {
        }

        public virtual void SendAutoPickupWeapon(int entityId)
        {
        }

       
    }
}