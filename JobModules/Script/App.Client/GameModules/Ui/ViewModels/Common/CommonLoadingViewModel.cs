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
    public class CommonLoadingViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonLoadingView : UIView
        {
            public GameObject showActive;
            [HideInInspector]
            public bool orishowActive;
            public Slider sliderValue;
            [HideInInspector]
            public float orisliderValue;
            public Text curText;
            [HideInInspector]
            public string oricurText;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            showActive = v.gameObject;
                            break;
                    }
                }

                Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
                foreach (var v in sliders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "slider":
                            sliderValue = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "text":
                            curText = v;
                            break;
                    }
                }

            }
        }


        private bool _showActive;
        private float _sliderValue;
        private string _curText;
        public bool showActive { get { return _showActive; } set {if(_showActive != value) Set(ref _showActive, value, "showActive"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float sliderValue { get { return _sliderValue; } set {if(_sliderValue != value) Set(ref _sliderValue, value, "sliderValue"); } }
        public string curText { get { return _curText; } set {if(_curText != value) Set(ref _curText, value, "curText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonLoadingView _view;
		
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

			var view = obj.GetComponent<CommonLoadingView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonLoadingView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonLoadingView, CommonLoadingViewModel> bindingSet =
                view.CreateBindingSet<CommonLoadingView, CommonLoadingViewModel>();

            view.orishowActive = _showActive = view.showActive.activeSelf;
            bindingSet.Bind(view.showActive).For(v => v.activeSelf).To(vm => vm.showActive).OneWay();
            view.orisliderValue = _sliderValue = view.sliderValue.value;
            bindingSet.Bind(view.sliderValue).For(v => v.value).To(vm => vm.sliderValue).OneWay();
            view.oricurText = _curText = view.curText.text;
            bindingSet.Bind(view.curText).For(v => v.text).To(vm => vm.curText).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonLoadingView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonLoadingViewModel()
        {
            Type type = typeof(CommonLoadingViewModel);
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
			showActive = _view.orishowActive;
			sliderValue = _view.orisliderValue;
			curText = _view.oricurText;
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
        public string ResourceAssetName { get { return "CommonLoading"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
