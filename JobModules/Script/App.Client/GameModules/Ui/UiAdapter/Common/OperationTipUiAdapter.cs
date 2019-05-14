using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{

    public class OperationTipUiAdapter : UIAdapter, IOperationTipUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public OperationTipUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = contexts.ui.uI;
        }
        public List<ITipData> CountdownTipDataList
        {
            get { return _ui.CountdownTipDataList; }
        }

        public override bool Enable
        {
            get { return base.Enable && OperationTipData != null; }
            set { base.Enable = value; }
        }

        public ITipData OperationTipData
        {
            get { return _ui.OperationTipData; }
            set { _ui.OperationTipData = value; }
        }
    }
}
