using App.Shared;
using App.Shared.Components.Ui;
using App.Shared.Components.Player;
using Core.Free;
using Free.framework;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class SplitUiAdapter : UIAdapter, ISplitUiAdapter
    {
        private Contexts _contexts;
        public SplitUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

      
        public SplitPropInfo Info
        {
            get
            {
                return _contexts.ui.uI.SplitPropInfo;
            }

            set
            {
                _contexts.ui.uI.SplitPropInfo = value;
            }
        }


        public void SendSplitMessage(int number, string itemKey)
        {
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.SplitItem;
            data.Ins.Add(number);
            data.Ss.Add(itemKey);
            _contexts.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
        }
        public void SetCrossActive(bool isActive)
        {
            _contexts.ui.uI.IsShowCrossHair = isActive;
        }

        private PlayerEntity _player;
        private PlayerEntity Player
        {
            get { return _player ?? (_player = _contexts.player.flagSelfEntity); }
        }

        public void ShowIllegalTip()
        {
            Player.tip.TipType = Core.ETipType.EnterNumError;
        }
    }
}