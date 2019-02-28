using System;
using App.Shared;
using App.Shared.Components.Ui;
using App.Client.GameModules.Ui.UiAdapter;
using Core.Free;
using Free.framework;
using UserInputManager.Lib;

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
    }
}