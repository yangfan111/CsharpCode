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
    public class CommonTaskTipViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonTaskTipView : UIView
        {
            public Text TitleText;
            [HideInInspector]
            public string oriTitleText;
            public Text ScheduleText;
            [HideInInspector]
            public string oriScheduleText;
            public Image ProgressVal;
            [HideInInspector]
            public float oriProgressVal;
            public CanvasGroup Alpha;
            [HideInInspector]
            public float oriAlpha;
            
            public void FillField()
            {
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Title":
                            TitleText = v;
                            break;
                        case "Schedule":
                            ScheduleText = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Progress":
                            ProgressVal = v;
                            break;
                    }
                }

                CanvasGroup[] canvasgroups = gameObject.GetComponentsInChildren<CanvasGroup>(true);
                foreach (var v in canvasgroups)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            Alpha = v;
                            break;
                    }
                }

            }
        }


        private string _titleText;
        private string _scheduleText;
        private float _progressVal;
        private float _alpha;
        public string TitleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "TitleText"); } }
        public string ScheduleText { get { return _scheduleText; } set {if(_scheduleText != value) Set(ref _scheduleText, value, "ScheduleText"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float ProgressVal { get { return _progressVal; } set {if(_progressVal != value) Set(ref _progressVal, value, "ProgressVal"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float Alpha { get { return _alpha; } set {if(_alpha != value) Set(ref _alpha, value, "Alpha"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonTaskTipView _view;
		
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

			var view = obj.GetComponent<CommonTaskTipView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonTaskTipView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonTaskTipView, CommonTaskTipViewModel> bindingSet =
                view.CreateBindingSet<CommonTaskTipView, CommonTaskTipViewModel>();

            view.oriTitleText = _titleText = view.TitleText.text;
            bindingSet.Bind(view.TitleText).For(v => v.text).To(vm => vm.TitleText).OneWay();
            view.oriScheduleText = _scheduleText = view.ScheduleText.text;
            bindingSet.Bind(view.ScheduleText).For(v => v.text).To(vm => vm.ScheduleText).OneWay();
            view.oriProgressVal = _progressVal = view.ProgressVal.fillAmount;
            bindingSet.Bind(view.ProgressVal).For(v => v.fillAmount).To(vm => vm.ProgressVal).OneWay();
            view.oriAlpha = _alpha = view.Alpha.alpha;
            bindingSet.Bind(view.Alpha).For(v => v.alpha).To(vm => vm.Alpha).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonTaskTipView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonTaskTipViewModel()
        {
            Type type = typeof(CommonTaskTipViewModel);
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
			TitleText = _view.oriTitleText;
			ScheduleText = _view.oriScheduleText;
			ProgressVal = _view.oriProgressVal;
			Alpha = _view.oriAlpha;
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
        public string ResourceAssetName { get { return "CommonTaskTip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
