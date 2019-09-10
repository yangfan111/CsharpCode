using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class CommonMaxMap : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonMaxMap));
        private CommonMaxMapViewModel _viewModel = new CommonMaxMapViewModel();
        private IMapUiAdapter _adapter;
        private KeyHandler keyReceive = null;

        private CommonMap _map;

        private EUICampType _currentCamp = EUICampType.None;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonMaxMap(IMapUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            adapter.Enable = false;
            _map = new CommonMap(_adapter);
            _map.SetLinkParentModel(this);
            this.AddChildModel(_map);
            _map.Initialize();
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitKeyBinding();
            Init();
            _map.UIRoot(_viewModel.MapRoot);
            _map.SetVisible(true);
        }

        private void Init()
        {
            _viewModel.MapNameUIText.text = SingletonManager.Get<MapConfigManager>().SceneParameters.Name;
            if (!string.IsNullOrEmpty(_adapter.ChannelName) || !string.IsNullOrEmpty(_adapter.RoomName))
            {
                _viewModel.RoomInfoGroup.SetActive(true);
                _viewModel.RoomInfoUIText.text = !string.IsNullOrEmpty(_adapter.ChannelName)? string.Format("{0}/{1}", _adapter.ChannelName , _adapter.RoomName) : _adapter.RoomName;
            }
            else
            {
                _viewModel.RoomInfoGroup.SetActive(false);
            }

            var winDes = _adapter.GetWinConditionDescription();
            _viewModel.WinConditionGroup.SetActive(false);
            if (!string.IsNullOrEmpty(winDes))
            {
                _viewModel.WinConditionUIText.text = winDes;
                _viewModel.WinConditionGroup.SetActive(true);
            }

            _viewModel.NumberUIText.text = _adapter.PlayerCapacity.ToString();
            _viewModel.TeamGroup.SetActive(false);
            _viewModel.TipGroup.SetActive(false);
        }

        private void InitKeyBinding()
        {
            var handler = new KeyHandler(UiConstant.maxMapWindowLayer, BlockType.None);
            handler.BindKeyAction(UserInputKey.ShowMaxMap, (data) =>
            {
                ShowMap(!_adapter.Enable);
            });
            _adapter.RegisterOpenKey(handler);

            keyReceive = new KeyHandler(UiConstant.maxMapWindowKeyBlockLayer, BlockType.All);
            keyReceive.BindKeyAction(UserInputKey.HideWindow, (data) =>
            {
                if (_adapter.Enable)
                {
                    ShowMap(!_adapter.Enable);
                }
            });
        }

        protected override void OnCanvasEnabledUpdate(bool visible)
        {
            if (_viewInitialized)
            {
                if (visible)
                {
                    _adapter.SetCrossActive(false);
                    _adapter.HideUiGroup(Core.Ui.UiGroup.MapHide);
                    _adapter.RegisterKeyReceive(keyReceive);
                }
                else
                {
                    _adapter.SetCrossActive(true);
                    _adapter.ShowUiGroup(Core.Ui.UiGroup.MapHide);
                    _adapter.UnRegisterKeyReceive(keyReceive);
                }
            }
        }

        private void ShowMap(bool visible)
        {
            if (_viewInitialized)
            {
                if (visible && !_adapter.Enable)
                {
                    _adapter.Enable = true;
                    PlayerStateUtil.AddUIState(EPlayerUIState.MapOpen, _adapter.gamePlay);
                }
                else if (!visible && _adapter.Enable)
                {
                    _adapter.Enable = false;
                    PlayerStateUtil.RemoveUIState(EPlayerUIState.MapOpen, _adapter.gamePlay);
                }
            }
        }

        public new void OnUiRender(float interval)
        {
            base.OnUiRender(interval);
            _map.OnUiRender(interval);
        }

        public override void Update(float interval)
        {
            if (!isVisible) return;
            UpdateCamp();
        }

        private void UpdateCamp()
        {
            if (_currentCamp.Equals(_adapter.MyCamp)) return;
            _currentCamp = _adapter.MyCamp;
            _viewModel.TipUIText.text = _adapter.GetModeDescription();
            _viewModel.TipGroup.SetActive(true);
            _viewModel.TeamGroup.SetActive(true);
            if (_currentCamp.Equals(EUICampType.T))
            {
                _viewModel.TeamUIText.text = "灰烬";
            }
            else if (_currentCamp.Equals(EUICampType.CT))
            {
                _viewModel.TeamUIText.text = "泰坦";
            }
            else
            {
                _viewModel.TipGroup.SetActive(false);
                _viewModel.TeamGroup.SetActive(false);
            }

        }
       
    }
}