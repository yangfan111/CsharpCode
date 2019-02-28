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

			var view = obj.GetComponent<CommonMCountDownView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonMCountDownView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonMCountDownView, CommonMCountDownViewModel> bindingSet =
                view.CreateBindingSet<CommonMCountDownView, CommonMCountDownViewModel>();

            view.orirootActive = _rootActive = view.rootActive.activeSelf;
            bindingSet.Bind(view.rootActive).For(v => v.activeSelf).To(vm => vm.rootActive).OneWay();
            view.oricountNumText = _countNumText = view.countNumText.text;
            bindingSet.Bind(view.countNumText).For(v => v.text).To(vm => vm.countNumText).OneWay();
            view.orinumBgFillAmount = _numBgFillAmount = view.numBgFillAmount.fillAmount;
            bindingSet.Bind(view.numBgFillAmount).For(v => v.fillAmount).To(vm => vm.numBgFillAmount).OneWay();
            bindingSet.Build();

			SpriteReset();
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

		private void SpriteReset()
		{
		}

		public void Reset()
		{
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

        public string ResourceBundleName { get { return "uiprefabs/common"; } }
        public string ResourceAssetName { get { return "CommonMCountDown"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
