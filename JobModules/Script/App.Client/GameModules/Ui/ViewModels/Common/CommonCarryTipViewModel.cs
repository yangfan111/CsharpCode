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
    public class CommonCarryTipViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonCarryTipView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Slider HpBarValue;
            [HideInInspector]
            public float oriHpBarValue;
            public Image HpFillColor;
            [HideInInspector]
            public Color oriHpFillColor;
            public Slider OilBarValue;
            [HideInInspector]
            public float oriOilBarValue;
            public Text SpeedString;
            [HideInInspector]
            public string oriSpeedString;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            Show = v.gameObject;
                            break;
                    }
                }

                Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
                foreach (var v in sliders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "HpBar":
                            HpBarValue = v;
                            break;
                        case "OilBar":
                            OilBarValue = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "HpFill":
                            HpFillColor = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Speed":
                            SpeedString = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private float _hpBarValue;
        private Color _hpFillColor;
        private float _oilBarValue;
        private string _speedString;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float HpBarValue { get { return _hpBarValue; } set {if(_hpBarValue != value) Set(ref _hpBarValue, value, "HpBarValue"); } }
        public Color HpFillColor { get { return _hpFillColor; } set {if(_hpFillColor != value) Set(ref _hpFillColor, value, "HpFillColor"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float OilBarValue { get { return _oilBarValue; } set {if(_oilBarValue != value) Set(ref _oilBarValue, value, "OilBarValue"); } }
        public string SpeedString { get { return _speedString; } set {if(_speedString != value) Set(ref _speedString, value, "SpeedString"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonCarryTipView _view;
		
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

			var view = obj.GetComponent<CommonCarryTipView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonCarryTipView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonCarryTipView, CommonCarryTipViewModel> bindingSet =
                view.CreateBindingSet<CommonCarryTipView, CommonCarryTipViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.oriHpBarValue = _hpBarValue = view.HpBarValue.value;
            bindingSet.Bind(view.HpBarValue).For(v => v.value).To(vm => vm.HpBarValue).OneWay();
            view.oriHpFillColor = _hpFillColor = view.HpFillColor.color;
            bindingSet.Bind(view.HpFillColor).For(v => v.color).To(vm => vm.HpFillColor).OneWay();
            view.oriOilBarValue = _oilBarValue = view.OilBarValue.value;
            bindingSet.Bind(view.OilBarValue).For(v => v.value).To(vm => vm.OilBarValue).OneWay();
            view.oriSpeedString = _speedString = view.SpeedString.text;
            bindingSet.Bind(view.SpeedString).For(v => v.text).To(vm => vm.SpeedString).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonCarryTipView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonCarryTipViewModel()
        {
            Type type = typeof(CommonCarryTipViewModel);
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
			Show = _view.oriShow;
			HpBarValue = _view.oriHpBarValue;
			HpFillColor = _view.oriHpFillColor;
			OilBarValue = _view.oriOilBarValue;
			SpeedString = _view.oriSpeedString;
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
        public string ResourceAssetName { get { return "CommonCarryTip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
