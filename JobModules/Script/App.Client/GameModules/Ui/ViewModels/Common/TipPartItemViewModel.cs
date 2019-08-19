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

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class TipPartItemViewModel : ViewModelBase, IUiViewModel
    {
        private class TipPartItemView : UIView
        {
            public Image PartQualitySprite;
            public Text PartName;
            [HideInInspector]
            public string oriPartName;
            public Text PartNameColor;
            [HideInInspector]
            public Color oriPartNameColor;
            public Text PartsText;
            [HideInInspector]
            public string oriPartsText;
            public Text PartsColor;
            [HideInInspector]
            public Color oriPartsColor;
            
            public void FillField()
            {
                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PartQuality":
                            PartQualitySprite = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PartName":
                            PartName = v;
                            PartNameColor = v;
                            break;
                        case "Parts":
                            PartsText = v;
                            PartsColor = v;
                            break;
                    }
                }

            }
        }


        private Sprite _partQualitySprite;
        private string _partName;
        private Color _partNameColor;
        private string _partsText;
        private Color _partsColor;
        public Sprite PartQualitySprite { get { return _partQualitySprite; } set {if(_partQualitySprite != value) Set(ref _partQualitySprite, value, "PartQualitySprite"); if(null != _view && null != _view.PartQualitySprite && null == value) _view.PartQualitySprite.sprite = ViewModelUtil.EmptySprite; } }
        public string PartName { get { return _partName; } set {if(_partName != value) Set(ref _partName, value, "PartName"); } }
        public Color PartNameColor { get { return _partNameColor; } set {if(_partNameColor != value) Set(ref _partNameColor, value, "PartNameColor"); } }
        public string PartsText { get { return _partsText; } set {if(_partsText != value) Set(ref _partsText, value, "PartsText"); } }
        public Color PartsColor { get { return _partsColor; } set {if(_partsColor != value) Set(ref _partsColor, value, "PartsColor"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private TipPartItemView _view;
		
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
			var view = obj.GetComponent<TipPartItemView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<TipPartItemView>();
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
		private void EventTriggerBind(TipPartItemView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static TipPartItemViewModel()
        {
            Type type = typeof(TipPartItemViewModel);
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

		void ViewBind(TipPartItemView view)
		{
		     BindingSet<TipPartItemView, TipPartItemViewModel> bindingSet =
                view.CreateBindingSet<TipPartItemView, TipPartItemViewModel>();
            bindingSet.Bind(view.PartQualitySprite).For(v => v.sprite).To(vm => vm.PartQualitySprite).OneWay();
            bindingSet.Bind(view.PartName).For(v => v.text).To(vm => vm.PartName).OneWay();
            bindingSet.Bind(view.PartNameColor).For(v => v.color).To(vm => vm.PartNameColor).OneWay();
            bindingSet.Bind(view.PartsText).For(v => v.text).To(vm => vm.PartsText).OneWay();
            bindingSet.Bind(view.PartsColor).For(v => v.color).To(vm => vm.PartsColor).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(TipPartItemView view)
		{
            _partName = view.PartName.text;
            _partNameColor = view.PartNameColor.color;
            _partsText = view.PartsText.text;
            _partsColor = view.PartsColor.color;
		}


		void SaveOriData(TipPartItemView view)
		{
            view.oriPartName = _partName;
            view.oriPartNameColor = _partNameColor;
            view.oriPartsText = _partsText;
            view.oriPartsColor = _partsColor;
		}




		private void SpriteReset()
		{
			PartQualitySprite = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			PartName = _view.oriPartName;
			PartNameColor = _view.oriPartNameColor;
			PartsText = _view.oriPartsText;
			PartsColor = _view.oriPartsColor;
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

        public string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public string ResourceAssetName { get { return "BattleTipPartItem"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
