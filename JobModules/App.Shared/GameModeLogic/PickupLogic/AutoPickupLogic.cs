using App.Shared.WeaponLogic;
using App.Shared.Util;
using Core;
using Core.EntityComponent;
using Core.Utils;

namespace App.Shared.GameModeLogic.PickupLogic
{
    public class AutoPickupLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AutoPickupLogic));
        private SceneObjectContext _sceneObjectContext;
        private PlayerContext _playerContext;
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;

        public AutoPickupLogic(SceneObjectContext sceneObjectContext, PlayerContext playerContext, ISceneObjectEntityFactory sceneObjectEntityFactory)
        {
            _sceneObjectContext = sceneObjectContext;
            _playerContext = playerContext;
            _sceneObjectEntityFactory = sceneObjectEntityFactory;
        }

        public virtual void AutoPickupWeapon(int playerEntityId, int weaponEntityId)
        {
            var entity = _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(weaponEntityId, (short)EEntityType.SceneObject));
            if (null == entity)
            {
                Logger.ErrorFormat("{0} doesn't exist in scene object context ", weaponEntityId);
                return;
            }
            var player = _playerContext.GetEntityWithEntityKey(new EntityKey(playerEntityId, (short)EEntityType.Player));
            if (null == player)
            {
                Logger.ErrorFormat("{0} doesn't exist in player context ", playerEntityId);
                return;
            }
            if (!entity.hasWeapon)
            {
                Logger.ErrorFormat("only weapon is supported in normal mode");
                return;
            }
            if(!entity.IsCanPickUpByPlayer(player))
            {
                return;
            }
            var pickupSuccess = player.playerAction.Logic.AutoPickUpWeapon(entity.weapon.ToWeaponInfo());
            if (pickupSuccess)
            {
                _sceneObjectEntityFactory.DestroyEquipmentEntity(entity.entityKey.Value.EntityId);
            }
        }   
    }
}
