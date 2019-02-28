using Core.Utils;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class ChickenScoreUiAdapter : UIAdapter, IChickenScoreUiAdapter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenScoreUiAdapter));

        private Contexts _contexts;

        public bool IsGameBegin
        {
            get { return _contexts.ui.uI.IsGameBegin; }
        }

        public int JoinPlayerCount
        {
            get { return _contexts.ui.uI.JoinPlayerCount; }
        }

        public int SurvivalCount
        {
            get { return _contexts.ui.uI.SurvivalCount; }
        } 

        public int BeatCount { get{return _contexts.ui.uI.BeatCount; } }


        public ChickenScoreUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public override bool IsReady()
        {
            return null != _contexts.player.flagSelfEntity;
        }


        
    }


}
