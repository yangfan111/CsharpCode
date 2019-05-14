using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.ViewModels.Chicken;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Chicken
{
    public class ChickenBagItem: UIItem
    {
        private ChickenBagItemViewModel _viewModel = new ChickenBagItemViewModel();
        protected override void SetView()
        {
            base.SetView();
            if (Data is IChickenBagItemUiData)
            {
                SetView(Data as IChickenBagItemUiData);
            }
            else if (Data is IBaseChickenBagItemData)
            {
                SetView(Data as IBaseChickenBagItemData);
            }

        }

        public override void Init()
        {
            base.Init();
            InitEvent();
            InitVariable();
        }

        private void InitVariable()
        {
            if (root == null)
            {
                root = FindChildGo(_viewModel.ResourceAssetName);
            }
        }

        UIEventTriggerListener eventTrigger;

        UIDrag uiDrog;
        public Action<IBaseChickenBagItemData> DragCallback;
        public Action<Transform,IBaseChickenBagItemData> EnterCallback;

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

            eventTrigger = FindComponent<UIEventTriggerListener>("ItemGroup");
            if (eventTrigger != null)
            {
                eventTrigger.onClick = RightClickAction;
                eventTrigger.onEnter = EnterAction;
            }
            uiDrog = FindComponent<UIDrag>("ItemGroup");
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

                EnterCallback.Invoke(root,Data as IBaseChickenBagItemData);
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
        private Transform root;

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
            }
            else
            {
                SetView(realData as IBaseChickenBagItemData);            
            }
        }

        private void SetView(IBaseChickenBagItemData realData)
        {
            _viewModel.ItemGroupShow = true;
            _viewModel.TitleGroupShow = false;
            _viewModel.CountText = realData.count.ToString();
            var config = SingletonManager.Get<ItemBaseConfigManager>().GetConfigById(realData.cat, realData.id,true);
            if (config == null) return;
            _viewModel.ItemIconBundle = config.IconBundle;
            _viewModel.ItemIconAsset = config.Icon;
            _viewModel.ItemNameText = config.Name;
        }

        protected override IUiViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

    }
}
