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
    public class WeaponPropertyBarItemViewModel : ViewModelBase, IUiViewModel
    {
        private class WeaponPropertyBarItemView : UIView
        {
            public GameObject ProgressBgActiveSelf;
            [HideInInspector]
            public bool oriProgressBgActiveSelf;
            public RectTransform ProgressBgSizeDelta;
            [HideInInspector]
            public Vector2 oriProgressBgSizeDelta;
            public Text PropertyNameText;
            [HideInInspector]
            public string oriPropertyNameText;
            public GameObject CurrentBarImageActiveSelf;
            [HideInInspector]
            public bool oriCurrentBarImageActiveSelf;
            public RectTransform CurrentBarImageSizeDelta;
            [HideInInspector]
            public Vector2 oriCurrentBarImageSizeDelta;
            public RectTransform CurrentBarImageAnchoredPosition;
            [HideInInspector]
            public Vector2 oriCurrentBarImageAnchoredPosition;
            public GameObject MissBarImageActiveSelf;
            [HideInInspector]
            public bool oriMissBarImageActiveSelf;
            public RectTransform MissBarImageAnchoredPosition;
            [HideInInspector]
            public Vector2 oriMissBarImageAnchoredPosition;
            public RectTransform MissBarImageSizeDelta;
            [HideInInspector]
            public Vector2 oriMissBarImageSizeDelta;
            public GameObject AddBarImageActiveSelf;
            [HideInInspector]
            public bool oriAddBarImageActiveSelf;
            public RectTransform AddBarImageAnchoredPosition;
            [HideInInspector]
            public Vector2 oriAddBarImageAnchoredPosition;
            public RectTransform AddBarImageSizeDelta;
            [HideInInspector]
            public Vector2 oriAddBarImageSizeDelta;
            public GameObject DownImageActiveSelf;
            [HideInInspector]
            public bool oriDownImageActiveSelf;
            public GameObject UpImageActiveSelf;
            [HideInInspector]
            public bool oriUpImageActiveSelf;
            public Text NumberText;
            [HideInInspector]
            public string oriNumberText;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ProgressBg":
                            ProgressBgActiveSelf = v.gameObject;
                            break;
                        case "CurrentBarImage":
                            CurrentBarImageActiveSelf = v.gameObject;
                            break;
                        case "MissBarImage":
                            MissBarImageActiveSelf = v.gameObject;
                            break;
                        case "AddBarImage":
                            AddBarImageActiveSelf = v.gameObject;
                            break;
                        case "DownImage":
                            DownImageActiveSelf = v.gameObject;
                            break;
                        case "UpImage":
                            UpImageActiveSelf = v.gameObject;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ProgressBg":
                            ProgressBgSizeDelta = v;
                            break;
                        case "CurrentBarImage":
                            CurrentBarImageSizeDelta = v;
                            CurrentBarImageAnchoredPosition = v;
                            break;
                        case "MissBarImage":
                            MissBarImageAnchoredPosition = v;
                            MissBarImageSizeDelta = v;
                            break;
                        case "AddBarImage":
                            AddBarImageAnchoredPosition = v;
                            AddBarImageSizeDelta = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PropertyNameText":
                            PropertyNameText = v;
                            break;
                        case "NumberText":
                            NumberText = v;
                            break;
                    }
                }

            }
        }


        private bool _progressBgActiveSelf;
        private Vector2 _progressBgSizeDelta;
        private string _propertyNameText;
        private bool _currentBarImageActiveSelf;
        private Vector2 _currentBarImageSizeDelta;
        private Vector2 _currentBarImageAnchoredPosition;
        private bool _missBarImageActiveSelf;
        private Vector2 _missBarImageAnchoredPosition;
        private Vector2 _missBarImageSizeDelta;
        private bool _addBarImageActiveSelf;
        private Vector2 _addBarImageAnchoredPosition;
        private Vector2 _addBarImageSizeDelta;
        private bool _downImageActiveSelf;
        private bool _upImageActiveSelf;
        private string _numberText;
        public bool ProgressBgActiveSelf { get { return _progressBgActiveSelf; } set {if(_progressBgActiveSelf != value) Set(ref _progressBgActiveSelf, value, "ProgressBgActiveSelf"); } }
        public Vector2 ProgressBgSizeDelta { get { return _progressBgSizeDelta; } set {if(_progressBgSizeDelta != value) Set(ref _progressBgSizeDelta, value, "ProgressBgSizeDelta"); } }
        public string PropertyNameText { get { return _propertyNameText; } set {if(_propertyNameText != value) Set(ref _propertyNameText, value, "PropertyNameText"); } }
        public bool CurrentBarImageActiveSelf { get { return _currentBarImageActiveSelf; } set {if(_currentBarImageActiveSelf != value) Set(ref _currentBarImageActiveSelf, value, "CurrentBarImageActiveSelf"); } }
        public Vector2 CurrentBarImageSizeDelta { get { return _currentBarImageSizeDelta; } set {if(_currentBarImageSizeDelta != value) Set(ref _currentBarImageSizeDelta, value, "CurrentBarImageSizeDelta"); } }
        public Vector2 CurrentBarImageAnchoredPosition { get { return _currentBarImageAnchoredPosition; } set {if(_currentBarImageAnchoredPosition != value) Set(ref _currentBarImageAnchoredPosition, value, "CurrentBarImageAnchoredPosition"); } }
        public bool MissBarImageActiveSelf { get { return _missBarImageActiveSelf; } set {if(_missBarImageActiveSelf != value) Set(ref _missBarImageActiveSelf, value, "MissBarImageActiveSelf"); } }
        public Vector2 MissBarImageAnchoredPosition { get { return _missBarImageAnchoredPosition; } set {if(_missBarImageAnchoredPosition != value) Set(ref _missBarImageAnchoredPosition, value, "MissBarImageAnchoredPosition"); } }
        public Vector2 MissBarImageSizeDelta { get { return _missBarImageSizeDelta; } set {if(_missBarImageSizeDelta != value) Set(ref _missBarImageSizeDelta, value, "MissBarImageSizeDelta"); } }
        public bool AddBarImageActiveSelf { get { return _addBarImageActiveSelf; } set {if(_addBarImageActiveSelf != value) Set(ref _addBarImageActiveSelf, value, "AddBarImageActiveSelf"); } }
        public Vector2 AddBarImageAnchoredPosition { get { return _addBarImageAnchoredPosition; } set {if(_addBarImageAnchoredPosition != value) Set(ref _addBarImageAnchoredPosition, value, "AddBarImageAnchoredPosition"); } }
        public Vector2 AddBarImageSizeDelta { get { return _addBarImageSizeDelta; } set {if(_addBarImageSizeDelta != value) Set(ref _addBarImageSizeDelta, value, "AddBarImageSizeDelta"); } }
        public bool DownImageActiveSelf { get { return _downImageActiveSelf; } set {if(_downImageActiveSelf != value) Set(ref _downImageActiveSelf, value, "DownImageActiveSelf"); } }
        public bool UpImageActiveSelf { get { return _upImageActiveSelf; } set {if(_upImageActiveSelf != value) Set(ref _upImageActiveSelf, value, "UpImageActiveSelf"); } }
        public string NumberText { get { return _numberText; } set {if(_numberText != value) Set(ref _numberText, value, "NumberText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private WeaponPropertyBarItemView _view;
		
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
			var view = obj.GetComponent<WeaponPropertyBarItemView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<WeaponPropertyBarItemView>();
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
		private void EventTriggerBind(WeaponPropertyBarItemView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static WeaponPropertyBarItemViewModel()
        {
            Type type = typeof(WeaponPropertyBarItemViewModel);
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

		void ViewBind(WeaponPropertyBarItemView view)
		{
		     BindingSet<WeaponPropertyBarItemView, WeaponPropertyBarItemViewModel> bindingSet =
                view.CreateBindingSet<WeaponPropertyBarItemView, WeaponPropertyBarItemViewModel>();
            bindingSet.Bind(view.ProgressBgActiveSelf).For(v => v.activeSelf).To(vm => vm.ProgressBgActiveSelf).OneWay();
            bindingSet.Bind(view.ProgressBgSizeDelta).For(v => v.sizeDelta).To(vm => vm.ProgressBgSizeDelta).OneWay();
            bindingSet.Bind(view.PropertyNameText).For(v => v.text).To(vm => vm.PropertyNameText).OneWay();
            bindingSet.Bind(view.CurrentBarImageActiveSelf).For(v => v.activeSelf).To(vm => vm.CurrentBarImageActiveSelf).OneWay();
            bindingSet.Bind(view.CurrentBarImageSizeDelta).For(v => v.sizeDelta).To(vm => vm.CurrentBarImageSizeDelta).OneWay();
            bindingSet.Bind(view.CurrentBarImageAnchoredPosition).For(v => v.anchoredPosition).To(vm => vm.CurrentBarImageAnchoredPosition).OneWay();
            bindingSet.Bind(view.MissBarImageActiveSelf).For(v => v.activeSelf).To(vm => vm.MissBarImageActiveSelf).OneWay();
            bindingSet.Bind(view.MissBarImageAnchoredPosition).For(v => v.anchoredPosition).To(vm => vm.MissBarImageAnchoredPosition).OneWay();
            bindingSet.Bind(view.MissBarImageSizeDelta).For(v => v.sizeDelta).To(vm => vm.MissBarImageSizeDelta).OneWay();
            bindingSet.Bind(view.AddBarImageActiveSelf).For(v => v.activeSelf).To(vm => vm.AddBarImageActiveSelf).OneWay();
            bindingSet.Bind(view.AddBarImageAnchoredPosition).For(v => v.anchoredPosition).To(vm => vm.AddBarImageAnchoredPosition).OneWay();
            bindingSet.Bind(view.AddBarImageSizeDelta).For(v => v.sizeDelta).To(vm => vm.AddBarImageSizeDelta).OneWay();
            bindingSet.Bind(view.DownImageActiveSelf).For(v => v.activeSelf).To(vm => vm.DownImageActiveSelf).OneWay();
            bindingSet.Bind(view.UpImageActiveSelf).For(v => v.activeSelf).To(vm => vm.UpImageActiveSelf).OneWay();
            bindingSet.Bind(view.NumberText).For(v => v.text).To(vm => vm.NumberText).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(WeaponPropertyBarItemView view)
		{
            _progressBgActiveSelf = view.ProgressBgActiveSelf.activeSelf;
            _progressBgSizeDelta = view.ProgressBgSizeDelta.sizeDelta;
            _propertyNameText = view.PropertyNameText.text;
            _currentBarImageActiveSelf = view.CurrentBarImageActiveSelf.activeSelf;
            _currentBarImageSizeDelta = view.CurrentBarImageSizeDelta.sizeDelta;
            _currentBarImageAnchoredPosition = view.CurrentBarImageAnchoredPosition.anchoredPosition;
            _missBarImageActiveSelf = view.MissBarImageActiveSelf.activeSelf;
            _missBarImageAnchoredPosition = view.MissBarImageAnchoredPosition.anchoredPosition;
            _missBarImageSizeDelta = view.MissBarImageSizeDelta.sizeDelta;
            _addBarImageActiveSelf = view.AddBarImageActiveSelf.activeSelf;
            _addBarImageAnchoredPosition = view.AddBarImageAnchoredPosition.anchoredPosition;
            _addBarImageSizeDelta = view.AddBarImageSizeDelta.sizeDelta;
            _downImageActiveSelf = view.DownImageActiveSelf.activeSelf;
            _upImageActiveSelf = view.UpImageActiveSelf.activeSelf;
            _numberText = view.NumberText.text;
		}


		void SaveOriData(WeaponPropertyBarItemView view)
		{
            view.oriProgressBgActiveSelf = _progressBgActiveSelf;
            view.oriProgressBgSizeDelta = _progressBgSizeDelta;
            view.oriPropertyNameText = _propertyNameText;
            view.oriCurrentBarImageActiveSelf = _currentBarImageActiveSelf;
            view.oriCurrentBarImageSizeDelta = _currentBarImageSizeDelta;
            view.oriCurrentBarImageAnchoredPosition = _currentBarImageAnchoredPosition;
            view.oriMissBarImageActiveSelf = _missBarImageActiveSelf;
            view.oriMissBarImageAnchoredPosition = _missBarImageAnchoredPosition;
            view.oriMissBarImageSizeDelta = _missBarImageSizeDelta;
            view.oriAddBarImageActiveSelf = _addBarImageActiveSelf;
            view.oriAddBarImageAnchoredPosition = _addBarImageAnchoredPosition;
            view.oriAddBarImageSizeDelta = _addBarImageSizeDelta;
            view.oriDownImageActiveSelf = _downImageActiveSelf;
            view.oriUpImageActiveSelf = _upImageActiveSelf;
            view.oriNumberText = _numberText;
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
			ProgressBgActiveSelf = _view.oriProgressBgActiveSelf;
			ProgressBgSizeDelta = _view.oriProgressBgSizeDelta;
			PropertyNameText = _view.oriPropertyNameText;
			CurrentBarImageActiveSelf = _view.oriCurrentBarImageActiveSelf;
			CurrentBarImageSizeDelta = _view.oriCurrentBarImageSizeDelta;
			CurrentBarImageAnchoredPosition = _view.oriCurrentBarImageAnchoredPosition;
			MissBarImageActiveSelf = _view.oriMissBarImageActiveSelf;
			MissBarImageAnchoredPosition = _view.oriMissBarImageAnchoredPosition;
			MissBarImageSizeDelta = _view.oriMissBarImageSizeDelta;
			AddBarImageActiveSelf = _view.oriAddBarImageActiveSelf;
			AddBarImageAnchoredPosition = _view.oriAddBarImageAnchoredPosition;
			AddBarImageSizeDelta = _view.oriAddBarImageSizeDelta;
			DownImageActiveSelf = _view.oriDownImageActiveSelf;
			UpImageActiveSelf = _view.oriUpImageActiveSelf;
			NumberText = _view.oriNumberText;
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
        public string ResourceAssetName { get { return "BattleWeaponPropertyBarItem"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
