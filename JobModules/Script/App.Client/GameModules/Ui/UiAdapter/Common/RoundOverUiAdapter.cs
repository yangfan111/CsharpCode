using App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public class RoundOverUiAdapter : UIAdapter, IRoundOverUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public RoundOverUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
            _ui = _contexts.ui.uI;
        }

        public int CurRoundCount
        {
            get { return _ui.CurRoundCount; }
        }

        public int GetScoreByCamp(EUICampType type)
        {
            return _ui.ScoreByCampTypeDict[type];
        }

        public override bool IsReady()
        {
            return CurRoundCount > 0;
        }
    }
}
