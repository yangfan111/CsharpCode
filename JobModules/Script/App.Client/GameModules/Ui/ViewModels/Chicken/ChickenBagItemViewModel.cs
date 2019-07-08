using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Assets.UiFramework.Libs;
using UnityEngine.UI;
using UIComponent.UI;

namespace App.Client.GameModules.Ui.ViewModels.Chicken
{
    public class ChickenBagItemViewModel : ViewModelBase, IUiViewModel
    {
        private class ChickenBagItemView : UIView
        {
            public GameObject TitleGroupShow;
            [HideInInspector]
            public bool oriTitleGroupShow;
            public GameObject ItemGroupShow;
            [HideInInspector]
            public bool oriItemGroupShow;
            public Text TitleText;
            [HideInInspector]
            public string oriTitleText;
            public Text CountText;
            [HideInInspector]
            public string oriCountText;
            public GameObject CountShow;
            [HideInInspector]
            public bool oriCountShow;
            public UIImageLoader ItemIconBundle;
            [HideInInspector]
            public string oriItemIconBundle;
            public UIImageLoader ItemIconAsset;
            [HideInInspector]
            public string oriItemIconAsset;
            public Text ItemNameText;
            [HideInInspector]
            public string oriItemNameText;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "TitleGroup":
                            TitleGroupShow = v.gameObject;
                            break;
                        case "ItemGroup":
                            ItemGroupShow = v.gameObject;
                            break;
                        case "Count":
                            CountShow = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Title":
                            TitleText = v;
                            break;
                        case "Count":
                            CountText = v;
                            break;
                        case "ItemName":
                            ItemNameText = v;
                            break;
                    }
                }

