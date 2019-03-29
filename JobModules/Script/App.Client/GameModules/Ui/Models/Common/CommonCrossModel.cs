using System.Collections.Generic;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;
using UserInputManager.Lib;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using XmlConfig;
using App.Shared.GameModules.Weapon;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonCrossModel : AbstractModel, IUiSystem
    {
        private int _maxTypeCount;
        private bool _isCfgInited;
        private int _curType;
        private readonly int _defaultType = 1;
        private readonly Color _defaultColor = Color.green;
        private PlayerContext _playerContext;

        private bool Show
        {
            get { return _viewModel.ShowGameObjectActiveSelf; }
            set
            {
                _viewModel.ShowGameObjectActiveSelf = value;
            }
        }

        public CommonCrossModel(Contexts contexts)
        {
            InitKeyBinding(contexts);
            _playerContext = contexts.player;
        }

        protected override List<UiAssetInfo> ConfigRequests
        {
            get
            {
                return new List<UiAssetInfo>()
                {
                    new UiAssetInfo(UiConstant.UiConfigBundleName, _viewModel.UiConfigAssetName)
                };
            }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();

            // 等待配置加载完成
            if (_isCfgInited)
            {
                InitCross();
            }
        }

        public void InitKeyBinding(Contexts contexts)
        {
            var receiver = new KeyReceiver(Layer.Ui, BlockType.None);
            receiver.AddAction(UserInputKey.Switch1, (data)=>{SelectType(1, _defaultColor);});
            receiver.AddAction(UserInputKey.Switch2, (data)=>{SelectType(2, _defaultColor);});
            receiver.AddAction(UserInputKey.Switch3, (data)=>{SelectType(3, _defaultColor);});
            receiver.AddAction(UserInputKey.Switch4, (data)=>{SelectType(4, _defaultColor);});
            receiver.AddAction(UserInputKey.Switch5, (data)=>{SelectType(5, _defaultColor);});
            receiver.AddAction(UserInputKey.Switch6, (data)=>{SelectType(6, _defaultColor);});
            contexts.userInput.userInputManager.Instance.RegisterKeyReceiver(receiver);
        }

        protected override void GetConfigValue()
        {
            var val = GetValue("MaxTypeCount");
            if (!int.TryParse(val, out _maxTypeCount))
            {
                _maxTypeCount = 6;
            }
            _isCfgInited = true;
            // 如果还没初始化执行初始化
            if (_curType < 1)
            {
                InitCross();
            }
        }

        CommonCrossViewModel _viewModel = new CommonCrossViewModel();

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

       public void OnUiRender(float interval)
        {
            if (!isVisible) return;

            if (null != _playerContext.flagSelfEntity)
            {
                var player = _playerContext.flagSelfEntity;
                if (player.cameraStateNew.ViewNowMode == (short) ECameraViewMode.GunSight)
                {
                    Show = false;
                }else if (player.cameraStateNew.MainNowMode == (short) ECameraPoseMode.AirPlane ||
                          player.cameraStateNew.MainNowMode == (short) ECameraPoseMode.DriveCar ||
                          player.cameraStateNew.MainNowMode == (short) ECameraPoseMode.Dead ||
                          player.cameraStateNew.MainNowMode == (short) ECameraPoseMode.Dying ||
                          player.cameraStateNew.MainNowMode == (short) ECameraPoseMode.Gliding ||
                          player.cameraStateNew.MainNowMode == (short) ECameraPoseMode.Parachuting
                )
                {
                    Show = false;
                }
                else
                {
                    Show = true;
                }
            }
        }

        private void InitCross()
        {
            SelectType(_defaultType, _defaultColor);
        }

        private void SelectType(int index, Color col)
        {
            for (var i = 1; i <= _maxTypeCount; i++)
            {
                var active = i == index;
                _viewModel.SetTypeGameObjectActiveSelf(i, active);
                _viewModel.SetCenterGameObjectActiveSelf(i, active);
                _viewModel.SetCenterImageColor(i, col);
                _viewModel.SetLeftGameObjectActiveSelf(i, active);
                _viewModel.SetLeftImageColor(i, col);
                _viewModel.SetRightGameObjectActiveSelf(i, active);
                _viewModel.SetRightImageColor(i, col);
                _viewModel.SetUpGameObjectActiveSelf(i, active);
                _viewModel.SetUpImageColor(i, col);
                _viewModel.SetDownGameObjectActiveSelf(i, active);
                _viewModel.SetDownImageColor(i, col);
                _viewModel.SetCycleGameObjectActiveSelf(i, active);
                _viewModel.SetCycleImageColor(i, col);
                _viewModel.SetCycleRectTransformLocalScale(i, Vector3.one);
            }

            _curType = index;
        }
    }
}
