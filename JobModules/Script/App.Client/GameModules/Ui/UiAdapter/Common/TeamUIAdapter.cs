using System.Collections.Generic;
using App.Shared.Components.Ui;
using App.Client.GameModules.Ui.UiAdapter;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class TeamUiAdapter : UIAdapter, ITeamUiAdapter
    {
        private Contexts _contexts;
        public TeamUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public List<MiniMapTeamPlayInfo> TeamPlayerInfos
        {
            get
            {
                var map = _contexts.ui.map;
                if (null != map)
                {
                    return map.TeamInfos;
                }
                return null;
            }
        }

        public int ModeID
        {
            get { return _contexts.session.commonSession.RoomInfo.TeamCapacity; }
        }
    }
}