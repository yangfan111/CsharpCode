using App.Client.GameModules.Ui.UiAdapter;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class CountDownUiAdapter : UIAdapter, ICountDownUiAdapter
    {
        private Contexts _contexts;
        public CountDownUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public bool StartCountDown { get { return _contexts.ui.uIEntity.uI.CountingDown; } set { _contexts.ui.uIEntity.uI.CountingDown = value; } }
        public float CountDownNum { get { return _contexts.ui.uIEntity.uI.CountDownNum; }  set { _contexts.ui.uIEntity.uI.CountDownNum = value; } }
        public void SetCrossActive(bool isActive)
        {
            _contexts.ui.uI.IsShowCrossHair = isActive;
        }
        public bool CrossActiveStatue()
        {
            return _contexts.ui.uI.IsShowCrossHair;
        }
    }
}
