using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UserInputManager.Lib;
using UnityEngine.EventSystems;
using App.Client.Utility;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonNoticeModel : ClientAbstractModel, IUiSystem
    {
        private INoticeUiAdapter adapter = null;
        private bool isGameObjectCreated = false;
        private const string uiIconsBundleName = "ui/icons";

        private string title = string.Empty;
        private string yesTex = string.Empty;
        private string noText = string.Empty;
        private float countDownTime = 0; 
        private Action yesCallback = null;
        private Action noCallback = null;
        private Action countDownCallback = null;
        private bool isCountDown = false;
        private float temperCountDown = 0;

        NoticeWindowStyle style = NoticeWindowStyle.YESNO;
        KeyReceiver keyReceive = null;
        PointerReceiver pointerReceiver = null;

        private CommonNoticeViewModel _viewModel = new CommonNoticeViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonNoticeModel(INoticeUiAdapter noticeAdapterInfo):base(noticeAdapterInfo)
        {
            this.adapter = noticeAdapterInfo;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
            InitKeyBinding();
            BindEventTrigger();
        }
        public override void Update(float interval)
        {
            if (!isGameObjectCreated) return;
            
            if(adapter != null)
            {
                if (adapter.Style == NoticeWindowStyle.NONE)
                {
                    if (_viewModel.rootActive)
                    {
                        _viewModel.rootActive = false;
//                        UiModule.HideSpecialModelAndShowALL(this.GetType().Name);
//                        adapter.HideUiGroup(Core.Ui.UiGroup.Base);
                        adapter.UnRegisterKeyReceive(keyReceive);
                        adapter.UnRegisterPointerReceive(pointerReceiver);
                        adapter.SetCrossVisible(true);
                    }
                }
                else 
                {
                    if (!_viewModel.rootActive)
                    {
                        _viewModel.rootActive = true;
                        if (adapter.Style == NoticeWindowStyle.COUNTDOWN)
                        {
                            this.ShowCountDownWindow(adapter.InfoItem.Title, adapter.InfoItem.CountDownCallback, adapter.InfoItem.CountDownTime);
                        }
                        if (adapter.Style == NoticeWindowStyle.YESNO)
                        {
                            this.ShowYesNoWindow(adapter.InfoItem.Title, adapter.InfoItem.YesCallback, adapter.InfoItem.NoCallback, adapter.InfoItem.YesText, adapter.InfoItem.NoText);
                        }
                        if (adapter.Style == NoticeWindowStyle.YES)
                        {
                            this.ShowYesWindow(adapter.InfoItem.Title, adapter.InfoItem.YesCallback, adapter.InfoItem.YesText);
                        }
                    }
                }
            }

            CountDown(interval);
        }

        public void InitKeyBinding()
        {
            //屏幕所有操作
            pointerReceiver = new PointerReceiver(UiConstant.noticeWindowPointBlockLayer, BlockType.All);

            //屏蔽所有操作
            keyReceive = new KeyReceiver(UiConstant.noticeWindowKeyBlockLayer, BlockType.All);  
            keyReceive.AddAction(UserInputKey.HideWindow, (data) =>
            {
                HideWindow();
            });
        }

        private void InitGui()
        {
            _viewModel.rootActive = false;
        }

        private void RefreshGui()
        {            
            switch (style)
            {
                case NoticeWindowStyle.YESNO:
                    {
                        _viewModel.YesBtnActive = true;
                        _viewModel.NoBtnActive = true;
                        _viewModel.yNameText = yesTex;
                        _viewModel.nNameText = noText;
                        _viewModel.TitleText = title;
                        isCountDown = false;
                    }
                    break;
                case NoticeWindowStyle.YES:
                    {
                        _viewModel.YesBtnActive = true;
                        _viewModel.NoBtnActive = false;
                        _viewModel.yNameText = yesTex;
                        _viewModel.nNameText = noText;
                        _viewModel.TitleText = title;
                        isCountDown = false;
                    }
                    break;
                case NoticeWindowStyle.COUNTDOWN:
                    {
                        _viewModel.YesBtnActive = false;
                        _viewModel.NoBtnActive = false;
                        _viewModel.TitleText = title;
                        isCountDown = true;
                        temperCountDown = 0;
                    }
                    break;
            }

            _viewModel.rootActive = true;
//            UiModule.ShowSpecialModelAndHideALL(this.GetType().Name);
//            adapter.ShowUiGroup(Core.Ui.UiGroup.Base);
            adapter.RegisterKeyReceive(keyReceive);
            adapter.RegisterPointerReceive(pointerReceiver);

            adapter.SetCrossVisible(false);
        }

        private void BindEventTrigger()
        {
            _viewModel.OnYesBtnClickListener = OnYesBtnClick;
            _viewModel.OnNoBtnClickListener = OnNoBtnClick;
        }

        private void OnYesBtnClick(BaseEventData data)
        {
            this.HideWindow();

            if (yesCallback != null)
                yesCallback();
            
        }

        private void OnNoBtnClick(BaseEventData data)
        {
            this.HideWindow();

            if (noCallback != null)
                noCallback();
        }

        private void CountDown(float interval)
        {
            if(isCountDown)
            {
                temperCountDown += interval;
                if(temperCountDown > countDownTime)
                {
                    _viewModel.rootActive = false;
                    temperCountDown = 0;
                    isCountDown = false;

                    if (countDownCallback != null)
                    {
                        countDownCallback();
                    }
                    HideWindow();
                }
            }
        }


        private void Reset()
        {
            style = NoticeWindowStyle.YESNO;
            title = string.Empty;
            yesCallback = null;
            noCallback = null;
            countDownCallback = null;
            yesTex = string.Empty;
            noText = string.Empty;
            countDownTime = 0;

            adapter.SetCrossVisible(false);
        }

        //---------------打开窗口
        private void ShowYesNoWindow(string title, Action yesCallback, Action noCallback, string yesText, string noText)
        {
            Reset();
            this.style = NoticeWindowStyle.YESNO;
            this.title = title;
            this.yesCallback = yesCallback;
            this.yesTex = yesText;

            this.noCallback = noCallback;
            this.noText = noText;

            this.countDownCallback = null;
            this.countDownTime = 0;

            RefreshGui();
        }

        private void ShowYesWindow(string title, Action yesCallback, string yesText)
        {
            Reset();
            this.style = NoticeWindowStyle.YES;
            this.title = title;
            this.yesCallback = yesCallback;
            this.yesTex = yesText;

            this.noCallback = null;
            this.noText = "";

            this.countDownCallback = null;
            this.countDownTime = 0;

            RefreshGui();
        }

        private void ShowCountDownWindow(string title, Action countDownCallback, float countDownTime)
        {
            Reset();
            this.style = NoticeWindowStyle.COUNTDOWN;
            this.title = title;
            this.yesCallback = null;
            this.yesTex = "";

            this.noCallback = null;
            this.noText = "";

            this.countDownCallback = countDownCallback;
            this.countDownTime = countDownTime;

            RefreshGui();
        }

        private void HideWindow()
        {
            if (adapter != null)
                adapter.Style = NoticeWindowStyle.NONE;
        }
    }
}    
