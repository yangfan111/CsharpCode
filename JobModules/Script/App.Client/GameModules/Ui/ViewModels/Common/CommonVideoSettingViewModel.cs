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
    public class CommonVideoSettingViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonVideoSettingView : UIView
        {
            public Button CloseBtnClick;
            public Button initBtnClick;
            public Button cancelBtnClick;
            public Button applyBtnClick;
            
            public void FillField()
            {
                Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
                foreach (var v in buttons)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "CloseBtn":
                            CloseBtnClick = v;
                            break;
                        case "initBtn":
                            initBtnClick = v;
                            break;
                        case "cancelBtn":
                            cancelBtnClick = v;
                            break;
                        case "applyBtn":
                            applyBtnClick = v;
                            break;
                    }
                }

            }
        }


        private Action _closeBtnClick;
        private Action _initBtnClick;
        private Action _cancelBtnClick;
        private Action _applyBtnClick;
        public Action CloseBtnClick { get { return _closeBtnClick; } set {if(_closeBtnClick != value) Set(ref _closeBtnClick, value, "CloseBtnClick"); } }
        public Action initBtnClick { get { return _initBtnClick; } set {if(_initBtnClick != value) Set(ref _initBtnClick, value, "initBtnClick"); } }
        public Action cancelBtnClick { get { return _cancelBtnClick; } set {if(_cancelBtnClick != value) Set(ref _cancelBtnClick, value, "cancelBtnClick"); } }
        public Action applyBtnClick { get { return _applyBtnClick; } set {if(_applyBtnClick != value) Set(ref _applyBtnClick, value, "applyBtnClick"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
				
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
			var view = obj.GetComponent<CommonVideoSettingView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonVideoSettingView>();
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

        }
		private void EventTriggerBind(CommonVideoSettingView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonVideoSettingViewModel()
        {
            Type type = typeof(CommonVideoSettingViewModel);
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

		void ViewBind(CommonVideoSettingView view)
		{
		     BindingSet<CommonVideoSettingView, CommonVideoSettingViewModel> bindingSet =
                view.CreateBindingSet<CommonVideoSettingView, CommonVideoSettingViewModel>();
            bindingSet.Bind(view.CloseBtnClick).For(v => v.onClick).To(vm => vm.CloseBtnClick).OneWay();
            bindingSet.Bind(view.initBtnClick).For(v => v.onClick).To(vm => vm.initBtnClick).OneWay();
            bindingSet.Bind(view.cancelBtnClick).For(v => v.onClick).To(vm => vm.cancelBtnClick).OneWay();
            bindingSet.Bind(view.applyBtnClick).For(v => v.onClick).To(vm => vm.applyBtnClick).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonVideoSettingView view)
		{
		}


		void SaveOriData(CommonVideoSettingView view)
		{
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

        public string ResourceBundleName { get { return "ui/hall/prefabs/setting"; } }
        public string ResourceAssetName { get { return "VideoSetting"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
