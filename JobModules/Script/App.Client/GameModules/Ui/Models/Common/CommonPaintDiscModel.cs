using System;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;
using Utils.Configuration;
using XmlConfig;
using App.Shared.GameModules.Player;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonPaintDiscModel :  ClientAbstractModel, IUiSystem
    {
        private CommonRoleDiskViewModel view = new CommonRoleDiskViewModel();
        IPaintUiAdapter _adapter;
        private KeyHandler _openKeyHandler;

        public CommonPaintDiscModel(IPaintUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }


        protected override IUiViewModel ViewModel
        {
            get
            {
                return view;
            }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            Init();
            Refresh();
            InitKey();
        }

        private void InitKey()
        {
            _openKeyHandler = new KeyHandler(UiConstant.paintWindowLayer, BlockType.None);
            _openKeyHandler.BindKeyAction(UserInputKey.F1, (data) => { _adapter.Enable = true;});
            _adapter.RegisterOpenKey(_openKeyHandler);

            keyReveiver = new KeyHandler(UiConstant.paintWindowKeyBlockLayer, BlockType.All);
            keyReveiver.BindKeyAction(UserInputKey.F1, (data) => { _adapter.Enable = false; });
            keyReveiver.BindKeyAction(UserInputKey.HideWindow, (data) => { _adapter.Enable = false; });
            _pointerKeyHandler = new PointerKeyHandler(UiConstant.paintWindowKeyBlockLayer, BlockType.All);
        }


        Transform btnRoot;
        UITab tab;
        private bool _haveRegister;
        private KeyHandler keyReveiver;
        private PointerKeyHandler _pointerKeyHandler;

        void Init()
        {
            tab = FindComponent<UITab>("Group");
            tab.tabClickAction = OnDiskSetAndPlay;
            tab.tabHoverAction = OnDiskSet;
            btnRoot = tab.transform;
        }

        private void OnDiskSet(UIEventTriggerListener listerner, int index)
        {
            _adapter.SelectedPaintIndex = index;
        }

        void OnDiskSetAndPlay(UIEventTriggerListener listerner, int index)
        {
            _adapter.SelectedPaintIndex = index;
            _adapter.Paint();
            _adapter.Enable = false;
        }

        void Refresh()
        {
            for (int i = 0; i < btnRoot.childCount; i++)
            {
                Transform tf = btnRoot.GetChild(i);
                Transform iconMax = tf.Find("Content/IconMax");
                Transform empty = tf.Find("Content/Empty");
                var configItem = GetIndividuationByIndex(i);
                if (configItem == null)
                {
                    empty.gameObject.SetActive(true);
                    iconMax.gameObject.SetActive(false);
                    var eventTriggerListener = tf.GetComponent<UIEventTriggerListener>();
                    if (eventTriggerListener != null)
                    {
                        eventTriggerListener.enabled = false;
                    }
                }
                else
                {
                    empty.gameObject.SetActive(false);
                    iconMax.gameObject.SetActive(true);
                    //AssetInfo iconAsset = new AssetInfo(configItem.Icon, configItem.IconBundle);

                    UIImageLoader iconLoader = iconMax.Find("Icon").GetComponent<UIImageLoader>();
     
                    iconLoader.BundleName = configItem.IconBundle;
                    iconLoader.AssetName = configItem.Icon;

                    var nameText = iconMax.Find("Name").GetComponent<Text>();
                    nameText.text = configItem.Name;

                }
            }
        }

        private Individuation GetIndividuationByIndex(int i)
        {
            var list = _adapter.PaintIdList;
            if (list == null || list.Count <= i || list[i] == 0) return null;
            var id = list[i];
            var config = IndividuationConfigManager.GetInstance().GetConfigById(id);
            return config;
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);

            if (enable)
            {
                SetSelect();
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

        private void UnRegisterKeyhandler()
        {
            _adapter.SetCrossVisible(true);
            PlayerStateUtil.RemoveUIState(EPlayerUIState.PaintOpen, _adapter.gamePlay);
            if (keyReveiver == null || _pointerKeyHandler == null)
            {
                return;
            }
            _adapter.UnRegisterKeyReceive(keyReveiver);
            _adapter.UnRegisterPointerReceive(_pointerKeyHandler);

            _haveRegister = false;
        }

        private void RegisterKeyhandler()
        {
            _adapter.SetCrossVisible(false);
            PlayerStateUtil.AddUIState(EPlayerUIState.PaintOpen, _adapter.gamePlay);
            if (keyReveiver == null || _pointerKeyHandler == null)
            {
                return;
            }
            _adapter.RegisterKeyReceive(keyReveiver);
            _adapter.RegisterPointerReceive(_pointerKeyHandler);
            _haveRegister = true;
        }

        private void SetSelect()
        {
            _adapter.Select();
            var index = _adapter.SelectedPaintIndex;
            if (tab != null)
            {
                tab.HighLight(index);
            }
        }
    }
}
