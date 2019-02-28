using App.Client.Utility;
using App.Shared.Components.Ui.UiAdapter;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class GameOverUiAdapter : UIAdapter,IGameOverUiAdapter
    {
        private Contexts _contexts;

        public GameOverUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }

        public EUIGameResultType GameResult
        {
            get { return _contexts.ui.uI.GameResult; }
        }

        public void GameOver()
        {
            HallUtility.GameOver();
        }
        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }
    }
}
