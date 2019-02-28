using App.Client.GameMode.PickupLogic;
using App.Shared.GameModeLogic.LogicFactory;
using Core.GameModeLogic;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Client.GameMode.LogicFacotry
{
    class ClientSurvivalModeLogicFactory : SurvivalModeLogicFactory
    {
        private IUserCmdGenerator _userCmdGenerator;
     
        Contexts _contexts;
        private IPickupLogic _pickupLogic;
        
        public ClientSurvivalModeLogicFactory(IUserCmdGenerator cmdGenerator, 
           
            Contexts contexts):base(contexts, contexts.session.commonSession)
        {
            _userCmdGenerator = cmdGenerator;
            
            _contexts = contexts;
        }

        protected override IPickupLogic GetPickupLogic()
        {
            if(null == _pickupLogic)
            {
                _pickupLogic = new ClientSurvivalPickupLogic(_userCmdGenerator,  _contexts);
            }
            return _pickupLogic;
        }
    }
}
