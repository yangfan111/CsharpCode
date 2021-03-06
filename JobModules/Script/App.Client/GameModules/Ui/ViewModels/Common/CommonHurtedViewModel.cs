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
    public class CommonHurtedViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonHurtedView : UIView
        {
            public GameObject hurtedRootActive;
            [HideInInspector]
            public bool orihurtedRootActive;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "hurtedRoot":
                            hurtedRootActive = v.gameObject;
                            break;
                    }
                }

            }
        }


        private bool _hurtedRootActive;
        public bool hurtedRootActive { get { return _hurtedRootActive; } set {if(_hurtedRootActive != value) Set(ref _hurtedRootActive, value, "hurtedRootActive"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonHurtedView _view;
		
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
			var view = obj.GetComponent<CommonHurtedView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonHurtedView>();
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
		private void EventTriggerBind(CommonHurtedView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonHurtedViewModel()
        {
            Type type = typeof(CommonHurtedViewModel);
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

		void ViewBind(CommonHurtedView view)
		{
		     BindingSet<CommonHurtedView, CommonHurtedViewModel> bindingSet =
                view.CreateBindingSet<CommonHurtedView, CommonHurtedViewModel>();
            bindingSet.Bind(view.hurtedRootActive).For(v => v.activeSelf).To(vm => vm.hurtedRootActive).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonHurtedView view)
		{
            _hurtedRootActive = view.hurtedRootActive.activeSelf;
		}


		void SaveOriData(CommonHurtedView view)
		{
            view.orihurtedRootActive = _hurtedRootActive;
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
			hurtedRootActive = _view.orihurtedRootActive;
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
        public string ResourceAssetName { get { return "CommonHurted"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
