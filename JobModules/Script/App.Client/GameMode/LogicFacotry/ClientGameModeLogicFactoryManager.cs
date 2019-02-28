using App.Shared.GameModeLogic.LogicFactory;
using Core.GameModeLogic;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameMode.LogicFacotry
{
    public  class ClientGameModeLogicFactoryManager 
    {

        public static IGameModeLogicFactory GetModeLogicFactory(Contexts contexts, int gameMode)
        {
            var bagType = SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(gameMode);
            switch (bagType)
            {
                case EBagType.Chicken:
                    return new ClientSurvivalModeLogicFactory(contexts.session.clientSessionObjects.UserCmdGenerator, contexts);
                case EBagType.Group:
                default:
                    return new ClientNormalModeLogicFactory(contexts.session.clientSessionObjects.UserCmdGenerator, contexts, gameMode);
            }
        }
    }
}