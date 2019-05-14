using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{

    public class SystemTipUiAdapter : UIAdapter, ISystemTipUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public SystemTipUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = contexts.ui.uI;
        }


        public Queue<ITipData> SystemTipDataQueue
        {
            get
            {
                return _ui.SystemTipDataQueue;
            }

        }
    }
}
