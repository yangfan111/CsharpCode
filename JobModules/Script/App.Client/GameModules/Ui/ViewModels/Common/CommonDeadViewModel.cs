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
    public class CommonDeadViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonDeadView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public GameObject ButtonGroupShow;
            [HideInInspector]
            public bool oriButtonGroupShow;
            public Button ContinueBtnClick;
            public Text ContinueBtnText;
            [HideInInspector]
            public string oriContinueBtnText;
            public Button ObserverBtnClick;
            public GameObject MaskGroupShow;
            [HideInInspector]
            public bool oriMaskGroupShow;
            
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
                        case "ButtonGroup":
                            ButtonGroupShow = v.gameObject;
                            break;
                        case "MaskGroup":
                            MaskGroupShow = v.gameObject;
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
                        case "ObserverBtn":
                            ObserverBtnClick = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ContinueBtnText":
                            ContinueBtnText = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private bool _buttonGroupShow;
        private Action _continueBtnClick;
        private string _continueBtnText;
        private Action _observerBtnClick;
        private bool _maskGroupShow;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public bool ButtonGroupShow { get { return _buttonGroupShow; } set {if(_buttonGroupShow != value) Set(ref _buttonGroupShow, value, "ButtonGroupShow"); } }
        public Action ContinueBtnClick { get { return _continueBtnClick; } set {if(_continueBtnClick != value) Set(ref _continueBtnClick, value, "ContinueBtnClick"); } }
        public string ContinueBtnText { get { return _continueBtnText; } set {if(_continueBtnText != value) Set(ref _continueBtnText, value, "ContinueBtnText"); } }
        public Action ObserverBtnClick { get { return _observerBtnClick; } set {if(_observerBtnClick != value) Set(ref _observerBtnClick, value, "ObserverBtnClick"); } }
        public bool MaskGroupShow { get { return _maskGroupShow; } set {if(_maskGroupShow != value) Set(ref _maskGroupShow, value, "MaskGroupShow"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonDeadView _view;
		
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
			var view = obj.GetComponent<CommonDeadView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonDeadView>();
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
		private void EventTriggerBind(CommonDeadView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonDeadViewModel()
        {
            Type type = typeof(CommonDeadViewModel);
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

		void ViewBind(CommonDeadView view)
		{
		     BindingSet<CommonDeadView, CommonDeadViewModel> bindingSet =
                view.CreateBindingSet<CommonDeadView, CommonDeadViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.ButtonGroupShow).For(v => v.activeSelf).To(vm => vm.ButtonGroupShow).OneWay();
            bindingSet.Bind(view.ContinueBtnClick).For(v => v.onClick).To(vm => vm.ContinueBtnClick).OneWay();
            bindingSet.Bind(view.ContinueBtnText).For(v => v.text).To(vm => vm.ContinueBtnText).OneWay();
            bindingSet.Bind(view.ObserverBtnClick).For(v => v.onClick).To(vm => vm.ObserverBtnClick).OneWay();
            bindingSet.Bind(view.MaskGroupShow).For(v => v.activeSelf).To(vm => vm.MaskGroupShow).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonDeadView view)
		{
            _show = view.Show.activeSelf;
            _buttonGroupShow = view.ButtonGroupShow.activeSelf;
            _continueBtnText = view.ContinueBtnText.text;
            _maskGroupShow = view.MaskGroupShow.activeSelf;
		}


		void SaveOriData(CommonDeadView view)
		{
            view.oriShow = _show;
            view.oriButtonGroupShow = _buttonGroupShow;
            view.oriContinueBtnText = _continueBtnText;
            view.oriMaskGroupShow = _maskGroupShow;
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
			ButtonGroupShow = _view.oriButtonGroupShow;
			ContinueBtnText = _view.oriContinueBtnText;
			MaskGroupShow = _view.oriMaskGroupShow;
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
        public string ResourceAssetName { get { return "CommonDead"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
