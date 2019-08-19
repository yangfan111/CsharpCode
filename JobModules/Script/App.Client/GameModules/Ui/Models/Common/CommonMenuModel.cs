using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UserInputManager.Lib;
using UnityEngine.EventSystems;
using App.Shared.Components.Player;
using App.Client.Utility;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.GameModules.Player;
using Assets.App.Client.GameModules.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonMenuModel : ClientAbstractModel, IUiSystem
    {
        private IMenuUiAdapter menuUiAdapter = null;
        private bool isGameObjectCreated = false;
        private KeyReceiver keyReceive = null;
        private PointerReceiver pointerReceiver = null;

        private CommonMenuViewModel _viewModel = new CommonMenuViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonMenuModel(IMenuUiAdapter adapter):base(adapter)
        {
            menuUiAdapter = adapter;
            menuUiAdapter.Enable = false;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitGui();
            BindEventTrigger();
            InitKeyBinding();
        }
       public override void Update(float interval)
        {
            RefreshGui();
        }

        private void InitGui()
        {
        }

        private void RefreshGui()
        {
            if (_viewModel.rootActiveSelf)
            {
                menuUiAdapter.SetCrossVisible(false);
            }
        }

        private void BindEventTrigger()
        {
            _viewModel.OnBackGameETClickListener = OnBackGameETClick;
            _viewModel.OnSettingETClickListener = OnSettingETClick;
            _viewModel.OnBackHallETClickListener = OnBackHallETClick;
        }

        public void InitKeyBinding()
        {
            var receiver = new KeyReceiver(UiConstant.menuWindowLayer, BlockType.None);
            receiver.BindKeyAction(UserInputKey.OpenMenu, (data) =>
            {
                ShowMenu(true);
            });
            menuUiAdapter.RegisterOpenKey(receiver);
            DynamicKeyBinding();
        }

        void DynamicKeyBinding()
        {
            //保证 菜单界面打开的时候 打不开大地图界面
            pointerReceiver = new PointerReceiver(UiConstant.menuWindowPointBlockLayer, BlockType.All);
            keyReceive = new KeyReceiver(UiConstant.menuWindowKeyBlockLayer, BlockType.All);
            keyReceive.BindKeyAction(UserInputKey.OpenMenu, (data) =>
            {
                ShowMenu(false);
            });
        }

        void RegisterReceive()
        {
            menuUiAdapter.RegisterKeyReceive(keyReceive);
            menuUiAdapter.RegisterPointerReceive(pointerReceiver);
        }

        void UnRegisterReceive()
        {
            menuUiAdapter.UnRegisterKeyReceive(keyReceive);
            menuUiAdapter.UnRegisterPointerReceive(pointerReceiver);
        }

        public void ShowMenu(bool visible)
        {
            if(visible)
            {
                PlayerStateUtil.AddUIState(EPlayerUIState.ExitOpen, menuUiAdapter.gamePlay);
                menuUiAdapter.SetCrossVisible(false);
                _viewModel.rootActiveSelf = true;
                RegisterReceive();
            }
            else
            {
                if (showNotice)
                {
                    UiCommon.AlertManager.ClearQueueAndClose();
                    showNotice = false;
                }
                PlayerStateUtil.RemoveUIState(EPlayerUIState.ExitOpen, menuUiAdapter.gamePlay);
                menuUiAdapter.SetCrossVisible(true);
                _viewModel.rootActiveSelf = false;
                UnRegisterReceive();
            }
            if(menuUiAdapter.Enable != visible)
            menuUiAdapter.Enable = visible;
        }

        void OnBackGameETClick(BaseEventData data)
        {
            ShowMenu(false);
        }

        void OnSettingETClick(BaseEventData data)
        {
            _viewModel.rootActiveSelf = false;
            UnRegisterReceive();

            Action<bool> hideCallback = (active) =>
            {
                menuUiAdapter.SetInputManagerEnable(true);
                ShowMenu(false);
            };
            Action<bool> showCallback = (active) =>
            {
                menuUiAdapter.SetInputManagerEnable(false);
                menuUiAdapter.SetCrossVisible(false);                   //关系准心
            };

            HallUtility.ShowSettingWindow(showCallback);
            HallUtility.SetSettingWindowNovisCallBack(hideCallback);
        }

        void OnBackHallETClick(BaseEventData data)
        {    
            if (menuUiAdapter != null)
            {
                _viewModel.rootActiveSelf = false;
                //UnRegisterReceive();

                string title = "";
                bool isOverGame = menuUiAdapter.PlayStaue == (int)EPlayerLifeState.Dead || menuUiAdapter.WarVictory;
                bool isHx = menuUiAdapter.IsHXMod;
                title = 
                    isOverGame ? I2.Loc.ScriptLocalization.client_common.word13 :
                        isHx ? "放弃角逐冠军不会影响奖励,您确定要终止演习嘛?":
                        I2.Loc.ScriptLocalization.client_common.word14;

                string yesText = I2.Loc.ScriptLocalization.client_common.word15;
                Action yesCB = HallUtility.GameOver;
                Action noCB = () =>
                {
                    showNotice = false;
                    ShowMenu(false);
                };

                string noText = I2.Loc.ScriptLocalization.client_common.word16;
                
                menuUiAdapter.ShowNoticeWindow(title, yesCB, noCB, yesText, noText);
                showNotice = true;
            }
        }

        private bool showNotice = false;

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);
            ShowMenu(enable);
        }
    }
}    
