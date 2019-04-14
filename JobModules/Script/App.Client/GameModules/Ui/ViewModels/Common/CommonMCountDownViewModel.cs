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
    public class CommonMCountDownViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonMCountDownView : UIView
        {
            public GameObject rootActive;
            [HideInInspector]
            public bool orirootActive;
            public Text countNumText;
            [HideInInspector]
            public string oricountNumText;
            public Image numBgFillAmount;
            [HideInInspector]
            public float orinumBgFillAmount;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootActive = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "countNum":
                            countNumText = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "numBg":
                            numBgFillAmount = v;
                            break;
                    }
                }

            }
        }


        private bool _rootActive;
        private string _countNumText;
        private float _numBgFillAmount;
        public bool rootActive { get { return _rootActive; } set {if(_rootActive != value) Set(ref _rootActive, value, "rootActive"); } }
        public string countNumText { get { return _countNumText; } set {if(_countNumText != value) Set(ref _countNumText, value, "countNumText"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float numBgFillAmount { get { return _numBgFillAmount; } set {if(_numBgFillAmount != value) Set(ref _numBgFillAmount, value, "numBgFillAmount"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMCountDownView _view;
		
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
			var view = obj.GetComponent<CommonMCountDownView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonMCountDownView>();
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
		private void EventTriggerBind(CommonMCountDownView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonMCountDownViewModel()
        {
            Type type = typeof(CommonMCountDownViewModel);
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

		void ViewBind(CommonMCountDownView view)
		{
		     BindingSet<CommonMCountDownView, CommonMCountDownViewModel> bindingSet =
                view.CreateBindingSet<CommonMCountDownView, CommonMCountDownViewModel>();
            bindingSet.Bind(view.rootActive).For(v => v.activeSelf).To(vm => vm.rootActive).OneWay();
            bindingSet.Bind(view.countNumText).For(v => v.text).To(vm => vm.countNumText).OneWay();
            bindingSet.Bind(view.numBgFillAmount).For(v => v.fillAmount).To(vm => vm.numBgFillAmount).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonMCountDownView view)
		{
            _rootActive = view.rootActive.activeSelf;
            _countNumText = view.countNumText.text;
            _numBgFillAmount = view.numBgFillAmount.fillAmount;
		}


		void SaveOriData(CommonMCountDownView view)
		{
            view.orirootActive = _rootActive;
            view.oricountNumText = _countNumText;
            view.orinumBgFillAmount = _numBgFillAmount;
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
			rootActive = _view.orirootActive;
			countNumText = _view.oricountNumText;
			numBgFillAmount = _view.orinumBgFillAmount;
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
        public string ResourceAssetName { get { return "CommonMCountDown"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
