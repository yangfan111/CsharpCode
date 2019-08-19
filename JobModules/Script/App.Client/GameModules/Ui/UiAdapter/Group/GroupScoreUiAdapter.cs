using System.Collections.Generic;
using App.Shared.Components.Ui;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class PlayerCountData : IPlayerCountData
    {
        public int PlayerCount { get; set; }
        public int DeadPlayerCount { get; set; }
    }
    public class GroupScoreUiAdapter : UIAdapter, IGroupScoreUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;

        public List<IGroupBattleData> GetBattleDataListByCampType(EUICampType type)
        {
            var list = _contexts.ui.uI.GroupBattleDataDict[(int)type];
            return list;
        }

        public GroupScoreUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = contexts.ui.uI;
        }
        public int GameTime
        {
            get { return _ui.GameTime; }
            set { _ui.GameTime = value; }
        }


        public int ScoreForWin
        {
            get { return _ui.ScoreForWin; }
        }

        public int GetScoreByCamp(EUICampType type)
        {
            return _ui.ScoreByCampTypeDict[(int)type];
        }

        public IPlayerCountData GetDataByCampType(EUICampType campType)
        {
            return _ui.PlayerCountByCampTypeDict[(int)campType];
        }

        public bool NeedPause
        {
            get
            {
                return _contexts.ui.uI.IsPause;
            }
        }
    }
}
