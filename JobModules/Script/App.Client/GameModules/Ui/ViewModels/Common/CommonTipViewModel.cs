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
    public class CommonTipViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonTipView : UIView
        {
            public RectTransform rootLocation;
            [HideInInspector]
            public Vector2 orirootLocation;
            public GameObject rootActiveSelf;
            [HideInInspector]
            public bool orirootActiveSelf;
            public Text typeText;
            [HideInInspector]
            public string oritypeText;
            public Text nameText;
            [HideInInspector]
            public string orinameText;
            public Text infoOneText;
            [HideInInspector]
            public string oriinfoOneText;
            public GameObject infoOneActiveSelf;
            [HideInInspector]
            public bool oriinfoOneActiveSelf;
            public Text infoTwoText;
            [HideInInspector]
            public string oriinfoTwoText;
            public GameObject infoTwoActiveSelf;
            [HideInInspector]
            public bool oriinfoTwoActiveSelf;
            public Image Icon;
            public GameObject attrGroupActiveSelf;
            [HideInInspector]
            public bool oriattrGroupActiveSelf;
            public GameObject desTextActiveSelf;
            [HideInInspector]
            public bool oridesTextActiveSelf;
            public Text desText;
            [HideInInspector]
            public string oridesText;
            
            public void FillField()
            {
                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootLocation = v;
                            break;
                    }
                }

                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootActiveSelf = v.gameObject;
                            break;
                        case "infoOne":
                            infoOneActiveSelf = v.gameObject;
                            break;
                        case "infoTwo":
                            infoTwoActiveSelf = v.gameObject;
                            break;
                        case "AttrGroup":
                            attrGroupActiveSelf = v.gameObject;
                            break;
                        case "DesText":
                            desTextActiveSelf = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "TypeText":
                            typeText = v;
                            break;
                        case "NameText":
                            nameText = v;
                            break;
                        case "infoOne":
                            infoOneText = v;
                            break;
                        case "infoTwo":
                            infoTwoText = v;
                            break;
                        case "DesText":
                            desText = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Icon":
                            Icon = v;
                            break;
                    }
                }

            }
        }


        private Vector2 _rootLocation;
        private bool _rootActiveSelf;
        private string _typeText;
        private string _nameText;
        private string _infoOneText;
        private bool _infoOneActiveSelf;
        private string _infoTwoText;
        private bool _infoTwoActiveSelf;
        private Sprite _icon;
        private bool _attrGroupActiveSelf;
        private bool _desTextActiveSelf;
        private string _desText;
        public Vector2 rootLocation { get { return _rootLocation; } set {if(_rootLocation != value) Set(ref _rootLocation, value, "rootLocation"); } }
        public bool rootActiveSelf { get { return _rootActiveSelf; } set {if(_rootActiveSelf != value) Set(ref _rootActiveSelf, value, "rootActiveSelf"); } }
        public string typeText { get { return _typeText; } set {if(_typeText != value) Set(ref _typeText, value, "typeText"); } }
        public string nameText { get { return _nameText; } set {if(_nameText != value) Set(ref _nameText, value, "nameText"); } }
        public string infoOneText { get { return _infoOneText; } set {if(_infoOneText != value) Set(ref _infoOneText, value, "infoOneText"); } }
        public bool infoOneActiveSelf { get { return _infoOneActiveSelf; } set {if(_infoOneActiveSelf != value) Set(ref _infoOneActiveSelf, value, "infoOneActiveSelf"); } }
        public string infoTwoText { get { return _infoTwoText; } set {if(_infoTwoText != value) Set(ref _infoTwoText, value, "infoTwoText"); } }
        public bool infoTwoActiveSelf { get { return _infoTwoActiveSelf; } set {if(_infoTwoActiveSelf != value) Set(ref _infoTwoActiveSelf, value, "infoTwoActiveSelf"); } }
        public Sprite Icon { get { return _icon; } set {if(_icon != value) Set(ref _icon, value, "Icon"); if(null != _view && null != _view.Icon && null == value) _view.Icon.sprite = ViewModelUtil.EmptySprite; } }
        public bool attrGroupActiveSelf { get { return _attrGroupActiveSelf; } set {if(_attrGroupActiveSelf != value) Set(ref _attrGroupActiveSelf, value, "attrGroupActiveSelf"); } }
        public bool desTextActiveSelf { get { return _desTextActiveSelf; } set {if(_desTextActiveSelf != value) Set(ref _desTextActiveSelf, value, "desTextActiveSelf"); } }
        public string desText { get { return _desText; } set {if(_desText != value) Set(ref _desText, value, "desText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonTipView _view;
		
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

			var view = obj.GetComponent<CommonTipView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonTipView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonTipView, CommonTipViewModel> bindingSet =
                view.CreateBindingSet<CommonTipView, CommonTipViewModel>();

            view.orirootLocation = _rootLocation = view.rootLocation.anchoredPosition;
            bindingSet.Bind(view.rootLocation).For(v => v.anchoredPosition).To(vm => vm.rootLocation).OneWay();
            view.orirootActiveSelf = _rootActiveSelf = view.rootActiveSelf.activeSelf;
            bindingSet.Bind(view.rootActiveSelf).For(v => v.activeSelf).To(vm => vm.rootActiveSelf).OneWay();
            view.oritypeText = _typeText = view.typeText.text;
            bindingSet.Bind(view.typeText).For(v => v.text).To(vm => vm.typeText).OneWay();
            view.orinameText = _nameText = view.nameText.text;
            bindingSet.Bind(view.nameText).For(v => v.text).To(vm => vm.nameText).OneWay();
            view.oriinfoOneText = _infoOneText = view.infoOneText.text;
            bindingSet.Bind(view.infoOneText).For(v => v.text).To(vm => vm.infoOneText).OneWay();
            view.oriinfoOneActiveSelf = _infoOneActiveSelf = view.infoOneActiveSelf.activeSelf;
            bindingSet.Bind(view.infoOneActiveSelf).For(v => v.activeSelf).To(vm => vm.infoOneActiveSelf).OneWay();
            view.oriinfoTwoText = _infoTwoText = view.infoTwoText.text;
            bindingSet.Bind(view.infoTwoText).For(v => v.text).To(vm => vm.infoTwoText).OneWay();
            view.oriinfoTwoActiveSelf = _infoTwoActiveSelf = view.infoTwoActiveSelf.activeSelf;
            bindingSet.Bind(view.infoTwoActiveSelf).For(v => v.activeSelf).To(vm => vm.infoTwoActiveSelf).OneWay();
            bindingSet.Bind(view.Icon).For(v => v.sprite).To(vm => vm.Icon).OneWay();
            view.oriattrGroupActiveSelf = _attrGroupActiveSelf = view.attrGroupActiveSelf.activeSelf;
            bindingSet.Bind(view.attrGroupActiveSelf).For(v => v.activeSelf).To(vm => vm.attrGroupActiveSelf).OneWay();
            view.oridesTextActiveSelf = _desTextActiveSelf = view.desTextActiveSelf.activeSelf;
            bindingSet.Bind(view.desTextActiveSelf).For(v => v.activeSelf).To(vm => vm.desTextActiveSelf).OneWay();
            view.oridesText = _desText = view.desText.text;
            bindingSet.Bind(view.desText).For(v => v.text).To(vm => vm.desText).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonTipView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonTipViewModel()
        {
            Type type = typeof(CommonTipViewModel);
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
			Icon = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			rootLocation = _view.orirootLocation;
			rootActiveSelf = _view.orirootActiveSelf;
			typeText = _view.oritypeText;
			nameText = _view.orinameText;
			infoOneText = _view.oriinfoOneText;
			infoOneActiveSelf = _view.oriinfoOneActiveSelf;
			infoTwoText = _view.oriinfoTwoText;
			infoTwoActiveSelf = _view.oriinfoTwoActiveSelf;
			attrGroupActiveSelf = _view.oriattrGroupActiveSelf;
			desTextActiveSelf = _view.oridesTextActiveSelf;
			desText = _view.oridesText;
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
        public string ResourceAssetName { get { return "CommonTip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
