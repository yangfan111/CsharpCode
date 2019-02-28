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
            public Image ChatListBgShow;
            [HideInInspector]
            public bool oriChatListBgShow;
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

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ChatList":
                            ChatListBgShow = v;
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

            }
        }


        private bool _show;
        private float _alpha;
        private bool _chatListBgShow;
        private bool _sendMessageGroupShow;
        private string _channelTipText;
        private Color _channelTipColor;
        private Action<string> _inputValueChanged;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float Alpha { get { return _alpha; } set {if(_alpha != value) Set(ref _alpha, value, "Alpha"); } }
        public bool ChatListBgShow { get { return _chatListBgShow; } set {if(_chatListBgShow != value) Set(ref _chatListBgShow, value, "ChatListBgShow"); } }
        public bool SendMessageGroupShow { get { return _sendMessageGroupShow; } set {if(_sendMessageGroupShow != value) Set(ref _sendMessageGroupShow, value, "SendMessageGroupShow"); } }
        public string ChannelTipText { get { return _channelTipText; } set {if(_channelTipText != value) Set(ref _channelTipText, value, "ChannelTipText"); } }
        public Color ChannelTipColor { get { return _channelTipColor; } set {if(_channelTipColor != value) Set(ref _channelTipColor, value, "ChannelTipColor"); } }
        public Action<string> InputValueChanged { get { return _inputValueChanged; } set {if(_inputValueChanged != value) Set(ref _inputValueChanged, value, "InputValueChanged"); } }

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

			var view = obj.GetComponent<CommonChatView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonChatView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonChatView, CommonChatViewModel> bindingSet =
                view.CreateBindingSet<CommonChatView, CommonChatViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.oriAlpha = _alpha = view.Alpha.alpha;
            bindingSet.Bind(view.Alpha).For(v => v.alpha).To(vm => vm.Alpha).OneWay();
            view.oriChatListBgShow = _chatListBgShow = view.ChatListBgShow.enabled;
            bindingSet.Bind(view.ChatListBgShow).For(v => v.enabled).To(vm => vm.ChatListBgShow).OneWay();
            view.oriSendMessageGroupShow = _sendMessageGroupShow = view.SendMessageGroupShow.activeSelf;
            bindingSet.Bind(view.SendMessageGroupShow).For(v => v.activeSelf).To(vm => vm.SendMessageGroupShow).OneWay();
            view.oriChannelTipText = _channelTipText = view.ChannelTipText.text;
            bindingSet.Bind(view.ChannelTipText).For(v => v.text).To(vm => vm.ChannelTipText).OneWay();
            view.oriChannelTipColor = _channelTipColor = view.ChannelTipColor.color;
            bindingSet.Bind(view.ChannelTipColor).For(v => v.color).To(vm => vm.ChannelTipColor).OneWay();
            bindingSet.Bind(view.InputValueChanged).For(v => v.onValueChanged).To(vm => vm.InputValueChanged).OneWay();
            bindingSet.Build();

			SpriteReset();
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

		private void SpriteReset()
		{
		}

		public void Reset()
		{
			Show = _view.oriShow;
			Alpha = _view.oriAlpha;
			ChatListBgShow = _view.oriChatListBgShow;
			SendMessageGroupShow = _view.oriSendMessageGroupShow;
			ChannelTipText = _view.oriChannelTipText;
			ChannelTipColor = _view.oriChannelTipColor;
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
        public string ResourceAssetName { get { return "CommonChat"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
