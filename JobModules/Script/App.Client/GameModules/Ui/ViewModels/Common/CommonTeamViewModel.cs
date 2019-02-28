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
    public class CommonTeamViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonTeamView : UIView
        {
            public RectTransform rootLocation;
            [HideInInspector]
            public Vector2 orirootLocation;
            public GameObject rootActiveSelf;
            [HideInInspector]
            public bool orirootActiveSelf;
            
            public void FillField()
            {
                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootLocation = v;
                            break;
                    }
                }

                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootActiveSelf = v.gameObject;
                            break;
                    }
                }

            }
        }


        private Vector2 _rootLocation;
        private bool _rootActiveSelf;
        public Vector2 rootLocation { get { return _rootLocation; } set {if(_rootLocation != value) Set(ref _rootLocation, value, "rootLocation"); } }
        public bool rootActiveSelf { get { return _rootActiveSelf; } set {if(_rootActiveSelf != value) Set(ref _rootActiveSelf, value, "rootActiveSelf"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonTeamView _view;
		
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

			var view = obj.GetComponent<CommonTeamView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonTeamView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonTeamView, CommonTeamViewModel> bindingSet =
                view.CreateBindingSet<CommonTeamView, CommonTeamViewModel>();

            view.orirootLocation = _rootLocation = view.rootLocation.anchoredPosition;
            bindingSet.Bind(view.rootLocation).For(v => v.anchoredPosition).To(vm => vm.rootLocation).OneWay();
            view.orirootActiveSelf = _rootActiveSelf = view.rootActiveSelf.activeSelf;
            bindingSet.Bind(view.rootActiveSelf).For(v => v.activeSelf).To(vm => vm.rootActiveSelf).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonTeamView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonTeamViewModel()
        {
            Type type = typeof(CommonTeamViewModel);
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
			rootLocation = _view.orirootLocation;
			rootActiveSelf = _view.orirootActiveSelf;
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
        public string ResourceAssetName { get { return "CommonTeam"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
