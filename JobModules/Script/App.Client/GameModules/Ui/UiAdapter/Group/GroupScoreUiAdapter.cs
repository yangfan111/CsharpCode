using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class GroupScoreUiAdapter : UIAdapter, IGroupScoreUiAdapter
    {
        private Contexts _contexts;

        public GroupScoreUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }


        public int GameTime
        {
            get
            {
                return _contexts.ui.uI.GameTime;
            }
            set
            {
                _contexts.ui.uI.GameTime = value;
            }
        }

        public int KillCountForWin
        {
            get
            {
                return _contexts.ui.uI.ScoreForWin;
            }
        }

        public int GetKillCountByCampType(EUICampType campType)
        {
            return _contexts.ui.uI.ScoreByCampTypeDict[(int)campType];
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
