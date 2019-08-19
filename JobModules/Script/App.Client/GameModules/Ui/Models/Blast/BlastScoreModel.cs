using App.Client.GameModules.Ui.Models.Group;
using App.Client.GameModules.Ui.UiAdapter;
using Core.Utils;
using I2.Loc;

namespace App.Client.GameModules.Ui.Models.Blast
{

    public class BlastScoreModel : GroupScoreModel
    {
        private Contexts _contexts;


        public BlastScoreModel(IGroupScoreUiAdapter adapter) : base(adapter)
        {
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            ViewInstance.name = "BlastScore";
            Logger = new LoggerAdapter(typeof(BlastScoreModel));
        }

        protected override bool NeedUpdateTime
        {
            get { return true; }
        }

        protected override void RealUpdateWinCodition(int score)
        {
            var format = ScriptLocalization.client_blast.WinRoundFormat;
            _viewModel.ScoreText = string.Format(format, score.ToString());
        }
    }
}


