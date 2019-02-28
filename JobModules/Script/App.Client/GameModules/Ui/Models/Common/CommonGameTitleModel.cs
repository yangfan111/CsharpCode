using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonGameTitleModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonGameTitleModel));
        private CommonGameTitleViewModel _viewModel = new CommonGameTitleViewModel();
        private IGameTitleUiAdapter _gameOverState;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonGameTitleModel(IGameTitleUiAdapter gameOverState):base(gameOverState)
        {
            _gameOverState = gameOverState;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
        }


        private void InitVariable()
        {
 
        }

        public override void Update(float intervalTime)
        {
            if (!isVisible || !_viewInitialized) return;

            UpdateGameTitle();
        }

        private void UpdateGameTitle()
        {
            var gameTitle = _gameOverState.GameTitle;
            _viewModel.KdShow = IsTitle(EUIGameTitleType.KdKing, gameTitle);
            _viewModel.AceShow = IsTitle(EUIGameTitleType.Ace, gameTitle);
            _viewModel.SecondShow = IsTitle(EUIGameTitleType.Second, gameTitle);
            _viewModel.ThirdShow = IsTitle(EUIGameTitleType.Third, gameTitle);

        }

        private bool IsTitle(EUIGameTitleType titleType, int title)
        {
            int type = 1 << (int)titleType;
            return (title & type) == type;
        }
    }
}
