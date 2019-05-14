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
    public class CommonSimpleTipViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonSimpleTipView : UIView
        {
            public GameObject ShowGameObjectActiveSelf;
            [HideInInspector]
            public bool oriShowGameObjectActiveSelf;
            public Text Content;
            [HideInInspector]
            public string oriContent;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            ShowGameObjectActiveSelf = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Content":
                            Content = v;
                            break;
                    }
                }

            }
        }


        private bool _showGameObjectActiveSelf;
        private string _content;
        public bool ShowGameObjectActiveSelf { get { return _showGameObjectActiveSelf; } set {if(_showGameObjectActiveSelf != value) Set(ref _showGameObjectActiveSelf, value, "ShowGameObjectActiveSelf"); } }
        public string Content { get { return _content; } set {if(_content != value) Set(ref _content, value, "Content"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonSimpleTipView _view;
		
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
			var view = obj.GetComponent<CommonSimpleTipView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonSimpleTipView>();
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
		private void EventTriggerBind(CommonSimpleTipView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonSimpleTipViewModel()
        {
            Type type = typeof(CommonSimpleTipViewModel);
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

		void ViewBind(CommonSimpleTipView view)
		{
		     BindingSet<CommonSimpleTipView, CommonSimpleTipViewModel> bindingSet =
                view.CreateBindingSet<CommonSimpleTipView, CommonSimpleTipViewModel>();
            bindingSet.Bind(view.ShowGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.ShowGameObjectActiveSelf).OneWay();
            bindingSet.Bind(view.Content).For(v => v.text).To(vm => vm.Content).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonSimpleTipView view)
		{
            _showGameObjectActiveSelf = view.ShowGameObjectActiveSelf.activeSelf;
            _content = view.Content.text;
		}


		void SaveOriData(CommonSimpleTipView view)
		{
            view.oriShowGameObjectActiveSelf = _showGameObjectActiveSelf;
            view.oriContent = _content;
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
			ShowGameObjectActiveSelf = _view.oriShowGameObjectActiveSelf;
			Content = _view.oriContent;
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
        public string ResourceAssetName { get { return "CommonSimpleTip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
