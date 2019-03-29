using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Util;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="PickupDropHandlerAdapter" />
    /// </summary>
    public class PickupDropHandlerAdapter : IPickupHandler
    {
        protected readonly Components.CommonSessionComponent commonSession;

        protected readonly ISceneObjectEntityFactory sceneObjectEntityFactory;

        public void AutoPickupWeapon(PlayerEntity player,int sceneKey )
        {

            SceneObjectEntity sceneEntity = sceneObjectEntityFactory.GetSceneEntity(sceneKey) as SceneObjectEntity;
            if (sceneEntity == null|| !sceneEntity.IsCanPickUpByPlayer(player))
                return;
            if (!sceneEntity.hasWeaponObject)
                return;
            var newWeaponScan = (WeaponScanStruct)sceneEntity.weaponObject;
            bool pickupResult = player.WeaponController().AutoPickUpWeapon(newWeaponScan);

            if (!pickupResult) return;
            sceneObjectEntityFactory.DestroySceneWeaponObjectEntity(sceneEntity.entityKey.Value);
            IEventArgs args = commonSession.FreeArgs as IEventArgs;
            if (null != args)
            {
                
                TriggerArgs ta = new TriggerArgs();
                ta.AddPara(new IntPara("weaponId", sceneEntity.weaponObject.ConfigId));
                ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                args.Trigger(FreeTriggerConstant.WEAPON_PICKUP, ta);
            }
        }

        public PickupDropHandlerAdapter(Contexts context)
        {
            sceneObjectEntityFactory = context.session.entityFactoryObject.SceneObjectEntityFactory;
            commonSession = context.session.commonSession;
        }

        public virtual void DoPickup(PlayerEntity player, int sceneKey)
        {
        }

        public virtual void Drop(PlayerEntity player, EWeaponSlotType slot, IUserCmd cmd)
        {
        }

        public virtual void SendAutoPickupWeapon(int entityId)
        {
        }

        public virtual void SendPickup(int entityId, int itemId, int category, int count)
        {
        }
    }
}
