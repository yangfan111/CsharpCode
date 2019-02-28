using App.Shared.Components.Ui.UiAdapter;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class GameTitleUiAdapter : UIAdapter, IGameTitleUiAdapter
    {
        private Contexts _contexts;

        public GameTitleUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }

        public int GameTitle
        {
            get
            {
                return  _contexts.ui.uI.MyselfGameTitle;
            }
        }

    }

}
