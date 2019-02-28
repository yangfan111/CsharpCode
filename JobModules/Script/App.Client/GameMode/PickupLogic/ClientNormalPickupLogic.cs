using App.Shared.GameModeLogic.PickupLogic;
using Core;
using Core.Configuration;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Client.GameMode.PickupLogic
{
    public class ClientNormalPickupLogic : NormalPickupLogic 
    {
        private IUserCmdGenerator _userCmdGenerator;
        private ClientAutoPickupLogic _clientAutoPickupLogic;

        public ClientNormalPickupLogic(IUserCmdGenerator userCmdGenerator,
            Contexts contexts,
            ISceneObjectEntityFactory sceneObjectEntityFactory,
            RuntimeGameConfig runtimeGameConfig,
            int sceneWeaponLifeTime) : base(contexts, sceneObjectEntityFactory, runtimeGameConfig, sceneWeaponLifeTime)
        {
            _userCmdGenerator = userCmdGenerator;
            _clientAutoPickupLogic = new ClientAutoPickupLogic(contexts, userCmdGenerator);
        }

        public override void SendPickup(int entityId, int itemId, int category, int count)
        {
            _userCmdGenerator.SetUserCmd((cmd) => cmd.PickUpEquip = entityId);     
            _userCmdGenerator.SetUserCmd((cmd) => cmd.IsManualPickUp= true);
        }

        public override void SendAutoPickupWeapon(int entityId)
        {
            _clientAutoPickupLogic.SendAutoPickupWeapon(entityId);
        }

        protected override void DoDropGrenade(PlayerEntity playerEntity)
        {
            if(null != _userCmdGenerator)
            {
                _userCmdGenerator.SetUserCmd((userCmd) => {
                    userCmd.IsLeftAttack = true;
                });
                playerEntity.weaponAutoState.AutoThrowing = true;
            }
        }
    }
}
