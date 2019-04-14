using System;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonDeadModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonDeadModel));
        private CommonDeadViewModel _viewModel = new CommonDeadViewModel();

        private IDeadUiAdapter _adapter;

        

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }


        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitGui();
        }

        Transform AnimeRoot;
        private void InitGui()
        {
            _viewModel.ButtonGroupShow = false;
            _viewModel.ContinueBtnClick = _adapter.BackToHall;
            _viewModel.ObserverBtnClick = _adapter.Observe;
            AnimeRoot = FindChildGo("MaskGroup");
            if (AnimeRoot != null)
            {
                Loader.LoadAsync(AssetBundleConstant.Effect_Common, "death_interface_ui", (obj) =>
                {
                    GameObject go = obj as GameObject;
                    go.transform.SetParent(AnimeRoot, false);
                });
            }
        }

        public CommonDeadModel(IDeadUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }

        
        public override void Update(float interval)
        {
            UpdateButtonState();
        }

        private bool haveRegister;
        private void UpdateButtonState()
        {
            if (!_adapter.DeadButtonShow)
            {
                _viewModel.ButtonGroupShow = false;
            }
            else
            {
                _viewModel.ButtonGroupShow = true;
                _viewModel.ContinueBtnText = _adapter.HaveAliveTeammate ? BackToHallText : ContinueText;
            }
        }

        private void RegisterReceiver()
        {
            if (!haveRegister)
            {
                haveRegister = true;
                _adapter.SetCrossVisible(false);
                _viewModel.MaskGroupShow = true;
            }
            
        }

        private void UnRegisterReceiver()
        {
            if (haveRegister)
            {
                haveRegister = false;
                _adapter.SetCrossVisible(true);
                _viewModel.MaskGroupShow = false;
            }
        }

        public string BackToHallText
        {
            get { return I2.Loc.ScriptLocalization.client_common.word67; }
        }

        public string ContinueText
        {
            get { return I2.Loc.ScriptLocalization.client_common.word68; }
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);
            if (!enable)
            {
                UnRegisterReceiver();
            }
            else
            {
                RegisterReceiver();
            }
           
        }
    }
}
