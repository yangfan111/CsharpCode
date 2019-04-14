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

        public virtual void AutoPickupWeapon(PlayerEntity player, List<int> sceneKeys)
        {
            for (int i = 0; i < sceneKeys.Count; i++)
            {
                SceneObjectEntity sceneEntity = sceneObjectEntityFactory.GetSceneEntity(sceneKeys[i]) as SceneObjectEntity;
                //sceneKeys.Remove(sceneKeys[i]);
                if (sceneEntity == null || !sceneEntity.hasWeaponObject)
                {
                    Logger.Warn("sceneEntity null failed");
                    return;
                }

                EWeaponType_Config configType = (EWeaponType_Config) SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(sceneEntity.weaponObject.ConfigId).Type;
                if (!configType.CanAutoPick())
                {
                    Logger.Warn("CanAutoPick failed");
                    return;
                }
                if (configType == EWeaponType_Config.TacticWeapon && !sceneEntity.IsCanPickUpByPlayer(player))
                {
                    Logger.Warn("IsCanPickUpByPlayer failed");
                    return;
                }

                var  newWeaponScan = (WeaponScanStruct) sceneEntity.weaponObject;
                bool pickupResult  = player.WeaponController().AutoPickUpWeapon(newWeaponScan);
                if (!pickupResult)
                {
                    Logger.Warn("pickupResult failed");
                    return;
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
                var  dropPos          = player.GetHandWeaponPosition();
                var  playerTrans      = player.characterContoller.Value.transform;
                var  forward          = playerTrans.forward;
                var  pos              = dropPos + forward * runtimeGameConfig.WeaponDropOffset;
                var  weaponScacn      = heldAgent.ComponentScan;
                bool generateSceneObj = player.WeaponController().DropWeapon(slot);
                if (!generateSceneObj || weaponScacn.IsUnSafeOrEmpty()) return;
                //     DebugUtil.LogInUnity(weaponScacn.ToString(), DebugUtil.DebugColor.Black);
                RaycastHit        hhit;
                SceneObjectEntity sceneObjectEntity;
                if (Physics.Raycast(dropPos, forward, out hhit, runtimeGameConfig.WeaponDropOffset,
                    UnityLayers.SceneCollidableLayerMask))
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(hhit.point, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity =
                            sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point,
                                sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity =
                            sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn,
                                playerTrans.position, sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                }
                else
                {
                    RaycastHit vhit;
                    if (Physics.Raycast(pos, Vector3.down, out vhit, 100, UnityLayers.SceneCollidableLayerMask))
                    {
                        sceneObjectEntity =
                            sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, vhit.point,
                                sceneWeaponLifeTime) as SceneObjectEntity;
                    }
                    else
                    {
                        sceneObjectEntity =
                            sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn,
                                playerTrans.position, sceneWeaponLifeTime) as SceneObjectEntity;
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

        public virtual void SendAutoPickupWeapon(int entityId)
        {
        }

        public virtual void SendPickup(int entityId, int itemId, int category, int count)
        {
        }
    }
}