using App.Shared;
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
                return _player ?? (_player = _contexts.player.flagSelfEntity);
            }
        }

        public override bool IsReady()
        {
            return Player != null; 
        }

        public override bool Enable
        {
            get { return _enable && CanOpenBag; }

        }


        public bool CanOpenBag
        {
            get
            {
                return Player.WeaponController().CanSwitchWeaponBag;
            }
        }

    }
}
