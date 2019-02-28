using App.Client.GameModules.Ui.UiAdapter;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class ScreenFlashUiAdapter : UIAdapter, IScreenFlashUiAdapter
    {
        private Contexts _contexts;

        public ScreenFlashUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }


        public bool IsShow
        {
            get
            {
                var player = _contexts.player.flagSelfEntity;
                if(null != player && player.gamePlay.IsLifeState(Shared.Components.Player.EPlayerLifeState.Dead))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsDirty
        {
            get
            {
                return _contexts.ui.uI.ScreenFlashInfo != null && _contexts.ui.uI.ScreenFlashInfo.IsShow;
            }
        }

        public float Alpha
        {
            get
            {
                return _contexts.ui.uI.ScreenFlashInfo.Alpha;
            }
        }

        public float KeepTime
        {
            get
            {
                return _contexts.ui.uI.ScreenFlashInfo.KeepTime;
            }
        }

        public float DecayTime
        {
            get
            {
                return _contexts.ui.uI.ScreenFlashInfo.DecayTime;
            }
        }

        public void Clear()
        {
            _contexts.ui.uI.ScreenFlashInfo = null;
        }
    }
}