                UIImageLoader[] uiimageloaders = gameObject.GetComponentsInChildren<UIImageLoader>(true);
                foreach (var v in uiimageloaders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ItemIcon":
                            ItemIconBundle = v;
                            ItemIconAsset = v;
                            break;
                    }
                }

            }
        }


        private bool _titleGroupShow;
        private bool _itemGroupShow;
        private string _titleText;
        private string _countText;
        private bool _countShow;
        private string _itemIconBundle;
        private string _itemIconAsset;
        private string _itemNameText;
        public bool TitleGroupShow { get { return _titleGroupShow; } set {if(_titleGroupShow != value) Set(ref _titleGroupShow, value, "TitleGroupShow"); } }
        public bool ItemGroupShow { get { return _itemGroupShow; } set {if(_itemGroupShow != value) Set(ref _itemGroupShow, value, "ItemGroupShow"); } }
        public string TitleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "TitleText"); } }
        public string CountText { get { return _countText; } set {if(_countText != value) Set(ref _countText, value, "CountText"); } }
        public bool CountShow { get { return _countShow; } set {if(_countShow != value) Set(ref _countShow, value, "CountShow"); } }
        public string ItemIconBundle { get { return _itemIconBundle; } set {if(_itemIconBundle != value) Set(ref _itemIconBundle, value, "ItemIconBundle"); } }
        public string ItemIconAsset { get { return _itemIconAsset; } set {if(_itemIconAsset != value) Set(ref _itemIconAsset, value, "ItemIconAsset"); } }
        public string ItemNameText { get { return _itemNameText; } set {if(_itemNameText != value) Set(ref _itemNameText, value, "ItemNameText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private ChickenBagItemView _view;
		
		public void Destory()
        {
            if (_viewGameObject != null)
            {
				UnityEngine.Object.Destroy(_viewGameObject);
            }
        }
		public void Visible(bool isViaible)
		{
		    if (_viewGameObject != null)
            {
				_viewGameObject.SetActive(isViaible);
            }
		
		}
		public void SetCanvasEnabled(bool value)
        {
            if (_viewCanvas != null)
            {
                _viewCanvas.enabled = value;
            }
        }
        public void CreateBinding(GameObject obj)
        {
			_viewGameObject = obj;
			_viewCanvas = _viewGameObject.GetComponent<Canvas>();

			bool bFirst = false;
			var view = obj.GetComponent<ChickenBagItemView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<ChickenBagItemView>();
				view.FillField();
			}
			DataInit(view);
			SpriteReset();
			view.BindingContext().DataContext = this;
			if(bFirst)
			{
				SaveOriData(view);
				ViewBind(view);
			}
			_view = view;

        }
		private void EventTriggerBind(ChickenBagItemView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static ChickenBagItemViewModel()
        {
            Type type = typeof(ChickenBagItemViewModel);
            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    PropertySetter.Add(property.Name, property);
                }
            }
			foreach (var methodInfo in type.GetMethods())
            {
                if (methodInfo.IsPublic)
                {
                    MethodSetter.Add(methodInfo.Name, methodInfo);
                }
            }
        }

		void ViewBind(ChickenBagItemView view)
		{
		     BindingSet<ChickenBagItemView, ChickenBagItemViewModel> bindingSet =
                view.CreateBindingSet<ChickenBagItemView, ChickenBagItemViewModel>();
            bindingSet.Bind(view.TitleGroupShow).For(v => v.activeSelf).To(vm => vm.TitleGroupShow).OneWay();
            bindingSet.Bind(view.ItemGroupShow).For(v => v.activeSelf).To(vm => vm.ItemGroupShow).OneWay();
            bindingSet.Bind(view.TitleText).For(v => v.text).To(vm => vm.TitleText).OneWay();
            bindingSet.Bind(view.CountText).For(v => v.text).To(vm => vm.CountText).OneWay();
            bindingSet.Bind(view.CountShow).For(v => v.activeSelf).To(vm => vm.CountShow).OneWay();
            bindingSet.Bind(view.ItemIconBundle).For(v => v.BundleName).To(vm => vm.ItemIconBundle).OneWay();
            bindingSet.Bind(view.ItemIconAsset).For(v => v.AssetName).To(vm => vm.ItemIconAsset).OneWay();
            bindingSet.Bind(view.ItemNameText).For(v => v.text).To(vm => vm.ItemNameText).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(ChickenBagItemView view)
		{
            _titleGroupShow = view.TitleGroupShow.activeSelf;
            _itemGroupShow = view.ItemGroupShow.activeSelf;
            _titleText = view.TitleText.text;
            _countText = view.CountText.text;
            _countShow = view.CountShow.activeSelf;
            _itemIconBundle = view.ItemIconBundle.BundleName;
            _itemIconAsset = view.ItemIconAsset.AssetName;
            _itemNameText = view.ItemNameText.text;
		}


		void SaveOriData(ChickenBagItemView view)
		{
            view.oriTitleGroupShow = _titleGroupShow;
            view.oriItemGroupShow = _itemGroupShow;
            view.oriTitleText = _titleText;
            view.oriCountText = _countText;
            view.oriCountShow = _countShow;
            view.oriItemIconBundle = _itemIconBundle;
            view.oriItemIconAsset = _itemIconAsset;
            view.oriItemNameText = _itemNameText;
		}




		private void SpriteReset()
		{
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			TitleGroupShow = _view.oriTitleGroupShow;
			ItemGroupShow = _view.oriItemGroupShow;
			TitleText = _view.oriTitleText;
			CountText = _view.oriCountText;
			CountShow = _view.oriCountShow;
			ItemIconBundle = _view.oriItemIconBundle;
			ItemIconAsset = _view.oriItemIconAsset;
			ItemNameText = _view.oriItemNameText;
			SpriteReset();
		}

		public void CallFunction(string functionName)
        {
            if (MethodSetter.ContainsKey(functionName))
            {
                MethodSetter[functionName].Invoke(this, null);
            }
        }

		public bool IsPropertyExist(string propertyId)
        {
            return PropertySetter.ContainsKey(propertyId);
        }

		public Transform GetParentLinkNode()
		{
			return null;
		}

		public Transform GetChildLinkNode()
		{
			return null;
		}

        public string ResourceBundleName { get { return "ui/client/prefab/chicken"; } }
        public string ResourceAssetName { get { return "ChickenBagItem"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
