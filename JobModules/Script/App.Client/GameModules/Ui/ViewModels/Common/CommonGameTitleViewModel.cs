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
    public class CommonGameTitleViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonGameTitleView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public GameObject KdShow;
            [HideInInspector]
            public bool oriKdShow;
            public GameObject AceShow;
            [HideInInspector]
            public bool oriAceShow;
            public GameObject SecondShow;
            [HideInInspector]
            public bool oriSecondShow;
            public GameObject ThirdShow;
            [HideInInspector]
            public bool oriThirdShow;
            
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
                        case "Kd":
                            KdShow = v.gameObject;
                            break;
                        case "Ace":
                            AceShow = v.gameObject;
                            break;
                        case "2nd":
                            SecondShow = v.gameObject;
                            break;
                        case "3rd":
                            ThirdShow = v.gameObject;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private bool _kdShow;
        private bool _aceShow;
        private bool _secondShow;
        private bool _thirdShow;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public bool KdShow { get { return _kdShow; } set {if(_kdShow != value) Set(ref _kdShow, value, "KdShow"); } }
        public bool AceShow { get { return _aceShow; } set {if(_aceShow != value) Set(ref _aceShow, value, "AceShow"); } }
        public bool SecondShow { get { return _secondShow; } set {if(_secondShow != value) Set(ref _secondShow, value, "SecondShow"); } }
        public bool ThirdShow { get { return _thirdShow; } set {if(_thirdShow != value) Set(ref _thirdShow, value, "ThirdShow"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonGameTitleView _view;
		
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

			var view = obj.GetComponent<CommonGameTitleView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonGameTitleView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonGameTitleView, CommonGameTitleViewModel> bindingSet =
                view.CreateBindingSet<CommonGameTitleView, CommonGameTitleViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.oriKdShow = _kdShow = view.KdShow.activeSelf;
            bindingSet.Bind(view.KdShow).For(v => v.activeSelf).To(vm => vm.KdShow).OneWay();
            view.oriAceShow = _aceShow = view.AceShow.activeSelf;
            bindingSet.Bind(view.AceShow).For(v => v.activeSelf).To(vm => vm.AceShow).OneWay();
            view.oriSecondShow = _secondShow = view.SecondShow.activeSelf;
            bindingSet.Bind(view.SecondShow).For(v => v.activeSelf).To(vm => vm.SecondShow).OneWay();
            view.oriThirdShow = _thirdShow = view.ThirdShow.activeSelf;
            bindingSet.Bind(view.ThirdShow).For(v => v.activeSelf).To(vm => vm.ThirdShow).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonGameTitleView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonGameTitleViewModel()
        {
            Type type = typeof(CommonGameTitleViewModel);
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
			KdShow = _view.oriKdShow;
			AceShow = _view.oriAceShow;
			SecondShow = _view.oriSecondShow;
			ThirdShow = _view.oriThirdShow;
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
        public string ResourceAssetName { get { return "CommonGameTitle"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
