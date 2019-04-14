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

     //   private int sceneWeaponLifeTime;

        //private readonly RuntimeGameConfig runtimeGameConfig;

        public CommonPickupDropHandler(Contexts contexts,int modelId) : base(contexts,modelId)
        {
         //   sceneWeaponLifeTime = SingletonManager.Get<GameModeConfigManager>().GetWepaonStayTime(modelId);
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
      
    }
}
