﻿using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.SettingManager;
using XmlConfig;
using ArtPlugins;


namespace App.Client.GameModules.Ui.Models.Common
{

    enum EControlType
    {
        CheckBox = 1,
        Combox,
        ProgressBar
    }
    public class CommonVideoSettingModel : ClientAbstractModel, IUiSystem
    {
        public CommonVideoSettingViewModel _viewModel = new CommonVideoSettingViewModel();
        private Transform _panelModel, _textModel, _comboxModel, _parentRoot, _toggleModel, _sliderModel;
        private Dictionary<int, float> _sendValList = new Dictionary<int, float>();
        private Dictionary<int, object> _uiControlDict = new Dictionary<int, object>();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        IVideoSettingUiAdapter _adapter;
        private PointerKeyHandler _pointerKeyHandler;
        private KeyHandler _keyHandler;
        private bool _haveRegister;

        public CommonVideoSettingModel(IVideoSettingUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitEventTriggers();
            InitGui();
            IniTKeyHandler();

        }

        private void IniTKeyHandler()
        {
            _pointerKeyHandler = new PointerKeyHandler(Layer.Env, BlockType.All);
            _keyHandler = new KeyHandler(Layer.Env, BlockType.All);
        }

        private void InitGui()
        {

            InitDefaultDict(ref _sendValList);
            InitOption();
        }

        private void InitDefaultDict(ref Dictionary<int, float> defaultVideoSettingList)
        {
            defaultVideoSettingList =
                defaultVideoSettingList = VideoSettingConfigManager.GetInstance().GetDefaultValueDict();
            var origDict = VideoSettingManager.GetInstance().LoadLocalVideoSetting();
            if (origDict == null || origDict.Count == 0) return;
            foreach (var it in origDict)
            {
                defaultVideoSettingList[it.Key] = origDict[it.Key];
            }
        }

        private void InitOption()
        {
            var typeDict = VideoSettingConfigManager.GetInstance().GetTypeDict();
            var enumList = Enum.GetValues(typeof(EVideoSettingType));
            foreach (EVideoSettingType key in enumList)
            {
                var typeListForThisKey = typeDict[key];
                InitOption(key, typeListForThisKey);
            }
        }

        private void InitOption(EVideoSettingType key, HashSet<VideoSetting> typeListForThisKey)
        {
            var textModel = GameObject.Instantiate(_textModel, _parentRoot);
            var title = VideoSettingManager.GetInstance().GetTypeNameByType(key);
            textModel.gameObject.SetActive(true);
            textModel.Find("Text").GetComponent<Text>().text = title;
            foreach (var config in typeListForThisKey)
            {
                InitControl(config);
            }
        }

        private void InitControl(VideoSetting config)
        {
            if (!_sendValList.ContainsKey(config.Id)) return;
            switch ((EControlType)config.ControlType)
            {
                case EControlType.CheckBox: InitCheckBox(config); break;
                case EControlType.Combox: InitCombox(config); break;
                case EControlType.ProgressBar: InitProgressBar(config); break;
            }
        }

        Transform GetNewControlWithTitle(Transform child, string title)
        {
            var childModel = GameObject.Instantiate(child, _parentRoot);
            childModel.gameObject.SetActive(true);
            childModel.Find("Text").GetComponent<Text>().text = title;
            return childModel;
        }

        private void InitProgressBar(VideoSetting config)
        {
            var sliderModel = GetNewControlWithTitle(_sliderModel, config.Description);
            var slider = sliderModel.Find("Slider").GetComponent<Slider>();
            float defaultVal = _sendValList[config.Id];
            sliderModel.Find("curText").GetComponent<Text>().text =
                defaultVal.ToString();
            slider.value = (defaultVal - config.MinValue) / (config.MaxValue - config.MinValue);
            slider.onValueChanged.AddListener((float val) =>
            {
                float curVal = config.MinValue + val * (config.MaxValue - config.MinValue);
                sliderModel.Find("curText").GetComponent<Text>().text =
                    curVal.ToString();
                _sendValList[config.Id] = curVal;
            });
            if (_uiControlDict.ContainsKey(config.Id))
            {
                Debug.LogError(config.Id + " is already in _uiControlDict");
                _uiControlDict[config.Id] = slider;
            }
            else
            {
                _uiControlDict.Add(config.Id, slider);
            }
        }

        private void InitCombox(VideoSetting config)
        {
            var comboxModel = GetNewControlWithTitle(_comboxModel, config.Description);
            var uiCombox = new UICombox(comboxModel);
            var cComboxDic = new Dictionary<string, object>();
            int selectedIndex = -1;
            for (int i = 0; i < config.LevelDatas.Count; i++)
            {
                cComboxDic[config.LevelNames[i]] = config.Id + ":" + config.LevelDatas[i];
                float targetVal;
                if(_sendValList.TryGetValue(config.Id,out targetVal))
                if (Math.Abs(config.LevelDatas[i] - targetVal) < 0.0001f)
                {
                    selectedIndex = i;
                }
            }
            uiCombox.RegisteComBox(cComboxDic);
            uiCombox.itemSelect = ComBoxItemClick;
            uiCombox.SetSelectByIndex(selectedIndex);
            //这里会报错，字典里已存在这个字段，看上去像是重复加载，为了进入游戏先临时修复 by wzq
            if(_uiControlDict.ContainsKey(config.Id))
            {
                Debug.LogError(config.Id + " is already in _uiControlDict");
                _uiControlDict[config.Id] = uiCombox;
            }
            else
            {
                _uiControlDict.Add(config.Id, uiCombox);
            }
            
        }

