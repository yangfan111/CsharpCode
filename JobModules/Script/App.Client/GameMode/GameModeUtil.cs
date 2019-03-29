using App.Shared;
using App.Shared.GameMode;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameMode
{
    /// <summary>
    /// Defines the <see cref="ModeUtil" />
    /// </summary>
    public static class ModeUtil
    {
        public static GameModeControllerBase CreateClientMode(Contexts contexts, int gameMode)
        {
            var bagType = SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(gameMode);
            GameModeControllerBase controller;
            switch (bagType)
            {
                case EBagType.Chicken:
                    controller = new ClientGameSurviveModeController();
                    break;
                case EBagType.Group:
                default:
                    controller = new ClientGameCommonModeController();
                    break;
            }
            controller.Initialize(contexts, gameMode);
            return controller;
        }
    }
}
