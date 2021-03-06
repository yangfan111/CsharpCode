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
    public class CommonChatViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonChatView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public CanvasGroup Alpha;
            [HideInInspector]
            public float oriAlpha;
            public GameObject SendMessageGroupShow;
            [HideInInspector]
            public bool oriSendMessageGroupShow;
            public Text ChannelTipText;
            [HideInInspector]
            public string oriChannelTipText;
            public Text ChannelTipColor;
            [HideInInspector]
            public Color oriChannelTipColor;
            public InputField InputValueChanged;
            public RectTransform InputSize;
            [HideInInspector]
            public Vector2 oriInputSize;
            public Text PrivateNameText;
            [HideInInspector]
            public string oriPrivateNameText;
            public GameObject PrivateNameShow;
            [HideInInspector]
            public bool oriPrivateNameShow;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Root":
                            Show = v.gameObject;
                            break;
                        case "SendMessageGroup":
                            SendMessageGroupShow = v.gameObject;
                            break;
                        case "PrivateName":
                            PrivateNameShow = v.gameObject;
                            break;
                    }
                }

                CanvasGroup[] canvasgroups = gameObject.GetComponentsInChildren<CanvasGroup>(true);
                foreach (var v in canvasgroups)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Root":
                            Alpha = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ChannelTip":
                            ChannelTipText = v;
                            ChannelTipColor = v;
                            break;
                        case "PrivateName":
                            PrivateNameText = v;
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
                            InputValueChanged = v;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "InputField":
                            InputSize = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private float _alpha;
        private bool _sendMessageGroupShow;
        private string _channelTipText;
        private Color _channelTipColor;
        private Action<string> _inputValueChanged;
        private Vector2 _inputSize;
        private string _privateNameText;
        private bool _privateNameShow;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float Alpha { get { return _alpha; } set {if(_alpha != value) Set(ref _alpha, value, "Alpha"); } }
        public bool SendMessageGroupShow { get { return _sendMessageGroupShow; } set {if(_sendMessageGroupShow != value) Set(ref _sendMessageGroupShow, value, "SendMessageGroupShow"); } }
        public string ChannelTipText { get { return _channelTipText; } set {if(_channelTipText != value) Set(ref _channelTipText, value, "ChannelTipText"); } }
        public Color ChannelTipColor { get { return _channelTipColor; } set {if(_channelTipColor != value) Set(ref _channelTipColor, value, "ChannelTipColor"); } }
        public Action<string> InputValueChanged { get { return _inputValueChanged; } set {if(_inputValueChanged != value) Set(ref _inputValueChanged, value, "InputValueChanged"); } }
        public Vector2 InputSize { get { return _inputSize; } set {if(_inputSize != value) Set(ref _inputSize, value, "InputSize"); } }
        public string PrivateNameText { get { return _privateNameText; } set {if(_privateNameText != value) Set(ref _privateNameText, value, "PrivateNameText"); } }
        public bool PrivateNameShow { get { return _privateNameShow; } set {if(_privateNameShow != value) Set(ref _privateNameShow, value, "PrivateNameShow"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonChatView _view;
		
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
			var view = obj.GetComponent<CommonChatView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonChatView>();
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
		private void EventTriggerBind(CommonChatView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonChatViewModel()
        {
            Type type = typeof(CommonChatViewModel);
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

		void ViewBind(CommonChatView view)
		{
		     BindingSet<CommonChatView, CommonChatViewModel> bindingSet =
                view.CreateBindingSet<CommonChatView, CommonChatViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.Alpha).For(v => v.alpha).To(vm => vm.Alpha).OneWay();
            bindingSet.Bind(view.SendMessageGroupShow).For(v => v.activeSelf).To(vm => vm.SendMessageGroupShow).OneWay();
            bindingSet.Bind(view.ChannelTipText).For(v => v.text).To(vm => vm.ChannelTipText).OneWay();
            bindingSet.Bind(view.ChannelTipColor).For(v => v.color).To(vm => vm.ChannelTipColor).OneWay();
            bindingSet.Bind(view.InputValueChanged).For(v => v.onValueChanged).To(vm => vm.InputValueChanged).OneWay();
            bindingSet.Bind(view.InputSize).For(v => v.sizeDelta).To(vm => vm.InputSize).OneWay();
            bindingSet.Bind(view.PrivateNameText).For(v => v.text).To(vm => vm.PrivateNameText).OneWay();
            bindingSet.Bind(view.PrivateNameShow).For(v => v.activeSelf).To(vm => vm.PrivateNameShow).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonChatView view)
		{
            _show = view.Show.activeSelf;
            _alpha = view.Alpha.alpha;
            _sendMessageGroupShow = view.SendMessageGroupShow.activeSelf;
            _channelTipText = view.ChannelTipText.text;
            _channelTipColor = view.ChannelTipColor.color;
            _inputSize = view.InputSize.sizeDelta;
            _privateNameText = view.PrivateNameText.text;
            _privateNameShow = view.PrivateNameShow.activeSelf;
		}


		void SaveOriData(CommonChatView view)
		{
            view.oriShow = _show;
            view.oriAlpha = _alpha;
            view.oriSendMessageGroupShow = _sendMessageGroupShow;
            view.oriChannelTipText = _channelTipText;
            view.oriChannelTipColor = _channelTipColor;
            view.oriInputSize = _inputSize;
            view.oriPrivateNameText = _privateNameText;
            view.oriPrivateNameShow = _privateNameShow;
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
			Alpha = _view.oriAlpha;
			SendMessageGroupShow = _view.oriSendMessageGroupShow;
			ChannelTipText = _view.oriChannelTipText;
			ChannelTipColor = _view.oriChannelTipColor;
			InputSize = _view.oriInputSize;
			PrivateNameText = _view.oriPrivateNameText;
			PrivateNameShow = _view.oriPrivateNameShow;
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
        public string ResourceAssetName { get { return "CommonChat"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
