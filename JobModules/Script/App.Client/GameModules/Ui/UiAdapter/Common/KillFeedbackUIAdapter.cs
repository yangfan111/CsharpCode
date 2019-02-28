using App.Client.GameModules.Ui.UiAdapter;
using System.Collections.Generic;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class KillFeedBackUiAdapter : UIAdapter, IKillFeedBackUiAdapter
    {
        private Contexts _contexts;
        public KillFeedBackUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public List<int> Types
        {
            get
            {
                return _contexts.ui.uI.KillFeedBackList;
            }
            
        }         
    }
}