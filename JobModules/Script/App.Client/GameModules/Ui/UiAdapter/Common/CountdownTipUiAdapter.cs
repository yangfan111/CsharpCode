using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class BaseTipData : ITipData
    {
        public string Title { get; set; }
        public long DurationTime { get; set; }
        public int Id { get; set; }
        public override string ToString()
        {
            return "Title:" + Title + " Duratime:" + DurationTime;
        }
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
        public List<ITipData> CountdownTipDataList
        {
            get { return _ui.CountdownTipDataList; }
        }

        public long CurTime
        {
            get { return DateTime.Now.Ticks / 10000; }
        }

        public override bool Enable
        {
            get { return base.Enable && CountdownTipDataList != null && CountdownTipDataList.Count > 0; }
            set { base.Enable = value; }
        }

    }
}
