using App.Client.GameMode.PickupLogic;
using App.Shared.GameModeLogic.LogicFactory;
using Core.GameModeLogic;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameMode.LogicFacotry
{
    class ClientNormalModeLogicFactory : NormalModeLogicFactory
    {
        private IUserCmdGenerator _userCmdGenerator;
        private Contexts _contexts;
        private IPickupLogic _pickupLogic;
        private int _modeId;

        public ClientNormalModeLogicFactory(IUserCmdGenerator userCmdGenerator,        
            Contexts contexts,
            int modeId) : base(contexts, contexts.session.commonSession, modeId)
        {
            _userCmdGenerator = userCmdGenerator;
            _contexts = contexts;
            _modeId = modeId;
        }

        protected override IPickupLogic GetPickupLogic()
        {
            if (null == _pickupLogic)
            {
                _pickupLogic = new ClientNormalPickupLogic(_userCmdGenerator,
                    _contexts,
                    _contexts.session.entityFactoryObject.SceneObjectEntityFactory,
                    _contexts.session.commonSession.RuntimeGameConfig,
                    SingletonManager.Get<GameModeConfigManager>().GetWepaonStayTime(_modeId) * 1000);
            }
            return _pickupLogic;
        }
    }
}
