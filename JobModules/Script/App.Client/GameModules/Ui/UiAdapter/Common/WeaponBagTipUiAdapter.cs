using App.Shared.Components.Ui.UiAdapter;
using Core.Utils;


namespace App.Client.GameModules.Ui.UiAdapter
{
    public class WeaponBagTipUiAdapter : UIAdapter, IWeaponBagTipUiAdapter
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBagUiAdapter));
        private Contexts _contexts;
        private PlayerEntity _player;

        public WeaponBagTipUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }

        public PlayerEntity Player 
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;
            }
        }

        public override bool IsReady()
        {
            return Player != null; ;
        }

        public override bool Enable
        {
            get { return _enable && CanOpenBag; }

            set {
                    _enable = value;
                }
        }


        public bool CanOpenBag
        {
            get
            {
                if (null != Player && Player.modeLogic.ModeLogic.IsBagSwithEnabled(Player))
                {
                    return true;
                }
                return false;
            }
        }

    }
}
