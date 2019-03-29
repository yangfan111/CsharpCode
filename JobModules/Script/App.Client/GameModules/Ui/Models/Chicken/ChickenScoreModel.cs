using App.Client.GameModules.Ui.ViewModels.Chicken;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;

namespace App.Client.GameModules.Ui.Models.Chicken
{

    public class ChickenScoreModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenScoreModel));
        private ChickenScoreViewModel _viewModel = new ChickenScoreViewModel();

        private IChickenScoreUiAdapter _chickenScoreState;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
        }


        private void InitVariable()
        {
        }



        public ChickenScoreModel(IChickenScoreUiAdapter chickenScoreState):base(chickenScoreState)
        {
            _chickenScoreState = chickenScoreState;
        }

        public override void Update(float interval)
        {

            if (!_chickenScoreState.IsGameBegin)//游戏未开始显示加入人数，反之显示玩家淘汰数量和剩余玩家
            {
                _viewModel.JoinPlayerCountString = _chickenScoreState.JoinPlayerCount.ToString();
                _viewModel.JoinGroupShow = true;
                _viewModel.TotalPlayerInfoShow = false;
                return;
            }
            else
            {
                _viewModel.JoinGroupShow = false;
                _viewModel.TotalPlayerInfoShow = true;
            }
            UpdateTotalPlayerInfo();
        }

        /// <summary>
        /// 更新幸存者数量和当前玩家击杀数量
        /// </summary>
        private void UpdateTotalPlayerInfo()
        {
            var beatCount = _chickenScoreState.BeatCount;
            var survialCount = _chickenScoreState.SurvivalCount;

            if (beatCount > 0)
            {
                _viewModel.BeatGroupShow = true;
            }

            _viewModel.BeatPlayerCountString = beatCount.ToString();
            _viewModel.SurvivalPlayerCountString = survialCount.ToString();
        }
    }
}


