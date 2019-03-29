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

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonCrossHairViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonCrossHairView : UIView
        {
            public GameObject crossHariRootActive;
            [HideInInspector]
            public bool oricrossHariRootActive;
            public GameObject normalActive;
            [HideInInspector]
            public bool orinormalActive;
            public GameObject countDownActive;
            [HideInInspector]
            public bool oricountDownActive;
            public Image numBgFillAmount;
            [HideInInspector]
            public float orinumBgFillAmount;
            public Text countNumText;
            [HideInInspector]
            public string oricountNumText;
            public GameObject addBloodActive;
            [HideInInspector]
            public bool oriaddBloodActive;
            public GameObject noVisibleActive;
            [HideInInspector]
            public bool orinoVisibleActive;
            public GameObject attackRootActive;
            [HideInInspector]
            public bool oriattackRootActive;
            public GameObject ImageWhiteActive;
            [HideInInspector]
            public bool oriImageWhiteActive;
            public RectTransform ImageWhiteSize;
            [HideInInspector]
            public Vector2 oriImageWhiteSize;
            public Image ImageWhiteColor;
            [HideInInspector]
            public Color oriImageWhiteColor;
            public GameObject ImageRedActive;
            [HideInInspector]
            public bool oriImageRedActive;
            public RectTransform ImageRedSize;
            [HideInInspector]
            public Vector2 oriImageRedSize;
            public Image ImageRedColor;
            [HideInInspector]
            public Color oriImageRedColor;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "crossHariRoot":
                            crossHariRootActive = v.gameObject;
                            break;
                        case "normal":
                            normalActive = v.gameObject;
                            break;
                        case "countDown":
                            countDownActive = v.gameObject;
                            break;
                        case "addBlood":
                            addBloodActive = v.gameObject;
                            break;
                        case "noVisible":
                            noVisibleActive = v.gameObject;
                            break;
                        case "attackRoot":
                            attackRootActive = v.gameObject;
                            break;
                        case "ImageWhite":
                            ImageWhiteActive = v.gameObject;
                            break;
                        case "ImageRed":
                            ImageRedActive = v.gameObject;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "numBg":
                            numBgFillAmount = v;
                            break;
                        case "ImageWhite":
                            ImageWhiteColor = v;
                            break;
                        case "ImageRed":
                            ImageRedColor = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "countNum":
                            countNumText = v;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ImageWhite":
                            ImageWhiteSize = v;
                            break;
                        case "ImageRed":
                            ImageRedSize = v;
                            break;
                    }
                }

            }
        }


        private bool _crossHariRootActive;
        private bool _normalActive;
        private bool _countDownActive;
        private float _numBgFillAmount;
        private string _countNumText;
        private bool _addBloodActive;
        private bool _noVisibleActive;
        private bool _attackRootActive;
        private bool _imageWhiteActive;
        private Vector2 _imageWhiteSize;
        private Color _imageWhiteColor;
        private bool _imageRedActive;
        private Vector2 _imageRedSize;
        private Color _imageRedColor;
        public bool crossHariRootActive { get { return _crossHariRootActive; } set {if(_crossHariRootActive != value) Set(ref _crossHariRootActive, value, "crossHariRootActive"); } }
        public bool normalActive { get { return _normalActive; } set {if(_normalActive != value) Set(ref _normalActive, value, "normalActive"); } }
        public bool countDownActive { get { return _countDownActive; } set {if(_countDownActive != value) Set(ref _countDownActive, value, "countDownActive"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float numBgFillAmount { get { return _numBgFillAmount; } set {if(_numBgFillAmount != value) Set(ref _numBgFillAmount, value, "numBgFillAmount"); } }
        public string countNumText { get { return _countNumText; } set {if(_countNumText != value) Set(ref _countNumText, value, "countNumText"); } }
        public bool addBloodActive { get { return _addBloodActive; } set {if(_addBloodActive != value) Set(ref _addBloodActive, value, "addBloodActive"); } }
        public bool noVisibleActive { get { return _noVisibleActive; } set {if(_noVisibleActive != value) Set(ref _noVisibleActive, value, "noVisibleActive"); } }
        public bool attackRootActive { get { return _attackRootActive; } set {if(_attackRootActive != value) Set(ref _attackRootActive, value, "attackRootActive"); } }
        public bool ImageWhiteActive { get { return _imageWhiteActive; } set {if(_imageWhiteActive != value) Set(ref _imageWhiteActive, value, "ImageWhiteActive"); } }
        public Vector2 ImageWhiteSize { get { return _imageWhiteSize; } set {if(_imageWhiteSize != value) Set(ref _imageWhiteSize, value, "ImageWhiteSize"); } }
        public Color ImageWhiteColor { get { return _imageWhiteColor; } set {if(_imageWhiteColor != value) Set(ref _imageWhiteColor, value, "ImageWhiteColor"); } }
        public bool ImageRedActive { get { return _imageRedActive; } set {if(_imageRedActive != value) Set(ref _imageRedActive, value, "ImageRedActive"); } }
        public Vector2 ImageRedSize { get { return _imageRedSize; } set {if(_imageRedSize != value) Set(ref _imageRedSize, value, "ImageRedSize"); } }
        public Color ImageRedColor { get { return _imageRedColor; } set {if(_imageRedColor != value) Set(ref _imageRedColor, value, "ImageRedColor"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonCrossHairView _view;
		
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

			var view = obj.GetComponent<CommonCrossHairView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonCrossHairView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonCrossHairView, CommonCrossHairViewModel> bindingSet =
                view.CreateBindingSet<CommonCrossHairView, CommonCrossHairViewModel>();

            view.oricrossHariRootActive = _crossHariRootActive = view.crossHariRootActive.activeSelf;
            bindingSet.Bind(view.crossHariRootActive).For(v => v.activeSelf).To(vm => vm.crossHariRootActive).OneWay();
            view.orinormalActive = _normalActive = view.normalActive.activeSelf;
            bindingSet.Bind(view.normalActive).For(v => v.activeSelf).To(vm => vm.normalActive).OneWay();
            view.oricountDownActive = _countDownActive = view.countDownActive.activeSelf;
            bindingSet.Bind(view.countDownActive).For(v => v.activeSelf).To(vm => vm.countDownActive).OneWay();
            view.orinumBgFillAmount = _numBgFillAmount = view.numBgFillAmount.fillAmount;
            bindingSet.Bind(view.numBgFillAmount).For(v => v.fillAmount).To(vm => vm.numBgFillAmount).OneWay();
            view.oricountNumText = _countNumText = view.countNumText.text;
            bindingSet.Bind(view.countNumText).For(v => v.text).To(vm => vm.countNumText).OneWay();
            view.oriaddBloodActive = _addBloodActive = view.addBloodActive.activeSelf;
            bindingSet.Bind(view.addBloodActive).For(v => v.activeSelf).To(vm => vm.addBloodActive).OneWay();
            view.orinoVisibleActive = _noVisibleActive = view.noVisibleActive.activeSelf;
            bindingSet.Bind(view.noVisibleActive).For(v => v.activeSelf).To(vm => vm.noVisibleActive).OneWay();
            view.oriattackRootActive = _attackRootActive = view.attackRootActive.activeSelf;
            bindingSet.Bind(view.attackRootActive).For(v => v.activeSelf).To(vm => vm.attackRootActive).OneWay();
            view.oriImageWhiteActive = _imageWhiteActive = view.ImageWhiteActive.activeSelf;
            bindingSet.Bind(view.ImageWhiteActive).For(v => v.activeSelf).To(vm => vm.ImageWhiteActive).OneWay();
            view.oriImageWhiteSize = _imageWhiteSize = view.ImageWhiteSize.sizeDelta;
            bindingSet.Bind(view.ImageWhiteSize).For(v => v.sizeDelta).To(vm => vm.ImageWhiteSize).OneWay();
            view.oriImageWhiteColor = _imageWhiteColor = view.ImageWhiteColor.color;
            bindingSet.Bind(view.ImageWhiteColor).For(v => v.color).To(vm => vm.ImageWhiteColor).OneWay();
            view.oriImageRedActive = _imageRedActive = view.ImageRedActive.activeSelf;
            bindingSet.Bind(view.ImageRedActive).For(v => v.activeSelf).To(vm => vm.ImageRedActive).OneWay();
            view.oriImageRedSize = _imageRedSize = view.ImageRedSize.sizeDelta;
            bindingSet.Bind(view.ImageRedSize).For(v => v.sizeDelta).To(vm => vm.ImageRedSize).OneWay();
            view.oriImageRedColor = _imageRedColor = view.ImageRedColor.color;
            bindingSet.Bind(view.ImageRedColor).For(v => v.color).To(vm => vm.ImageRedColor).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonCrossHairView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonCrossHairViewModel()
        {
            Type type = typeof(CommonCrossHairViewModel);
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

		private void SpriteReset()
		{
		}

		public void Reset()
		{
			crossHariRootActive = _view.oricrossHariRootActive;
			normalActive = _view.orinormalActive;
			countDownActive = _view.oricountDownActive;
			numBgFillAmount = _view.orinumBgFillAmount;
			countNumText = _view.oricountNumText;
			addBloodActive = _view.oriaddBloodActive;
			noVisibleActive = _view.orinoVisibleActive;
			attackRootActive = _view.oriattackRootActive;
			ImageWhiteActive = _view.oriImageWhiteActive;
			ImageWhiteSize = _view.oriImageWhiteSize;
			ImageWhiteColor = _view.oriImageWhiteColor;
			ImageRedActive = _view.oriImageRedActive;
			ImageRedSize = _view.oriImageRedSize;
			ImageRedColor = _view.oriImageRedColor;
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

        public string ResourceBundleName { get { return "uiprefabs/common"; } }
        public string ResourceAssetName { get { return "CommonCrossHair"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
