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
    public class CommonDebugInfoViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonDebugInfoView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text LeftInfoText;
            [HideInInspector]
            public string oriLeftInfoText;
            public Text RightInfoText;
            [HideInInspector]
            public string oriRightInfoText;
            
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
                        case "LeftInfoText":
                            LeftInfoText = v;
                            break;
                        case "RightInfoText":
                            RightInfoText = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _leftInfoText;
        private string _rightInfoText;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string LeftInfoText { get { return _leftInfoText; } set {if(_leftInfoText != value) Set(ref _leftInfoText, value, "LeftInfoText"); } }
        public string RightInfoText { get { return _rightInfoText; } set {if(_rightInfoText != value) Set(ref _rightInfoText, value, "RightInfoText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonDebugInfoView _view;
		
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

			var view = obj.GetComponent<CommonDebugInfoView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonDebugInfoView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonDebugInfoView, CommonDebugInfoViewModel> bindingSet =
                view.CreateBindingSet<CommonDebugInfoView, CommonDebugInfoViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.oriLeftInfoText = _leftInfoText = view.LeftInfoText.text;
            bindingSet.Bind(view.LeftInfoText).For(v => v.text).To(vm => vm.LeftInfoText).OneWay();
            view.oriRightInfoText = _rightInfoText = view.RightInfoText.text;
            bindingSet.Bind(view.RightInfoText).For(v => v.text).To(vm => vm.RightInfoText).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonDebugInfoView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonDebugInfoViewModel()
        {
            Type type = typeof(CommonDebugInfoViewModel);
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
			LeftInfoText = _view.oriLeftInfoText;
			RightInfoText = _view.oriRightInfoText;
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
        public string ResourceAssetName { get { return "CommonDebugInfo"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
