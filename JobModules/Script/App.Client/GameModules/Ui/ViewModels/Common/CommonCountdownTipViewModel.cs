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
    public class CommonCountdownTipViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonCountdownTipView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text TitleText;
            [HideInInspector]
            public string oriTitleText;
            public Text TimeText;
            [HideInInspector]
            public string oriTimeText;
            
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

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Title":
                            TitleText = v;
                            break;
                        case "Time":
                            TimeText = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _titleText;
        private string _timeText;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string TitleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "TitleText"); } }
        public string TimeText { get { return _timeText; } set {if(_timeText != value) Set(ref _timeText, value, "TimeText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonCountdownTipView _view;
		
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
			var view = obj.GetComponent<CommonCountdownTipView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonCountdownTipView>();
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
		private void EventTriggerBind(CommonCountdownTipView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonCountdownTipViewModel()
        {
            Type type = typeof(CommonCountdownTipViewModel);
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

		void ViewBind(CommonCountdownTipView view)
		{
		     BindingSet<CommonCountdownTipView, CommonCountdownTipViewModel> bindingSet =
                view.CreateBindingSet<CommonCountdownTipView, CommonCountdownTipViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.TitleText).For(v => v.text).To(vm => vm.TitleText).OneWay();
            bindingSet.Bind(view.TimeText).For(v => v.text).To(vm => vm.TimeText).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonCountdownTipView view)
		{
            _show = view.Show.activeSelf;
            _titleText = view.TitleText.text;
            _timeText = view.TimeText.text;
		}


		void SaveOriData(CommonCountdownTipView view)
		{
            view.oriShow = _show;
            view.oriTitleText = _titleText;
            view.oriTimeText = _timeText;
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
			TitleText = _view.oriTitleText;
			TimeText = _view.oriTimeText;
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
        public string ResourceAssetName { get { return "CommonCountdownTip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
