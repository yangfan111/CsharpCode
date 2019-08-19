using App.Client.GameModules.Ui.UiAdapter.Interface.Blast;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class BlastC4TipUiAdapter : UIAdapter, IBlastC4TipUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public BlastC4TipUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = contexts.ui.uI;
        }

        public EUIBombInstallState C4InstallState
        {
            get { return _ui.C4InstallState; }
        }

        public float C4InitialProgress
        {
            get { return _ui.C4InitialProgress; }
        }

        public override bool Enable
        {
            get
            {
                return base.Enable && C4InstallState == EUIBombInstallState.Installed;
            }
        }

    }
}