        private void InitCheckBox(VideoSetting config)
        {
            var toggleModel = GetNewControlWithTitle(_toggleModel, config.Description);
            var toggle = toggleModel.GetComponent<Toggle>();
            toggle.isOn = (int)_sendValList[config.Id] != 0;
            toggle.onValueChanged.AddListener((bool isOn) => { _sendValList[config.Id] = isOn ? 1 : 0; });
            if (_uiControlDict.ContainsKey(config.Id))
            {
                Debug.LogError(config.Id + " is already in _uiControlDict");
                _uiControlDict[config.Id] = toggle;
            }
            else
            {
                _uiControlDict.Add(config.Id, toggle);
            }
        }

        private void ComBoxItemClick(object obj)
        {
            int key;
            float value;
            ParseSendVal(obj.ToString(), out key, out value);
        }

        private void ParseSendVal(string str, out int key, out float value)
        {
            var array = str.Split(':');
            key = int.Parse(array[0]);
            value = float.Parse(array[1]);
            _sendValList[key] = value;
        }

        private void InitEventTriggers()
        {
            _viewModel.applyBtnClick = OnApplyBtnClick;
            _viewModel.CloseBtnClick = OnCloseBtnClick;
            _viewModel.initBtnClick = OnInitBtnClick;
            _viewModel.cancelBtnClick = OnCloseBtnClick;
        }

        private void OnInitBtnClick()
        {
            foreach (int id in Enum.GetValues(typeof(EVideoSettingId)))
            {
                object control;
                if (_uiControlDict.TryGetValue(id, out control))
                {
                    InitControlValue(id, control);
                }
            }
        }

        private void InitControlValue(int id, object control, bool isDefault = true)
        {

            if (control is UICombox)
            {
                InitCombox(id, control as UICombox,isDefault);
            }
            else if (control is Toggle)
            {
                InitToggle(id, control as Toggle, isDefault);
            }
            else if (control is Slider)
            {
                InitSlider(id, control as Slider, isDefault);
            }
        }

        private void InitSlider(int id, Slider control,bool isDefault = true)
        {
            if (!_sendValList.ContainsKey(id) && !isDefault) return;

            var config = VideoSettingConfigManager.GetInstance().GetItemById(id);
            float curValue = isDefault
                ? config.DefaultValue
                : _sendValList[id];
            control.value = (curValue - config.MinValue) / (config.MaxValue - config.MinValue);
        }

        private void InitToggle(int id, Toggle control, bool isDefault = true)
        {
            if (!_sendValList.ContainsKey(id) && !isDefault) return;

            var config = VideoSettingConfigManager.GetInstance().GetItemById(id);
            float curValue = isDefault
                ? config.DefaultValue
                : _sendValList[id];
            bool defaultIndex = curValue == 1;
            control.isOn = defaultIndex;
        }

        private void InitCombox(int id, UICombox control, bool isDefault = true)
        {
            if (!_sendValList.ContainsKey(id) && !isDefault) return;

            var config = VideoSettingConfigManager.GetInstance().GetItemById(id);
            float curValue = isDefault
                ? config.DefaultValue
                : _sendValList[id];
            var defaultList = config.LevelDatas;
            int defaultIndex = defaultList.FindIndex((val) => Math.Abs(val - curValue) < 0.0000001);
            control.SetSelectByIndex(defaultIndex);
        }

        private void OnCloseBtnClick()
        {
            _adapter.Enable = false;
        }

        private void OnApplyBtnClick()
        {
            SendVideoSetting(_sendValList);
        }

        private void SendVideoSetting(Dictionary<int, float> sendValList)
        {
            VideoSettingManager.GetInstance().FlushVideoSettingData(sendValList);
        }

        private void InitVariable()
        {
            _panelModel = FindChildGo("panelModel");
            _textModel = FindChildGo("TextModel");
            _comboxModel = FindChildGo("ComboxModel");
            _parentRoot = FindChildGo("Root");
            _toggleModel = FindChildGo("ToggleModel");
            _sliderModel = FindChildGo("SliderModel");
            _panelModel.gameObject.SetActive(true);
            _textModel.gameObject.SetActive(false);
            _comboxModel.gameObject.SetActive(false);
            _toggleModel.gameObject.SetActive(false);
            _sliderModel.gameObject.SetActive(false);
        }


        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);

            _adapter.Enable = enable;
            if (enable)
            {
                InitDefaultDict(ref _sendValList);
                ResetGui();
            }
            if (enable && !_haveRegister)
            {
                RegisterKeyhandler();
            }
            else if (!enable && _haveRegister)
            {
                UnRegisterKeyhandler();
            }
        }

        private void ResetGui()
        {
            foreach (int id in Enum.GetValues(typeof(EVideoSettingId)))
            {
                object control;
                if (_uiControlDict.TryGetValue(id, out control))
                {
                    InitControlValue(id, control,false);
                }
            }
        }

        private void UnRegisterKeyhandler()
        {
            _adapter.SetCrossVisible(true);
            _adapter.UnRegisterPointerReceive(_pointerKeyHandler);
            _adapter.UnRegisterKeyReceive(_keyHandler);
            _haveRegister = false;
        }

        private void RegisterKeyhandler()
        {
            _adapter.SetCrossVisible(false);
            _adapter.RegisterPointerReceive(_pointerKeyHandler);
            _adapter.RegisterKeyReceive(_keyHandler);
            _haveRegister = true;
        }
    }
}
