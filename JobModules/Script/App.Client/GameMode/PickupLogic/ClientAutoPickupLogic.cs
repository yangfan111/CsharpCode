using App.Client.CastObjectUtil;
using App.Shared;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Client.GameMode.PickupLogic
{
    public class ClientAutoPickupLogic
    {
        private PlayerContext _playerContext;
        private SceneObjectContext _sceneObjectContext;
        private IUserCmdGenerator _userCmdGenerator;
        private Contexts _contexts;

        public ClientAutoPickupLogic(Contexts contexts, IUserCmdGenerator userCmdGenerator)
        {
            _contexts = contexts;
            _playerContext = contexts.player;
            _sceneObjectContext = contexts.sceneObject;
            _userCmdGenerator = userCmdGenerator;
        }

        public void SendAutoPickupWeapon(int entityId)
        {
            var player = _playerContext.flagSelfEntity;
            var target = _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(entityId, (short)EEntityType.SceneObject));
            var model = target.hasUnityObject ? target.unityObject.UnityObject : target.multiUnityObject.FirstAsset;
            if(!CommonObjectCastUtil.HasObstacleBeteenPlayerAndItem(player, target.position.Value, model))
            {
                _userCmdGenerator.SetUserCmd((cmd) => cmd.PickUpEquip = entityId);     
            }
        }
    }
}
