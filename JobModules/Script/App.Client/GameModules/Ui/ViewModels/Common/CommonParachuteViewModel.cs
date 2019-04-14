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
    public class CommonParachuteViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonParachuteView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Slider HeightSliderValue;
            [HideInInspector]
            public float oriHeightSliderValue;
            public RectTransform HandleScale;
            [HideInInspector]
            public Vector3 oriHandleScale;
            public RectTransform HeightTipGroupPosition;
            [HideInInspector]
            public Vector3 oriHeightTipGroupPosition;
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
                        case "HeightSlider":
                            HeightSliderValue = v;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Handle":
                            HandleScale = v;
                            break;
                        case "HeightTipGroup":
                            HeightTipGroupPosition = v;
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
        private float _heightSliderValue;
        private Vector3 _handleScale;
        private Vector3 _heightTipGroupPosition;
        private string _speedString;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float HeightSliderValue { get { return _heightSliderValue; } set {if(_heightSliderValue != value) Set(ref _heightSliderValue, value, "HeightSliderValue"); } }
        public Vector3 HandleScale { get { return _handleScale; } set {if(_handleScale != value) Set(ref _handleScale, value, "HandleScale"); } }
        public Vector3 HeightTipGroupPosition { get { return _heightTipGroupPosition; } set {if(_heightTipGroupPosition != value) Set(ref _heightTipGroupPosition, value, "HeightTipGroupPosition"); } }
        public string SpeedString { get { return _speedString; } set {if(_speedString != value) Set(ref _speedString, value, "SpeedString"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonParachuteView _view;
		
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
			var view = obj.GetComponent<CommonParachuteView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonParachuteView>();
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
		private void EventTriggerBind(CommonParachuteView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonParachuteViewModel()
        {
            Type type = typeof(CommonParachuteViewModel);
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

		void ViewBind(CommonParachuteView view)
		{
		     BindingSet<CommonParachuteView, CommonParachuteViewModel> bindingSet =
                view.CreateBindingSet<CommonParachuteView, CommonParachuteViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.HeightSliderValue).For(v => v.value).To(vm => vm.HeightSliderValue).OneWay();
            bindingSet.Bind(view.HandleScale).For(v => v.localScale).To(vm => vm.HandleScale).OneWay();
            bindingSet.Bind(view.HeightTipGroupPosition).For(v => v.localPosition).To(vm => vm.HeightTipGroupPosition).OneWay();
            bindingSet.Bind(view.SpeedString).For(v => v.text).To(vm => vm.SpeedString).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonParachuteView view)
		{
            _show = view.Show.activeSelf;
            _heightSliderValue = view.HeightSliderValue.value;
            _handleScale = view.HandleScale.localScale;
            _heightTipGroupPosition = view.HeightTipGroupPosition.localPosition;
            _speedString = view.SpeedString.text;
		}


		void SaveOriData(CommonParachuteView view)
		{
            view.oriShow = _show;
            view.oriHeightSliderValue = _heightSliderValue;
            view.oriHandleScale = _handleScale;
            view.oriHeightTipGroupPosition = _heightTipGroupPosition;
            view.oriSpeedString = _speedString;
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
			Show = _view.oriShow;
			HeightSliderValue = _view.oriHeightSliderValue;
			HandleScale = _view.oriHandleScale;
			HeightTipGroupPosition = _view.oriHeightTipGroupPosition;
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

        public string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public string ResourceAssetName { get { return "CommonParachute"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
