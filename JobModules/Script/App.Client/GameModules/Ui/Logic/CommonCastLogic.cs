using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Player;
using App.Shared.Util;
using Core.EntityComponent;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Logic
{
    public class CommonCastLogic : AbstractCastLogic
    {
        private SceneObjectContext _sceneObjectContext;

        public CommonCastLogic(PlayerContext playerContext, SceneObjectContext sceneObjectContext, float maxDistance):base(playerContext, maxDistance)
        {
            _sceneObjectContext = sceneObjectContext;
        }

        public override void OnAction()
        {
        }

        protected override void DoSetData(PointerData data)
        {
            var entityId = CommonCastData.EntityId(data.IdList);
            var key = CommonCastData.Key(data.IdList);
            var tip = CommonCastData.Tip(data);
            var entity = _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(entityId, (short)EEntityType.SceneObject));
            if(null == entity)
            {
                return;
            }
            var player = _playerContext.flagSelfEntity;            
            if(null == player)
            {
                return;
            }
            if(!entity.IsCanPickUpByPlayer(player))
            {
                return;
            }
            if (IsUntouchableOffGround(player, data.Position, entity.rawGameObject.Value))
            {
                return;
            }
            if(player.hasRaycastTarget)
            {
                player.raycastTarget.Key = key;
            }
            else
            {
                player.AddRaycastTarget(key);
            }
            Tip = tip;
        }
    }
}
