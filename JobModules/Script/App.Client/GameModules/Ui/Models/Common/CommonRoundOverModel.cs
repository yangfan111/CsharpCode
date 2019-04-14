using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonRoundOverModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonRoundOverModel));
        private CommonRoundOverViewModel _viewModel = new CommonRoundOverViewModel();
        private IRoundOverUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonRoundOverModel(IRoundOverUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
        }

        Transform AnimeRoot;
        private void InitVariable()
        {
            AnimeRoot = FindChildGo("RoundGroup");
            if (AnimeRoot != null)
            {
                Loader.LoadAsync(AssetBundleConstant.Effect, "huihejieshu", (obj) =>
                {
                    GameObject go = obj as GameObject;
                    go.transform.SetParent(AnimeRoot, false);
                });
            }
        }


        public override void Update(float intervalTime)
        {

            //if (!UpdateViewShow())
            //{
            //    return;
            //}

            UpdateRoundInfo();
            UpdateScore();
        }

        private void UpdateScore()
        {
            _viewModel.CampScoreText1 = _adapter.GetScoreByCamp(EUICampType.T).ToString();
            _viewModel.CampScoreText2 = _adapter.GetScoreByCamp(EUICampType.CT).ToString();
        }

        private void UpdateRoundInfo()
        {
            _viewModel.TitleText = string.Format(I2.Loc.ScriptLocalization.client_common.word18, _adapter.CurRoundCount);
        }

        //private bool UpdateViewShow()
        //{
        //    bool show = _adapter.IsShow;

        //    _viewModel.Show = show;

        //    return show;
        //}
    }
}
