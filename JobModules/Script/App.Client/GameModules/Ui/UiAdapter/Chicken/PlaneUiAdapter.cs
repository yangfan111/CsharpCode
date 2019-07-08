using App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public class PlaneUiAdapter : UIAdapter, IPlaneUiAdapter
    {
        private Contexts _contexts;
        public PlaneUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public int CurCount
        {
            get { return _contexts.ui.uI.CurPlayerCountInPlane; }

        }

        public int TotalCount
        {
            get
            {
                return  _contexts.ui.uI.TotalPlayerCountInPlane; 
            }
        }

        private PlayerEntity _player;
        public PlayerEntity Player
        {
            get
            {
                return _contexts.ui.uI.Player;
            }
        }

        public override bool Enable
        {
            get { return base.Enable && Player.gamePlay.GameState == Shared.Components.GameState.AirPlane; }
        }
    }
}
