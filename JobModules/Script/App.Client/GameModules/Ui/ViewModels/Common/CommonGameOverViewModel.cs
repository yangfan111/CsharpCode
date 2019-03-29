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
    public class CommonGameOverViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonGameOverView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Button ContinueBtnClick;
            public GameObject ContinueBtnShow;
            [HideInInspector]
            public bool oriContinueBtnShow;
            
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
                        case "ContinueBtn":
                            ContinueBtnShow = v.gameObject;
                            break;
                    }
                }

                Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
                foreach (var v in buttons)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ContinueBtn":
                            ContinueBtnClick = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private Action _continueBtnClick;
        private bool _continueBtnShow;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public Action ContinueBtnClick { get { return _continueBtnClick; } set {if(_continueBtnClick != value) Set(ref _continueBtnClick, value, "ContinueBtnClick"); } }
        public bool ContinueBtnShow { get { return _continueBtnShow; } set {if(_continueBtnShow != value) Set(ref _continueBtnShow, value, "ContinueBtnShow"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonGameOverView _view;
		
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

			var view = obj.GetComponent<CommonGameOverView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonGameOverView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonGameOverView, CommonGameOverViewModel> bindingSet =
                view.CreateBindingSet<CommonGameOverView, CommonGameOverViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.ContinueBtnClick).For(v => v.onClick).To(vm => vm.ContinueBtnClick).OneWay();
            view.oriContinueBtnShow = _continueBtnShow = view.ContinueBtnShow.activeSelf;
            bindingSet.Bind(view.ContinueBtnShow).For(v => v.activeSelf).To(vm => vm.ContinueBtnShow).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonGameOverView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonGameOverViewModel()
        {
            Type type = typeof(CommonGameOverViewModel);
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
			ContinueBtnShow = _view.oriContinueBtnShow;
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
        public string ResourceAssetName { get { return "CommonGameOver"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
