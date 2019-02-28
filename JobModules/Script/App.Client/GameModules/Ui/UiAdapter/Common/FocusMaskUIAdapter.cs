namespace App.Client.GameModules.Ui.UiAdapter
{
    public class FocusMaskUIAdapter: UIAdapter,IFocusMaskUIAdapter
    {
        private Contexts _contexts;
        private UiContext _uiContext;
        public FocusMaskUIAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _uiContext = contexts.ui;
        }

        public bool IsShowCrossHair
        {
            get { return _uiContext.uI.IsShowCrossHair; }
        }
    }
}