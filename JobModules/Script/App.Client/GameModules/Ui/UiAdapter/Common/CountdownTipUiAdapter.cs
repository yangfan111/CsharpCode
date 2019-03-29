using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class CountdownTipData : ICountdownTipData
    {
        public string Title { get; set; }
        public long DurationTime { get; set; }
    }
    public class CountdownTipUiAdapter : UIAdapter, ICountdownTipUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public CountdownTipUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = contexts.ui.uI;
        }
        public List<ICountdownTipData> CountdownTipDataList
        {
            get { return _ui.CountdownTipDataList; }
        }

        public override bool Enable
        {
            get { return base.Enable && CountdownTipDataList != null && CountdownTipDataList.Count > 0; }
            set { base.Enable = value; }
        }
    }
}
