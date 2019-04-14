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

			bool bFirst = false;
			var view = obj.GetComponent<CommonSuoDuView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonSuoDuView>();
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

		void ViewBind(CommonSuoDuView view)
		{
		     BindingSet<CommonSuoDuView, CommonSuoDuViewModel> bindingSet =
                view.CreateBindingSet<CommonSuoDuView, CommonSuoDuViewModel>();
            bindingSet.Bind(view.TimeGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.TimeGameObjectActiveSelf).OneWay();
            bindingSet.Bind(view.TimeText).For(v => v.text).To(vm => vm.TimeText).OneWay();
            bindingSet.Bind(view.SuoDuValue).For(v => v.value).To(vm => vm.SuoDuValue).OneWay();
            bindingSet.Bind(view.PlayerImgColor).For(v => v.color).To(vm => vm.PlayerImgColor).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonSuoDuView view)
		{
            _timeGameObjectActiveSelf = view.TimeGameObjectActiveSelf.activeSelf;
            _timeText = view.TimeText.text;
            _suoDuValue = view.SuoDuValue.value;
            _playerImgColor = view.PlayerImgColor.color;
		}


		void SaveOriData(CommonSuoDuView view)
		{
            view.oriTimeGameObjectActiveSelf = _timeGameObjectActiveSelf;
            view.oriTimeText = _timeText;
            view.oriSuoDuValue = _suoDuValue;
            view.oriPlayerImgColor = _playerImgColor;
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

        public string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public string ResourceAssetName { get { return "CommonSuoDu"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
