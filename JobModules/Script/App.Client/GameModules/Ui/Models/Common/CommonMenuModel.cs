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
using App.Shared.Player;

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
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
            BindEventTrigger();
            InitKeyBinding();
        }
       public override void Update(float interval)
        {
            if (!isVisible || !isGameObjectCreated) return;
            RefreshGui();
        }

        private void InitGui()
        {
            _viewModel.rootActiveSelf = false;
        }

        private void RefreshGui()
        {
           
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
            receiver.AddAction(UserInputKey.OpenMenu, (data) =>
            {
                ShowMenu(!_viewModel.rootActiveSelf);
            });
            menuUiAdapter.RegisterKeyReceive(receiver);
            DynamicKeyBinding();
        }

        void DynamicKeyBinding()
        {
            //保证 菜单界面打开的时候 打不开大地图界面
            pointerReceiver = new PointerReceiver(UiConstant.menuWindowPointBlockLayer, BlockType.All);
            keyReceive = new KeyReceiver(UiConstant.menuWindowKeyBlockLayer, BlockType.All);
            keyReceive.AddAction(UserInputKey.OpenMenu, (data) =>
            {
                ShowMenu(!_viewModel.rootActiveSelf);
            });
        }

        public void ShowMenu(bool visible)
        {
            if (visible && !_viewModel.rootActiveSelf)
            {
                PlayerStateUtil.AddUIState(EPlayerUIState.ExitOpen, menuUiAdapter.gamePlay);
                menuUiAdapter.SetCrossVisible(false);
                _viewModel.rootActiveSelf = true;
                menuUiAdapter.RegisterKeyReceive(keyReceive);
                menuUiAdapter.RegisterPointerReceive(pointerReceiver);
            }
            else if (!visible && _viewModel.rootActiveSelf)
            {
                PlayerStateUtil.RemoveUIState(EPlayerUIState.ExitOpen, menuUiAdapter.gamePlay);
                menuUiAdapter.SetCrossVisible(true);
                _viewModel.rootActiveSelf = false;
                menuUiAdapter.UnRegisterKeyReceive(keyReceive);
                menuUiAdapter.UnRegisterPointerReceive(pointerReceiver);
            }
        }

        void OnBackGameETClick(BaseEventData data)
        {
            menuUiAdapter.SetCrossVisible(true);
            _viewModel.rootActiveSelf = false;
            menuUiAdapter.UnRegisterKeyReceive(keyReceive);
            menuUiAdapter.UnRegisterPointerReceive(pointerReceiver);

            menuUiAdapter.SetCrossVisible(true);
        }

        void OnSettingETClick(BaseEventData data)
        {
            _viewModel.rootActiveSelf = false;
            menuUiAdapter.UnRegisterKeyReceive(keyReceive);
            menuUiAdapter.UnRegisterPointerReceive(pointerReceiver);

            Action<bool> hideCallback = (active) =>
            {
                menuUiAdapter.SetInputManagerEnable(true);
                menuUiAdapter.SetCrossVisible(true);                   
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
            _viewModel.rootActiveSelf = false;
            menuUiAdapter.UnRegisterKeyReceive(keyReceive);
            menuUiAdapter.UnRegisterPointerReceive(pointerReceiver);
            if(menuUiAdapter != null)
            {
                string title = "";
                bool isOverGame = false;
                if (menuUiAdapter.PlayStaue == (int)EPlayerLifeState.Dead || menuUiAdapter.WarVictory)   //玩家死亡或者玩家胜利
                    isOverGame = true;

                if (isOverGame)        //战斗结束
                {
                    title = I2.Loc.ScriptLocalization.client_common.word13;
                }
                else   //战斗未结束
                {
                    title = I2.Loc.ScriptLocalization.client_common.word14;
                }

                string yesText = I2.Loc.ScriptLocalization.client_common.word15;
                Action yesCB = ()=>
                {
                    HallUtility.GameOver();
                };

                string noText = I2.Loc.ScriptLocalization.client_common.word16;
                
                //menuUiAdapter.NoticeUIAdatper.ShowYesNoWindow(title, yesCB, null, yesText, noText);
                menuUiAdapter.ShowNoticeWindow(title, yesCB, null, yesText, noText);
            }
        }
    }
}    
