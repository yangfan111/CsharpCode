using App.Client.GameModules.Ui.UiAdapter.Interface.Common;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{

    public class VideoSettingUiAdapter : UIAdapter, IVideoSettingUiAdapter
    {
        private Contexts _contexts;

        public VideoSettingUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }
    }
}
