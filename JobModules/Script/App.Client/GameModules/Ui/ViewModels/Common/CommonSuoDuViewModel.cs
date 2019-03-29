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
    public class CommonSuoDuViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonSuoDuView : UIView
        {
            public GameObject TimeGameObjectActiveSelf;
            [HideInInspector]
            public bool oriTimeGameObjectActiveSelf;
            public Text TimeText;
            [HideInInspector]
            public string oriTimeText;
            public Slider SuoDuValue;
            [HideInInspector]
            public float oriSuoDuValue;
            public Image PlayerImgColor;
            [HideInInspector]
            public Color oriPlayerImgColor;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Time":
                            TimeGameObjectActiveSelf = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Time":
                            TimeText = v;
                            break;
                    }
                }

                Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
                foreach (var v in sliders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "SuoDu":
                            SuoDuValue = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PlayerImg":
                            PlayerImgColor = v;
                            break;
                    }
                }

            }
        }


        private bool _timeGameObjectActiveSelf;
        private string _timeText;
        private float _suoDuValue;
        private Color _playerImgColor;
        public bool TimeGameObjectActiveSelf { get { return _timeGameObjectActiveSelf; } set {if(_timeGameObjectActiveSelf != value) Set(ref _timeGameObjectActiveSelf, value, "TimeGameObjectActiveSelf"); } }
        public string TimeText { get { return _timeText; } set {if(_timeText != value) Set(ref _timeText, value, "TimeText"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float SuoDuValue { get { return _suoDuValue; } set {if(_suoDuValue != value) Set(ref _suoDuValue, value, "SuoDuValue"); } }
        public Color PlayerImgColor { get { return _playerImgColor; } set {if(_playerImgColor != value) Set(ref _playerImgColor, value, "PlayerImgColor"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonSuoDuView _view;
		
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

			var view = obj.GetComponent<CommonSuoDuView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonSuoDuView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonSuoDuView, CommonSuoDuViewModel> bindingSet =
                view.CreateBindingSet<CommonSuoDuView, CommonSuoDuViewModel>();

            view.oriTimeGameObjectActiveSelf = _timeGameObjectActiveSelf = view.TimeGameObjectActiveSelf.activeSelf;
            bindingSet.Bind(view.TimeGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.TimeGameObjectActiveSelf).OneWay();
            view.oriTimeText = _timeText = view.TimeText.text;
            bindingSet.Bind(view.TimeText).For(v => v.text).To(vm => vm.TimeText).OneWay();
            view.oriSuoDuValue = _suoDuValue = view.SuoDuValue.value;
            bindingSet.Bind(view.SuoDuValue).For(v => v.value).To(vm => vm.SuoDuValue).OneWay();
            view.oriPlayerImgColor = _playerImgColor = view.PlayerImgColor.color;
            bindingSet.Bind(view.PlayerImgColor).For(v => v.color).To(vm => vm.PlayerImgColor).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonSuoDuView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonSuoDuViewModel()
        {
            Type type = typeof(CommonSuoDuViewModel);
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
			TimeGameObjectActiveSelf = _view.oriTimeGameObjectActiveSelf;
			TimeText = _view.oriTimeText;
			SuoDuValue = _view.oriSuoDuValue;
			PlayerImgColor = _view.oriPlayerImgColor;
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
        public string ResourceAssetName { get { return "CommonSuoDu"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
