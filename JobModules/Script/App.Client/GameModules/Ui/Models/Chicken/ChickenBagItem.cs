using System;
using App.Client.GameModules.Ui.ViewModels.Chicken;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.Utils;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Chicken
{
    public class ChickenBagItem: UIItem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenBagItem));

        private ChickenBagItemViewModel _viewModel = new ChickenBagItemViewModel();
        protected override void SetView()
        {
            base.SetView();
            ClearTip();
            InitEvent();
            if (Data is IChickenBagItemUiData)
            {
                SetView(Data as IChickenBagItemUiData);
            }
            else if (Data is IBaseChickenBagItemData)
            {
                SetView(Data as IBaseChickenBagItemData);
            }
        }



        private UIEventTriggerListener eventTrigger;

        UIDrag uiDrog;
        public Action<IBaseChickenBagItemData> DragCallback;
        public Action<ChickenBagItem> EnterCallback;

        private void InitEvent()
        {
            if (Data is IChickenBagItemUiData)//背包标题不需要
            {
                var data = Data as IChickenBagItemUiData;
                if (data.isBagTitle)
                {
                    return;
                }
            }

            EnterAction(null, null);
            eventTrigger = FindComponent<UIEventTriggerListener>(_viewModel.ResourceAssetName);
            if (eventTrigger != null)
            {
                eventTrigger.onClick = RightClickAction;
            }

            if (eventTrigger.isEnter && eventTrigger.onEnter != null)
            {
                eventTrigger.onEnter.Invoke(null, null);
            }
            uiDrog = FindComponent<UIDrag>(_viewModel.ResourceAssetName);
            if (uiDrog != null)
            {
                uiDrog.DragItem = FindChildGo("ItemIcon").gameObject;
                uiDrog.OnEndDragCallback = DragCallbackAction;
            }
        }

        private void EnterAction(UIEventTriggerListener arg1, PointerEventData arg2)
        {
            if (EnterCallback != null)
            {
                if (Data is IChickenBagItemUiData)
                {
                    var data = Data as IChickenBagItemUiData;
                    if (data.isBagTitle)
                    {
                        return;
                    }
                }

                EnterCallback.Invoke(this);
            }
        }

        private void DragCallbackAction(Vector2 obj)
        {
            if (DragCallback != null)
            {
                DragCallback.Invoke(Data as IBaseChickenBagItemData);
            }   
        }

        public Action<IBaseChickenBagItemData> RightClickCallback;

        public Transform GetRoot()
        {
            return ViewInstance.transform;
        }

        private void RightClickAction(UIEventTriggerListener arg1, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && RightClickCallback != null)
            {
                if (Data is IChickenBagItemUiData)
                {
                    var data = Data as IChickenBagItemUiData;
                    if (data.isBagTitle)
                    {
                        return;
                    }
                }
                RightClickCallback(Data as IBaseChickenBagItemData);
            }
        }

        private void SetView(IChickenBagItemUiData realData)
        {
            if (realData.isBagTitle)
            {
                _viewModel.TitleGroupShow = true;
                _viewModel.TitleText = realData.title;
                _viewModel.ItemGroupShow = false;
                ShowView(true);
            }
            else
            {
                SetView(realData as IBaseChickenBagItemData);            
            }
        }

        private void ShowView(bool show)
        {
            ViewInstance.GetComponent<CanvasGroup>().alpha = show ? 1 : 0;
        }


        private void SetView(IBaseChickenBagItemData realData)
        {
            _viewModel.ItemGroupShow = true;
            _viewModel.TitleGroupShow = false;
            _viewModel.CountText = realData.count.ToString();
            _viewModel.CountShow = realData.count > 1;
           
            var config = SingletonManager.Get<ItemBaseConfigManager>().GetConfigById(realData.cat, realData.id,true);
            if (config == null)
            {
                Logger.ErrorFormat("error config cat:{0} id:{1}", realData.cat, realData.id);
                ShowView(false);
                return;
            }
            _viewModel.ItemIconBundle = config.IconBundle;
            _viewModel.ItemIconAsset = config.Icon;
            _viewModel.ItemNameText = config.Name;
            ShowView(true);
        }

        protected override IUiViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        protected override void OnAddToPool()
        {
            base.OnAddToPool();
            CloseUIDrag();
            if (eventTrigger != null)
            {
                eventTrigger.onClick = null;
                eventTrigger.onEnter = null;
                eventTrigger.onExit = null;
            }

            ClearTip();
            
        }

        private void ClearTip()
        {
            var root = GetRoot();
            if (root != null)
                UiCommon.TipManager.UnRegisterTip(root);
        }

        public void CloseUIDrag()
        {
            if (uiDrog != null)
            {
                uiDrog.DestroyCopy();
            }
        }
    }
}
