using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.EventSystems;
using App.Client.GameModules.Ui.UiAdapter;
using UnityEngine;
using UserInputManager.Lib;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using App.Shared.Components.Player;
using Core.Free;
using UIComponent.UI;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonSplitModel : ClientAbstractModel, IUiSystem
    {
        private ISplitUiAdapter splitUIAdapter = null;
        private bool isGameObjectCreated = false;        
        private KeyHandler keyReveiver = null;

        //记录正在拆分的道具 信息
        private int maxValue = 0;
        private int minValue = 1;
        private long categoryId = -1;
        private long propId = -1;
        private string propName = "";
        private string itemKey = "";
        
        //界面变量
        private UIInputField inputFieldCom = null;
        private int lastInputValue = 0;
        private Slider sliderCom = null;
        private float lastSliderValue = 0;

        private CommonSplitViewModel _viewModel = new CommonSplitViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonSplitModel(ISplitUiAdapter splitUIAdapter):base(splitUIAdapter)
        {
            this.splitUIAdapter = splitUIAdapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitGui();
            BindEventTrigger();
            InitKeyBinding();
        }
        private void InitGui()
        {
            inputFieldCom = FindComponent<UIInputField>("InputField");
            sliderCom = FindComponent<Slider>("Slider");
            _viewModel.rootActiveSelf = false;
            _viewModel.leftText = minValue.ToString();
            //inputFieldCom.onValidateFailed = () => { splitUIAdapter.ShowIllegalTip(); };
        }
        public override void Update(float interval)
        {
            if (splitUIAdapter.Info != null && _viewModel.rootActiveSelf == false)
            {
                ShowSplitWindow();
            }
            else if(splitUIAdapter.Info == null && _viewModel.rootActiveSelf)
            {
                _viewModel.rootActiveSelf = false;
                splitUIAdapter.UnRegisterKeyReceive(keyReveiver);
            }

            if (_viewModel.rootActiveSelf == true)
            {
                //实时监测滑动条的值
                var temperValue = sliderCom.value;
                if (Math.Abs(temperValue - lastSliderValue) > 0.000001f)
                {
                    temperValue = (float)Mathf.Clamp(temperValue, 0, 1);
                    sliderCom.value = temperValue;
                    lastSliderValue = temperValue;

                    //刷新输入框
                    var result = (int)(lastSliderValue * (maxValue - minValue) + minValue);
                    result = (int)Mathf.Clamp(result, minValue, maxValue);
                    inputFieldCom.text = result.ToString();
                    lastInputValue = result;
                    return;
                }

                //实时监测输入框的值
                var inputTemperStr = inputFieldCom.text;
                int inputTemperValue = -1;
                if (int.TryParse(inputTemperStr, out inputTemperValue))
                {
                    if (inputTemperValue != lastInputValue)
                    {
                        lastInputValue = inputTemperValue;
                        inputFieldCom.text = inputTemperValue.ToString();
                        if (inputTemperValue > maxValue || inputTemperValue < minValue)
                        {
                            splitUIAdapter.ShowIllegalTip();
                            _viewModel.splitBtnInteractable = false;
                        }
                        else
                        {
                            _viewModel.splitBtnInteractable = true;
                        }
                        inputTemperValue = (int)Mathf.Clamp(inputTemperValue, minValue, maxValue);
                        float sliderValue = (float) (inputTemperValue - minValue) / (float) (maxValue - minValue + 1);
                        sliderCom.value = sliderValue;                //刷新进度条
                        lastSliderValue = sliderValue;
                        
                        return;
                    }
                }
                else
                {
                    lastInputValue = 0;
                    lastSliderValue = 0;
                    sliderCom.value = 0;
                    _viewModel.splitBtnInteractable = false;
                }
            }
        }

        private void BindEventTrigger()
        {
            _viewModel.splitBtnClick = OnSplitBtnClick;
            _viewModel.cancelBtnClick = OnCancelBtnClick;
            _viewModel.splitNumChanged = HandleInput;
        }

        private void HandleInput(string str)
        {
            string input = str;
            string pattern = "\\r+|\\n+|-+";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string res = rgx.Replace(input, replacement);
            inputFieldCom.text = res;
        }

        private void InitKeyBinding()
        {
            keyReveiver = new KeyHandler(UiConstant.splitWindowKeyBlockLayer, BlockType.All);
            keyReveiver.BindKeyAction(UserInputKey.SplitProp, (data) =>
            {
                if (!_viewModel.splitBtnInteractable)
                {
                    return;
                }
                SendMessage();
                HideWindow();
            });
            keyReveiver.BindKeyAction(UserInputKey.HideWindow, (data) => { HideWindow(); });
        }
        private void OnSplitBtnClick()
        {
            SendMessage();
            HideWindow();
        }
        private void OnCancelBtnClick()
        {
            HideWindow();
        }
        private void HideWindow()
        {
            splitUIAdapter.Info = null;
        }
        private void SendMessage()
        {
            if (lastInputValue >= minValue && lastInputValue <= maxValue)
                splitUIAdapter.SendSplitMessage((int)lastInputValue, itemKey);
        }

        private void ShowSplitWindow()
        {
            _viewModel.rootActiveSelf = true;
            inputFieldCom.ActivateInputField();
            int num = splitUIAdapter.Info.Num;
            int halfNum = num / 2;
            //刷新titiel
            _viewModel.titleText = I2.Loc.ScriptLocalization.client_common.word19 + splitUIAdapter.Info.propName;

            ////刷新左值
            //_viewModel.leftText = (num / 2).ToString();

            //刷新右值
            _viewModel.rightText = num.ToString();

            //刷新输入框
            inputFieldCom.text = (halfNum).ToString();

            //刷新进度条
            sliderCom.value = (float)(halfNum) / num;

            //记录当前拆分的道具
            //this.minValue = 1;
            this.maxValue = splitUIAdapter.Info.Num;
            this.categoryId = splitUIAdapter.Info.categoryId;
            this.propId = splitUIAdapter.Info.propId;
            this.propName = splitUIAdapter.Info.propName;
            this.itemKey = splitUIAdapter.Info.key;
            lastInputValue = halfNum;
            lastSliderValue = sliderCom.value;

            _viewModel.splitBtnInteractable = true;
            splitUIAdapter.RegisterKeyReceive(keyReveiver);
        }
    }
}    
