using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonGameOverModel : ClientAbstractModel, IUiHfrSystem
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonGameOverModel));
        private CommonGameOverViewModel _viewModel = new CommonGameOverViewModel();
        private IGameOverUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonGameOverModel(IGameOverUiAdapter adapter):base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
            InitReceiver();
        }

        private void InitReceiver()
        {
            pointerReceiver = new PointerReceiver(Layer.Env, BlockType.All);
        }

        Transform winAnime, loseAnime,tieAnime;

        private void InitGui()
        {
            _viewModel.ContinueBtnShow = false;
        }

        private void InitVariable()
        {
            winAnime = FindComponent<Transform>("shengli");
            loseAnime = FindComponent<Transform>("shibai");
            tieAnime = FindComponent<Transform>("pinju");
            _viewModel.ContinueBtnClick = ToSettlement;
        }

        private void ToSettlement()
        {
            _adapter.GameOver();
        }

        public override void Update(float intervalTime)
        {
            UpdateAnime();
        }

        private void UpdateAnime()
        {
            UIUtils.SetActive(winAnime,_adapter.GameResult == EUIGameResultType.Win);
            UIUtils.SetActive(loseAnime,_adapter.GameResult == EUIGameResultType.Lose);
            UIUtils.SetActive(tieAnime,_adapter.GameResult == EUIGameResultType.Tie);
            float curTime = Time.time;
            if ((curTime - baseTime) > 5f)
            {
                ShowContinueBtn();
            }

        }
        private PointerReceiver pointerReceiver;

        private void ShowContinueBtn()
        {
            _adapter.SetCrossVisible(false);
            _viewModel.ContinueBtnShow = true;
            RegisterReceiver();
        }

        private bool haveRegister = false;
        private void RegisterReceiver()
        {
            if (haveRegister) return;
            _adapter.RegisterPointerReceive(pointerReceiver);
            haveRegister = true;
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);
            if (!enable)
            {
                haveRegister = false;
                _adapter.UnRegisterPointerReceive(pointerReceiver);
            }
            if (enable)
            {
                baseTime = Time.time;
            }
        }

        float baseTime;
    }
}
