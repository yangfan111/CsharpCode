using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
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
            pointerReceiver = new PointerReceiver(UiConstant.specicalCmdKeyLayer - 1, BlockType.All);
            keyReceiver = new KeyReceiver(UiConstant.specicalCmdKeyLayer - 1, BlockType.All);
        }

        Transform winAnime, loseAnime,tieAnime;


        void InitEffect(Transform root, string bundle)
        {
            if (root == null) return;
            Loader.LoadAsync(AssetBundleConstant.Effect, bundle, (obj) =>
            {
                GameObject go = obj as GameObject;
                go.transform.SetParent(root, false);
            });
        }

        private void InitGui()
        {
            _viewModel.ContinueBtnShow = false;
            InitEffect(winAnime, "shengli");
            InitEffect(loseAnime, "shibai");
            InitEffect(tieAnime, "pinju");
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
        private KeyReceiver keyReceiver;

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
            _adapter.RegisterKeyReceive(keyReceiver);
            haveRegister = true;
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);
            if (!enable)
            {
                haveRegister = false;
                _adapter.UnRegisterPointerReceive(pointerReceiver);
                _adapter.UnRegisterKeyReceive(keyReceiver);
                _adapter.CanOpenUiByKey = true;
            }
            if (enable)
            {
                baseTime = Time.time;
                _adapter.CanOpenUiByKey = false;
                _adapter.HideUiGroup(Core.Ui.UiGroup.Singleton);
                _adapter.HideUiGroup(Core.Ui.UiGroup.GameOverHide);
            }
        }

        float baseTime;
    }
}
