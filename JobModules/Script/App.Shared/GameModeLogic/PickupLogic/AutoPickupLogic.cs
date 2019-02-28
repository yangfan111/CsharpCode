using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Util;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.EntityComponent;
using Core.Free;
using Core.Utils;

namespace App.Shared.GameModeLogic.PickupLogic
{
    public class AutoPickupLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AutoPickupLogic));
        private SceneObjectContext _sceneObjectContext;
        private PlayerContext _playerContext;
        private Contexts _contexts;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;

        public AutoPickupLogic(Contexts contexts, ISceneObjectEntityFactory sceneObjectEntityFactory)
        {
            _contexts = contexts;
            _sceneObjectContext = contexts.sceneObject;
            _playerContext = contexts.player;
            _sceneObjectEntityFactory = sceneObjectEntityFactory;
        }

        public virtual void AutoPickupWeapon(int playerEntityId, int sceneEnityId)
        {
            var entity = _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(sceneEnityId, (short)EEntityType.SceneObject));
            if (null == entity)
            {
                Logger.ErrorFormat("{0} doesn't exist in scene object context ", sceneEnityId);
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
            if(!entity.IsCanPickUpByPlayer(player))
            {
                return;
            }
            var newWeaponScan = (WeaponScanStruct)entity.weaponObject;
            var pickupSuccess = player.WeaponController().AutoPickUpWeapon(newWeaponScan);
            if (pickupSuccess)
            {
                IEventArgs args = _contexts.session.commonSession.FreeArgs as IEventArgs;
                if (null != args)
                {
                    TriggerArgs ta = new TriggerArgs();
                    ta.AddPara(new IntPara("weaponId", entity.weaponObject.ConfigId));
                    ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                    args.Trigger(FreeTriggerConstant.WEAPON_PICKUP, ta);
                }
                _sceneObjectEntityFactory.DestroySceneWeaponObjectEntity(entity.entityKey.Value.EntityId);
            }
        }   
    }
}
