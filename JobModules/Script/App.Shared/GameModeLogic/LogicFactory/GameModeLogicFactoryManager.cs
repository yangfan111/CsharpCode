using Core.GameModeLogic;
using Core.GameModule.System;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModeLogic.LogicFactory
{
    public class GameModeLogicFactoryManager
    {
        public static IGameModeLogicFactory GetModeLogicFactory(Contexts contexts, int gameMode)
        {
            var bagType = SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(gameMode);
            switch (bagType)
            {
                case EBagType.Chicken:
                    return new SurvivalModeLogicFactory(contexts, contexts.session.commonSession);
                case EBagType.Group:
                default:
                    return new NormalModeLogicFactory(contexts, contexts.session.commonSession, gameMode);
            }
        }
    }
}