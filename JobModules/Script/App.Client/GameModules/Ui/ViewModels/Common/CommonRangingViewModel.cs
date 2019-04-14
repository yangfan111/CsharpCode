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
    public class CommonRangingViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonRangingView : UIView
        {
            public GameObject rootActiveSelf;
            [HideInInspector]
            public bool orirootActiveSelf;
            public Text titleText;
            [HideInInspector]
            public string orititleText;
            public RectTransform UIPos;
            [HideInInspector]
            public Vector3 oriUIPos;
            
            public void FillField()
            {
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

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "title":
                            titleText = v;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "bg":
                            UIPos = v;
                            break;
                    }
                }

            }
        }


        private bool _rootActiveSelf;
        private string _titleText;
        private Vector3 _uIPos;
        public bool rootActiveSelf { get { return _rootActiveSelf; } set {if(_rootActiveSelf != value) Set(ref _rootActiveSelf, value, "rootActiveSelf"); } }
        public string titleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "titleText"); } }
        public Vector3 UIPos { get { return _uIPos; } set {if(_uIPos != value) Set(ref _uIPos, value, "UIPos"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonRangingView _view;
		
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
			var view = obj.GetComponent<CommonRangingView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonRangingView>();
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
		private void EventTriggerBind(CommonRangingView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonRangingViewModel()
        {
            Type type = typeof(CommonRangingViewModel);
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

		void ViewBind(CommonRangingView view)
		{
		     BindingSet<CommonRangingView, CommonRangingViewModel> bindingSet =
                view.CreateBindingSet<CommonRangingView, CommonRangingViewModel>();
            bindingSet.Bind(view.rootActiveSelf).For(v => v.activeSelf).To(vm => vm.rootActiveSelf).OneWay();
            bindingSet.Bind(view.titleText).For(v => v.text).To(vm => vm.titleText).OneWay();
            bindingSet.Bind(view.UIPos).For(v => v.localPosition).To(vm => vm.UIPos).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonRangingView view)
		{
            _rootActiveSelf = view.rootActiveSelf.activeSelf;
            _titleText = view.titleText.text;
            _uIPos = view.UIPos.localPosition;
		}


		void SaveOriData(CommonRangingView view)
		{
            view.orirootActiveSelf = _rootActiveSelf;
            view.orititleText = _titleText;
            view.oriUIPos = _uIPos;
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
			rootActiveSelf = _view.orirootActiveSelf;
			titleText = _view.orititleText;
			UIPos = _view.oriUIPos;
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
        public string ResourceAssetName { get { return "CommonRanging"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
