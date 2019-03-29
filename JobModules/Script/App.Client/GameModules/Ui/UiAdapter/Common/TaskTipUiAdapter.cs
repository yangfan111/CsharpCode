using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class TaskTipData : ITaskTipData
    {
        public string Title { get; set; }
        public int OldProgress { get; set; }
        public int NewProgress { get; set; }
        public int TotalProgress { get; set; }
    }

    public class TaskTipUiAdapter : UIAdapter, ITaskTipUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public TaskTipUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = contexts.ui.uI;
        }

        public List<ITaskTipData> TaskTipDataList
        {
            get { return _ui.TaskTipDataList; }
        }
    }
}
