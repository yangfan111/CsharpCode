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
    public class CommonSplitViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonSplitView : UIView
        {
            public GameObject rootActiveSelf;
            [HideInInspector]
            public bool orirootActiveSelf;
            public Text titleText;
            [HideInInspector]
            public string orititleText;
            public Text leftText;
            [HideInInspector]
            public string orileftText;
            public Text rightText;
            [HideInInspector]
            public string orirightText;
            public Slider SliderValue;
            [HideInInspector]
            public float oriSliderValue;
            public Slider SliderValueChanged;
            public Button splitBtnClick;
            public Button splitBtnInteractable;
            [HideInInspector]
            public bool orisplitBtnInteractable;
            public Button cancelBtnClick;
            public InputField splitNumChanged;
            public InputField splitNumName;
            [HideInInspector]
            public string orisplitNumName;
            
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
                        case "leftText":
                            leftText = v;
                            break;
                        case "rightText":
                            rightText = v;
                            break;
                    }
                }

                Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
                foreach (var v in sliders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Slider":
                            SliderValue = v;
                            SliderValueChanged = v;
                            break;
                    }
                }

                Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
                foreach (var v in buttons)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "splitBtn":
                            splitBtnClick = v;
                            splitBtnInteractable = v;
                            break;
                        case "cancelBtn":
                            cancelBtnClick = v;
                            break;
                    }
                }

                InputField[] inputfields = gameObject.GetComponentsInChildren<InputField>(true);
                foreach (var v in inputfields)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "InputField":
                            splitNumChanged = v;
                            splitNumName = v;
                            break;
                    }
                }

            }
        }


        private bool _rootActiveSelf;
        private string _titleText;
        private string _leftText;
        private string _rightText;
        private float _sliderValue;
        private Action<float> _sliderValueChanged;
        private Action _splitBtnClick;
        private bool _splitBtnInteractable;
        private Action _cancelBtnClick;
        private Action<string> _splitNumChanged;
        private string _splitNumName;
        public bool rootActiveSelf { get { return _rootActiveSelf; } set {if(_rootActiveSelf != value) Set(ref _rootActiveSelf, value, "rootActiveSelf"); } }
        public string titleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "titleText"); } }
        public string leftText { get { return _leftText; } set {if(_leftText != value) Set(ref _leftText, value, "leftText"); } }
        public string rightText { get { return _rightText; } set {if(_rightText != value) Set(ref _rightText, value, "rightText"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float SliderValue { get { return _sliderValue; } set {if(_sliderValue != value) Set(ref _sliderValue, value, "SliderValue"); } }
        public Action<float> SliderValueChanged { get { return _sliderValueChanged; } set {if(_sliderValueChanged != value) Set(ref _sliderValueChanged, value, "SliderValueChanged"); } }
        public Action splitBtnClick { get { return _splitBtnClick; } set {if(_splitBtnClick != value) Set(ref _splitBtnClick, value, "splitBtnClick"); } }
        public bool splitBtnInteractable { get { return _splitBtnInteractable; } set {if(_splitBtnInteractable != value) Set(ref _splitBtnInteractable, value, "splitBtnInteractable"); } }
        public Action cancelBtnClick { get { return _cancelBtnClick; } set {if(_cancelBtnClick != value) Set(ref _cancelBtnClick, value, "cancelBtnClick"); } }
        public Action<string> splitNumChanged { get { return _splitNumChanged; } set {if(_splitNumChanged != value) Set(ref _splitNumChanged, value, "splitNumChanged"); } }
        public string splitNumName { get { return _splitNumName; } set {if(_splitNumName != value) Set(ref _splitNumName, value, "splitNumName"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonSplitView _view;
		
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

			var view = obj.GetComponent<CommonSplitView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonSplitView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonSplitView, CommonSplitViewModel> bindingSet =
                view.CreateBindingSet<CommonSplitView, CommonSplitViewModel>();

            view.orirootActiveSelf = _rootActiveSelf = view.rootActiveSelf.activeSelf;
            bindingSet.Bind(view.rootActiveSelf).For(v => v.activeSelf).To(vm => vm.rootActiveSelf).OneWay();
            view.orititleText = _titleText = view.titleText.text;
            bindingSet.Bind(view.titleText).For(v => v.text).To(vm => vm.titleText).OneWay();
            view.orileftText = _leftText = view.leftText.text;
            bindingSet.Bind(view.leftText).For(v => v.text).To(vm => vm.leftText).OneWay();
            view.orirightText = _rightText = view.rightText.text;
            bindingSet.Bind(view.rightText).For(v => v.text).To(vm => vm.rightText).OneWay();
            view.oriSliderValue = _sliderValue = view.SliderValue.value;
            bindingSet.Bind(view.SliderValue).For(v => v.value).To(vm => vm.SliderValue).OneWay();
            bindingSet.Bind(view.SliderValueChanged).For(v => v.onValueChanged).To(vm => vm.SliderValueChanged).OneWay();
            bindingSet.Bind(view.splitBtnClick).For(v => v.onClick).To(vm => vm.splitBtnClick).OneWay();
            view.orisplitBtnInteractable = _splitBtnInteractable = view.splitBtnInteractable.interactable;
            bindingSet.Bind(view.splitBtnInteractable).For(v => v.interactable).To(vm => vm.splitBtnInteractable).OneWay();
            bindingSet.Bind(view.cancelBtnClick).For(v => v.onClick).To(vm => vm.cancelBtnClick).OneWay();
            bindingSet.Bind(view.splitNumChanged).For(v => v.onValueChanged).To(vm => vm.splitNumChanged).OneWay();
            view.orisplitNumName = _splitNumName = view.splitNumName.text;
            bindingSet.Bind(view.splitNumName).For(v => v.text).To(vm => vm.splitNumName).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonSplitView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonSplitViewModel()
        {
            Type type = typeof(CommonSplitViewModel);
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
			rootActiveSelf = _view.orirootActiveSelf;
			titleText = _view.orititleText;
			leftText = _view.orileftText;
			rightText = _view.orirightText;
			SliderValue = _view.oriSliderValue;
			splitBtnInteractable = _view.orisplitBtnInteractable;
			splitNumName = _view.orisplitNumName;
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
        public string ResourceAssetName { get { return "CommonSplit"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
