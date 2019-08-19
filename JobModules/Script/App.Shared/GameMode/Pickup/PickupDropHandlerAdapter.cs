using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.player;
using App.Shared.Components;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System.Collections.Generic;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameMode
{
    /// <summary>
    ///     Defines the <see cref="PickupDropHandlerAdapter" />
    /// </summary>
    public class PickupDropHandlerAdapter : IPickupHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PickupDropHandlerAdapter));

        protected readonly CommonSessionComponent commonSession;

        protected readonly ISceneObjectEntityFactory sceneObjectEntityFactory;

        protected int sceneWeaponLifeTime;

        public PickupDropHandlerAdapter(Contexts context, int modelId)
        {
            sceneObjectEntityFactory = context.session.entityFactoryObject.SceneObjectEntityFactory;
            commonSession            = context.session.commonSession;
            sceneWeaponLifeTime      = SingletonManager.Get<GameModeConfigManager>().GetWepaonStayTime(modelId);
        }

        public void AutoPickupWeapon(PlayerEntity player, List<int> sceneKeys)
        {
            for (int i = 0; i < sceneKeys.Count; i++)
            {
                SceneObjectEntity sceneEntity = sceneObjectEntityFactory.GetSceneEntity(sceneKeys[i]) as SceneObjectEntity;
                if (sceneEntity == null || !sceneEntity.hasWeaponObject || sceneEntity.isFlagDestroy) continue;

                EWeaponType_Config configType = (EWeaponType_Config) SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(sceneEntity.weaponObject.ConfigId).Type;
                if (!configType.CanAutoPick()) continue;

                if (configType == EWeaponType_Config.TacticWeapon && !sceneEntity.IsCanPickUpByPlayer(player)) continue;

                var newWeaponScan = (WeaponScanStruct) sceneEntity.weaponObject;
                bool pickupResult = player.WeaponController().AutoPickUpWeapon(newWeaponScan);
                if (!pickupResult) continue;

                sceneObjectEntityFactory.DestroySceneWeaponObjectEntity(sceneEntity.entityKey.Value);

                if (player.WeaponController().HeldConfigId == sceneEntity.weaponObject.ConfigId && SharedConfig.IsServer)
                    PlayerAnimationAction.DoAnimation(null, PlayerAnimationAction.PickUp, player, true);

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

        public virtual void DoPickup(PlayerEntity player, int sceneKey)
        {
        }

        public virtual void Drop(PlayerEntity player, EWeaponSlotType slot, IUserCmd cmd)
        {
            if (slot == EWeaponSlotType.ThrowingWeapon)
            {
                DoDropGrenade(player, slot, cmd);
                return;
            }

            var heldAgent = player.WeaponController().HeldWeaponAgent;
            if (heldAgent.IsValid())
            {

                var  weaponScacn      = heldAgent.ComponentScan;
                bool generateSceneObj = player.WeaponController().DropWeapon(slot);
                if (!generateSceneObj || weaponScacn.IsUnSafeOrEmpty()) return;
                sceneObjectEntityFactory.CreateDropSceneWeaponObjectEntity(weaponScacn, player, sceneWeaponLifeTime,commonSession.FreeArgs );
            }
        }

        public virtual void SendPickup(IPlayerWeaponProcessor weaponProcessor, int entityId, int itemId, int category,
                                       int count)
        {
        }

        public virtual void SendAutoPickupWeapon(int entityId)
        {
        }

        protected virtual void DoDropGrenade(PlayerEntity playerEntity, EWeaponSlotType slot, IUserCmd cmd)
        {
        }
    }
}