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

namespace App.Client.GameModules.Ui.ViewModels.Group
{
    public class GroupRecordInfoViewModel : ViewModelBase, IUiViewModel
    {
        private class GroupRecordInfoView : UIView
        {
            public GameObject ImgGroupShow;
            [HideInInspector]
            public bool oriImgGroupShow;
            public GameObject TextGroupShow;
            [HideInInspector]
            public bool oriTextGroupShow;
            public GameObject IconGroupShow;
            [HideInInspector]
            public bool oriIconGroupShow;
            public Image MySelfMaskShow;
            [HideInInspector]
            public bool oriMySelfMaskShow;
            public Image DeathMaskShow;
            [HideInInspector]
            public bool oriDeathMaskShow;
            public Image DeadIconShow;
            [HideInInspector]
            public bool oriDeadIconShow;
            public Text RankText;
            [HideInInspector]
            public string oriRankText;
            public Text RankColor;
            [HideInInspector]
            public Color oriRankColor;
            public Text PlayerNameText;
            [HideInInspector]
            public string oriPlayerNameText;
            public Text PlayerNameColor;
            [HideInInspector]
            public Color oriPlayerNameColor;
            public Text CorpsText;
            [HideInInspector]
            public string oriCorpsText;
            public Text CorpsColor;
            [HideInInspector]
            public Color oriCorpsColor;
            public Text KillText;
            [HideInInspector]
            public string oriKillText;
            public Text KillColor;
            [HideInInspector]
            public Color oriKillColor;
            public Text DamageText;
            [HideInInspector]
            public string oriDamageText;
            public Text DamageColor;
            [HideInInspector]
            public Color oriDamageColor;
            public Text DeadText;
            [HideInInspector]
            public string oriDeadText;
            public Text DeadColor;
            [HideInInspector]
            public Color oriDeadColor;
            public Text AssistText;
            [HideInInspector]
            public string oriAssistText;
            public Text AssistColor;
            [HideInInspector]
            public Color oriAssistColor;
            public Text PingText;
            [HideInInspector]
            public string oriPingText;
            public Text PingColor;
            [HideInInspector]
            public Color oriPingColor;
            public GameObject TitleIconShow1;
            public GameObject TitleIconShow2;
            public GameObject TitleIconShow3;
            public GameObject TitleIconShow4;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "ImgGroup":
                            ImgGroupShow = v.gameObject;
                            break;
                        case "TextGroup":
                            TextGroupShow = v.gameObject;
                            break;
                        case "IconGroup":
                            IconGroupShow = v.gameObject;
                            break;
                        case "TitleIcon1":
                            TitleIconShow1 = v.gameObject;
                            break;
                        case "TitleIcon2":
                            TitleIconShow2 = v.gameObject;
                            break;
                        case "TitleIcon3":
                            TitleIconShow3 = v.gameObject;
                            break;
                        case "TitleIcon4":
                            TitleIconShow4 = v.gameObject;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "MySelfMask":
                            MySelfMaskShow = v;
                            break;
                        case "DeathMask":
                            DeathMaskShow = v;
                            break;
                        case "DeadIcon":
                            DeadIconShow = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Rank":
                            RankText = v;
                            RankColor = v;
                            break;
                        case "PlayerName":
                            PlayerNameText = v;
                            PlayerNameColor = v;
                            break;
                        case "Corps":
                            CorpsText = v;
                            CorpsColor = v;
                            break;
                        case "Kill":
                            KillText = v;
                            KillColor = v;
                            break;
                        case "Damage":
                            DamageText = v;
                            DamageColor = v;
                            break;
                        case "Dead":
                            DeadText = v;
                            DeadColor = v;
                            break;
                        case "Assist":
                            AssistText = v;
                            AssistColor = v;
                            break;
                        case "Ping":
                            PingText = v;
                            PingColor = v;
                            break;
                    }
                }

            }
        }


        private bool _imgGroupShow;
        private bool _textGroupShow;
        private bool _iconGroupShow;
        private bool _mySelfMaskShow;
        private bool _deathMaskShow;
        private bool _deadIconShow;
        private string _rankText;
        private Color _rankColor;
        private string _playerNameText;
        private Color _playerNameColor;
        private string _corpsText;
        private Color _corpsColor;
        private string _killText;
        private Color _killColor;
        private string _damageText;
        private Color _damageColor;
        private string _deadText;
        private Color _deadColor;
        private string _assistText;
        private Color _assistColor;
        private string _pingText;
        private Color _pingColor;
        private bool _titleIconShow1;
        private bool _titleIconShow2;
        private bool _titleIconShow3;
        private bool _titleIconShow4;
        public bool ImgGroupShow { get { return _imgGroupShow; } set {if(_imgGroupShow != value) Set(ref _imgGroupShow, value, "ImgGroupShow"); } }
        public bool TextGroupShow { get { return _textGroupShow; } set {if(_textGroupShow != value) Set(ref _textGroupShow, value, "TextGroupShow"); } }
        public bool IconGroupShow { get { return _iconGroupShow; } set {if(_iconGroupShow != value) Set(ref _iconGroupShow, value, "IconGroupShow"); } }
        public bool MySelfMaskShow { get { return _mySelfMaskShow; } set {if(_mySelfMaskShow != value) Set(ref _mySelfMaskShow, value, "MySelfMaskShow"); } }
        public bool DeathMaskShow { get { return _deathMaskShow; } set {if(_deathMaskShow != value) Set(ref _deathMaskShow, value, "DeathMaskShow"); } }
        public bool DeadIconShow { get { return _deadIconShow; } set {if(_deadIconShow != value) Set(ref _deadIconShow, value, "DeadIconShow"); } }
        public string RankText { get { return _rankText; } set {if(_rankText != value) Set(ref _rankText, value, "RankText"); } }
        public Color RankColor { get { return _rankColor; } set {if(_rankColor != value) Set(ref _rankColor, value, "RankColor"); } }
        public string PlayerNameText { get { return _playerNameText; } set {if(_playerNameText != value) Set(ref _playerNameText, value, "PlayerNameText"); } }
        public Color PlayerNameColor { get { return _playerNameColor; } set {if(_playerNameColor != value) Set(ref _playerNameColor, value, "PlayerNameColor"); } }
        public string CorpsText { get { return _corpsText; } set {if(_corpsText != value) Set(ref _corpsText, value, "CorpsText"); } }
        public Color CorpsColor { get { return _corpsColor; } set {if(_corpsColor != value) Set(ref _corpsColor, value, "CorpsColor"); } }
        public string KillText { get { return _killText; } set {if(_killText != value) Set(ref _killText, value, "KillText"); } }
        public Color KillColor { get { return _killColor; } set {if(_killColor != value) Set(ref _killColor, value, "KillColor"); } }
        public string DamageText { get { return _damageText; } set {if(_damageText != value) Set(ref _damageText, value, "DamageText"); } }
        public Color DamageColor { get { return _damageColor; } set {if(_damageColor != value) Set(ref _damageColor, value, "DamageColor"); } }
        public string DeadText { get { return _deadText; } set {if(_deadText != value) Set(ref _deadText, value, "DeadText"); } }
        public Color DeadColor { get { return _deadColor; } set {if(_deadColor != value) Set(ref _deadColor, value, "DeadColor"); } }
        public string AssistText { get { return _assistText; } set {if(_assistText != value) Set(ref _assistText, value, "AssistText"); } }
        public Color AssistColor { get { return _assistColor; } set {if(_assistColor != value) Set(ref _assistColor, value, "AssistColor"); } }
        public string PingText { get { return _pingText; } set {if(_pingText != value) Set(ref _pingText, value, "PingText"); } }
        public Color PingColor { get { return _pingColor; } set {if(_pingColor != value) Set(ref _pingColor, value, "PingColor"); } }
        public bool TitleIconShow1 { get { return _titleIconShow1; } set {if(_titleIconShow1 != value) Set(ref _titleIconShow1, value, "TitleIconShow1"); } }
        public bool TitleIconShow2 { get { return _titleIconShow2; } set {if(_titleIconShow2 != value) Set(ref _titleIconShow2, value, "TitleIconShow2"); } }
        public bool TitleIconShow3 { get { return _titleIconShow3; } set {if(_titleIconShow3 != value) Set(ref _titleIconShow3, value, "TitleIconShow3"); } }
        public bool TitleIconShow4 { get { return _titleIconShow4; } set {if(_titleIconShow4 != value) Set(ref _titleIconShow4, value, "TitleIconShow4"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private GroupRecordInfoView _view;
		
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
			var view = obj.GetComponent<GroupRecordInfoView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<GroupRecordInfoView>();
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
		private void EventTriggerBind(GroupRecordInfoView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static GroupRecordInfoViewModel()
        {
            Type type = typeof(GroupRecordInfoViewModel);
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

		void ViewBind(GroupRecordInfoView view)
		{
		     BindingSet<GroupRecordInfoView, GroupRecordInfoViewModel> bindingSet =
                view.CreateBindingSet<GroupRecordInfoView, GroupRecordInfoViewModel>();
            bindingSet.Bind(view.ImgGroupShow).For(v => v.activeSelf).To(vm => vm.ImgGroupShow).OneWay();
            bindingSet.Bind(view.TextGroupShow).For(v => v.activeSelf).To(vm => vm.TextGroupShow).OneWay();
            bindingSet.Bind(view.IconGroupShow).For(v => v.activeSelf).To(vm => vm.IconGroupShow).OneWay();
            bindingSet.Bind(view.MySelfMaskShow).For(v => v.enabled).To(vm => vm.MySelfMaskShow).OneWay();
            bindingSet.Bind(view.DeathMaskShow).For(v => v.enabled).To(vm => vm.DeathMaskShow).OneWay();
            bindingSet.Bind(view.DeadIconShow).For(v => v.enabled).To(vm => vm.DeadIconShow).OneWay();
            bindingSet.Bind(view.RankText).For(v => v.text).To(vm => vm.RankText).OneWay();
            bindingSet.Bind(view.RankColor).For(v => v.color).To(vm => vm.RankColor).OneWay();
            bindingSet.Bind(view.PlayerNameText).For(v => v.text).To(vm => vm.PlayerNameText).OneWay();
            bindingSet.Bind(view.PlayerNameColor).For(v => v.color).To(vm => vm.PlayerNameColor).OneWay();
            bindingSet.Bind(view.CorpsText).For(v => v.text).To(vm => vm.CorpsText).OneWay();
            bindingSet.Bind(view.CorpsColor).For(v => v.color).To(vm => vm.CorpsColor).OneWay();
            bindingSet.Bind(view.KillText).For(v => v.text).To(vm => vm.KillText).OneWay();
            bindingSet.Bind(view.KillColor).For(v => v.color).To(vm => vm.KillColor).OneWay();
            bindingSet.Bind(view.DamageText).For(v => v.text).To(vm => vm.DamageText).OneWay();
            bindingSet.Bind(view.DamageColor).For(v => v.color).To(vm => vm.DamageColor).OneWay();
            bindingSet.Bind(view.DeadText).For(v => v.text).To(vm => vm.DeadText).OneWay();
            bindingSet.Bind(view.DeadColor).For(v => v.color).To(vm => vm.DeadColor).OneWay();
            bindingSet.Bind(view.AssistText).For(v => v.text).To(vm => vm.AssistText).OneWay();
            bindingSet.Bind(view.AssistColor).For(v => v.color).To(vm => vm.AssistColor).OneWay();
            bindingSet.Bind(view.PingText).For(v => v.text).To(vm => vm.PingText).OneWay();
            bindingSet.Bind(view.PingColor).For(v => v.color).To(vm => vm.PingColor).OneWay();
            bindingSet.Bind(view.TitleIconShow1).For(v => v.activeSelf).To(vm => vm.TitleIconShow1).OneWay();
            bindingSet.Bind(view.TitleIconShow2).For(v => v.activeSelf).To(vm => vm.TitleIconShow2).OneWay();
            bindingSet.Bind(view.TitleIconShow3).For(v => v.activeSelf).To(vm => vm.TitleIconShow3).OneWay();
            bindingSet.Bind(view.TitleIconShow4).For(v => v.activeSelf).To(vm => vm.TitleIconShow4).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(GroupRecordInfoView view)
		{
            _imgGroupShow = view.ImgGroupShow.activeSelf;
            _textGroupShow = view.TextGroupShow.activeSelf;
            _iconGroupShow = view.IconGroupShow.activeSelf;
            _mySelfMaskShow = view.MySelfMaskShow.enabled;
            _deathMaskShow = view.DeathMaskShow.enabled;
            _deadIconShow = view.DeadIconShow.enabled;
            _rankText = view.RankText.text;
            _rankColor = view.RankColor.color;
            _playerNameText = view.PlayerNameText.text;
            _playerNameColor = view.PlayerNameColor.color;
            _corpsText = view.CorpsText.text;
            _corpsColor = view.CorpsColor.color;
            _killText = view.KillText.text;
            _killColor = view.KillColor.color;
            _damageText = view.DamageText.text;
            _damageColor = view.DamageColor.color;
            _deadText = view.DeadText.text;
            _deadColor = view.DeadColor.color;
            _assistText = view.AssistText.text;
            _assistColor = view.AssistColor.color;
            _pingText = view.PingText.text;
            _pingColor = view.PingColor.color;
		}


		void SaveOriData(GroupRecordInfoView view)
		{
            view.oriImgGroupShow = _imgGroupShow;
            view.oriTextGroupShow = _textGroupShow;
            view.oriIconGroupShow = _iconGroupShow;
            view.oriMySelfMaskShow = _mySelfMaskShow;
            view.oriDeathMaskShow = _deathMaskShow;
            view.oriDeadIconShow = _deadIconShow;
            view.oriRankText = _rankText;
            view.oriRankColor = _rankColor;
            view.oriPlayerNameText = _playerNameText;
            view.oriPlayerNameColor = _playerNameColor;
            view.oriCorpsText = _corpsText;
            view.oriCorpsColor = _corpsColor;
            view.oriKillText = _killText;
            view.oriKillColor = _killColor;
            view.oriDamageText = _damageText;
            view.oriDamageColor = _damageColor;
            view.oriDeadText = _deadText;
            view.oriDeadColor = _deadColor;
            view.oriAssistText = _assistText;
            view.oriAssistColor = _assistColor;
            view.oriPingText = _pingText;
            view.oriPingColor = _pingColor;
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
			ImgGroupShow = _view.oriImgGroupShow;
			TextGroupShow = _view.oriTextGroupShow;
			IconGroupShow = _view.oriIconGroupShow;
			MySelfMaskShow = _view.oriMySelfMaskShow;
			DeathMaskShow = _view.oriDeathMaskShow;
			DeadIconShow = _view.oriDeadIconShow;
			RankText = _view.oriRankText;
			RankColor = _view.oriRankColor;
			PlayerNameText = _view.oriPlayerNameText;
			PlayerNameColor = _view.oriPlayerNameColor;
			CorpsText = _view.oriCorpsText;
			CorpsColor = _view.oriCorpsColor;
			KillText = _view.oriKillText;
			KillColor = _view.oriKillColor;
			DamageText = _view.oriDamageText;
			DamageColor = _view.oriDamageColor;
			DeadText = _view.oriDeadText;
			DeadColor = _view.oriDeadColor;
			AssistText = _view.oriAssistText;
			AssistColor = _view.oriAssistColor;
			PingText = _view.oriPingText;
			PingColor = _view.oriPingColor;
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

        public const string TitleIconShow = "TitleIconShow";
        public const int TitleIconShowCount = 4;
        public bool SetTitleIconShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		TitleIconShow1 = val;
        		break;
        	case 2:
        		TitleIconShow2 = val;
        		break;
        	case 3:
        		TitleIconShow3 = val;
        		break;
        	case 4:
        		TitleIconShow4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetTitleIconShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return TitleIconShow1;
        	case 2:
        		return TitleIconShow2;
        	case 3:
        		return TitleIconShow3;
        	case 4:
        		return TitleIconShow4;
        	default:
        		return default(bool);
        	}
        }
        public string ResourceBundleName { get { return "ui/client/prefab/group"; } }
        public string ResourceAssetName { get { return "GroupRecordInfo"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
