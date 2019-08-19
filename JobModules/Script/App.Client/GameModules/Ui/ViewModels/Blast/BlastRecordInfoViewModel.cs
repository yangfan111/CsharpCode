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

namespace App.Client.GameModules.Ui.ViewModels.Blast
{
    public class BlastRecordInfoViewModel : ViewModelBase, IUiViewModel
    {
        private class BlastRecordInfoView : UIView
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
            public GameObject BadgeGroupShow;
            [HideInInspector]
            public bool oriBadgeGroupShow;
            public Image MySelfMaskShow;
            [HideInInspector]
            public bool oriMySelfMaskShow;
            public Text DeadStateShow;
            [HideInInspector]
            public bool oriDeadStateShow;
            public Text HurtStateShow;
            [HideInInspector]
            public bool oriHurtStateShow;
            public Image BombIconShow;
            [HideInInspector]
            public bool oriBombIconShow;
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
            public Text BombText;
            [HideInInspector]
            public string oriBombText;
            public Text BombColor;
            [HideInInspector]
            public Color oriBombColor;
            public Image BadgeNormalBgShow;
            [HideInInspector]
            public bool oriBadgeNormalBgShow;
            public Image BadgeMySelfBgShow;
            [HideInInspector]
            public bool oriBadgeMySelfBgShow;
            public Image BadgeHurtBgShow;
            [HideInInspector]
            public bool oriBadgeHurtBgShow;
            public Image BadgeDeadBgShow;
            [HideInInspector]
            public bool oriBadgeDeadBgShow;
            public UIImageLoader BadgeIconBundle;
            [HideInInspector]
            public string oriBadgeIconBundle;
            public UIImageLoader BadgeIconAsset;
            [HideInInspector]
            public string oriBadgeIconAsset;
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
                        case "BadgeGroup":
                            BadgeGroupShow = v.gameObject;
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
                        case "BombIcon":
                            BombIconShow = v;
                            break;
                        case "BadgeNormalBg":
                            BadgeNormalBgShow = v;
                            break;
                        case "BadgeMySelfBg":
                            BadgeMySelfBgShow = v;
                            break;
                        case "BadgeHurtBg":
                            BadgeHurtBgShow = v;
                            break;
                        case "BadgeDeadBg":
                            BadgeDeadBgShow = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "DeadStateText":
                            DeadStateShow = v;
                            break;
                        case "HurtStateText":
                            HurtStateShow = v;
                            break;
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
                        case "Bomb":
                            BombText = v;
                            BombColor = v;
                            break;
                    }
                }

                UIImageLoader[] uiimageloaders = gameObject.GetComponentsInChildren<UIImageLoader>(true);
                foreach (var v in uiimageloaders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BadgeIcon":
                            BadgeIconBundle = v;
                            BadgeIconAsset = v;
                            break;
                    }
                }

            }
        }


        private bool _imgGroupShow;
        private bool _textGroupShow;
        private bool _iconGroupShow;
        private bool _badgeGroupShow;
        private bool _mySelfMaskShow;
        private bool _deadStateShow;
        private bool _hurtStateShow;
        private bool _bombIconShow;
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
        private string _bombText;
        private Color _bombColor;
        private bool _badgeNormalBgShow;
        private bool _badgeMySelfBgShow;
        private bool _badgeHurtBgShow;
        private bool _badgeDeadBgShow;
        private string _badgeIconBundle;
        private string _badgeIconAsset;
        private bool _titleIconShow1;
        private bool _titleIconShow2;
        private bool _titleIconShow3;
        private bool _titleIconShow4;
        public bool ImgGroupShow { get { return _imgGroupShow; } set {if(_imgGroupShow != value) Set(ref _imgGroupShow, value, "ImgGroupShow"); } }
        public bool TextGroupShow { get { return _textGroupShow; } set {if(_textGroupShow != value) Set(ref _textGroupShow, value, "TextGroupShow"); } }
        public bool IconGroupShow { get { return _iconGroupShow; } set {if(_iconGroupShow != value) Set(ref _iconGroupShow, value, "IconGroupShow"); } }
        public bool BadgeGroupShow { get { return _badgeGroupShow; } set {if(_badgeGroupShow != value) Set(ref _badgeGroupShow, value, "BadgeGroupShow"); } }
        public bool MySelfMaskShow { get { return _mySelfMaskShow; } set {if(_mySelfMaskShow != value) Set(ref _mySelfMaskShow, value, "MySelfMaskShow"); } }
        public bool DeadStateShow { get { return _deadStateShow; } set {if(_deadStateShow != value) Set(ref _deadStateShow, value, "DeadStateShow"); } }
        public bool HurtStateShow { get { return _hurtStateShow; } set {if(_hurtStateShow != value) Set(ref _hurtStateShow, value, "HurtStateShow"); } }
        public bool BombIconShow { get { return _bombIconShow; } set {if(_bombIconShow != value) Set(ref _bombIconShow, value, "BombIconShow"); } }
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
        public string BombText { get { return _bombText; } set {if(_bombText != value) Set(ref _bombText, value, "BombText"); } }
        public Color BombColor { get { return _bombColor; } set {if(_bombColor != value) Set(ref _bombColor, value, "BombColor"); } }
        public bool BadgeNormalBgShow { get { return _badgeNormalBgShow; } set {if(_badgeNormalBgShow != value) Set(ref _badgeNormalBgShow, value, "BadgeNormalBgShow"); } }
        public bool BadgeMySelfBgShow { get { return _badgeMySelfBgShow; } set {if(_badgeMySelfBgShow != value) Set(ref _badgeMySelfBgShow, value, "BadgeMySelfBgShow"); } }
        public bool BadgeHurtBgShow { get { return _badgeHurtBgShow; } set {if(_badgeHurtBgShow != value) Set(ref _badgeHurtBgShow, value, "BadgeHurtBgShow"); } }
        public bool BadgeDeadBgShow { get { return _badgeDeadBgShow; } set {if(_badgeDeadBgShow != value) Set(ref _badgeDeadBgShow, value, "BadgeDeadBgShow"); } }
        public string BadgeIconBundle { get { return _badgeIconBundle; } set {if(_badgeIconBundle != value) Set(ref _badgeIconBundle, value, "BadgeIconBundle"); } }
        public string BadgeIconAsset { get { return _badgeIconAsset; } set {if(_badgeIconAsset != value) Set(ref _badgeIconAsset, value, "BadgeIconAsset"); } }
        public bool TitleIconShow1 { get { return _titleIconShow1; } set {if(_titleIconShow1 != value) Set(ref _titleIconShow1, value, "TitleIconShow1"); } }
        public bool TitleIconShow2 { get { return _titleIconShow2; } set {if(_titleIconShow2 != value) Set(ref _titleIconShow2, value, "TitleIconShow2"); } }
        public bool TitleIconShow3 { get { return _titleIconShow3; } set {if(_titleIconShow3 != value) Set(ref _titleIconShow3, value, "TitleIconShow3"); } }
        public bool TitleIconShow4 { get { return _titleIconShow4; } set {if(_titleIconShow4 != value) Set(ref _titleIconShow4, value, "TitleIconShow4"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private BlastRecordInfoView _view;
		
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
			var view = obj.GetComponent<BlastRecordInfoView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<BlastRecordInfoView>();
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
		private void EventTriggerBind(BlastRecordInfoView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static BlastRecordInfoViewModel()
        {
            Type type = typeof(BlastRecordInfoViewModel);
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

		void ViewBind(BlastRecordInfoView view)
		{
		     BindingSet<BlastRecordInfoView, BlastRecordInfoViewModel> bindingSet =
                view.CreateBindingSet<BlastRecordInfoView, BlastRecordInfoViewModel>();
            bindingSet.Bind(view.ImgGroupShow).For(v => v.activeSelf).To(vm => vm.ImgGroupShow).OneWay();
            bindingSet.Bind(view.TextGroupShow).For(v => v.activeSelf).To(vm => vm.TextGroupShow).OneWay();
            bindingSet.Bind(view.IconGroupShow).For(v => v.activeSelf).To(vm => vm.IconGroupShow).OneWay();
            bindingSet.Bind(view.BadgeGroupShow).For(v => v.activeSelf).To(vm => vm.BadgeGroupShow).OneWay();
            bindingSet.Bind(view.MySelfMaskShow).For(v => v.enabled).To(vm => vm.MySelfMaskShow).OneWay();
            bindingSet.Bind(view.DeadStateShow).For(v => v.enabled).To(vm => vm.DeadStateShow).OneWay();
            bindingSet.Bind(view.HurtStateShow).For(v => v.enabled).To(vm => vm.HurtStateShow).OneWay();
            bindingSet.Bind(view.BombIconShow).For(v => v.enabled).To(vm => vm.BombIconShow).OneWay();
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
            bindingSet.Bind(view.BombText).For(v => v.text).To(vm => vm.BombText).OneWay();
            bindingSet.Bind(view.BombColor).For(v => v.color).To(vm => vm.BombColor).OneWay();
            bindingSet.Bind(view.BadgeNormalBgShow).For(v => v.enabled).To(vm => vm.BadgeNormalBgShow).OneWay();
            bindingSet.Bind(view.BadgeMySelfBgShow).For(v => v.enabled).To(vm => vm.BadgeMySelfBgShow).OneWay();
            bindingSet.Bind(view.BadgeHurtBgShow).For(v => v.enabled).To(vm => vm.BadgeHurtBgShow).OneWay();
            bindingSet.Bind(view.BadgeDeadBgShow).For(v => v.enabled).To(vm => vm.BadgeDeadBgShow).OneWay();
            bindingSet.Bind(view.BadgeIconBundle).For(v => v.BundleName).To(vm => vm.BadgeIconBundle).OneWay();
            bindingSet.Bind(view.BadgeIconAsset).For(v => v.AssetName).To(vm => vm.BadgeIconAsset).OneWay();
            bindingSet.Bind(view.TitleIconShow1).For(v => v.activeSelf).To(vm => vm.TitleIconShow1).OneWay();
            bindingSet.Bind(view.TitleIconShow2).For(v => v.activeSelf).To(vm => vm.TitleIconShow2).OneWay();
            bindingSet.Bind(view.TitleIconShow3).For(v => v.activeSelf).To(vm => vm.TitleIconShow3).OneWay();
            bindingSet.Bind(view.TitleIconShow4).For(v => v.activeSelf).To(vm => vm.TitleIconShow4).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(BlastRecordInfoView view)
		{
            _imgGroupShow = view.ImgGroupShow.activeSelf;
            _textGroupShow = view.TextGroupShow.activeSelf;
            _iconGroupShow = view.IconGroupShow.activeSelf;
            _badgeGroupShow = view.BadgeGroupShow.activeSelf;
            _mySelfMaskShow = view.MySelfMaskShow.enabled;
            _deadStateShow = view.DeadStateShow.enabled;
            _hurtStateShow = view.HurtStateShow.enabled;
            _bombIconShow = view.BombIconShow.enabled;
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
            _bombText = view.BombText.text;
            _bombColor = view.BombColor.color;
            _badgeNormalBgShow = view.BadgeNormalBgShow.enabled;
            _badgeMySelfBgShow = view.BadgeMySelfBgShow.enabled;
            _badgeHurtBgShow = view.BadgeHurtBgShow.enabled;
            _badgeDeadBgShow = view.BadgeDeadBgShow.enabled;
            _badgeIconBundle = view.BadgeIconBundle.BundleName;
            _badgeIconAsset = view.BadgeIconAsset.AssetName;
		}


		void SaveOriData(BlastRecordInfoView view)
		{
            view.oriImgGroupShow = _imgGroupShow;
            view.oriTextGroupShow = _textGroupShow;
            view.oriIconGroupShow = _iconGroupShow;
            view.oriBadgeGroupShow = _badgeGroupShow;
            view.oriMySelfMaskShow = _mySelfMaskShow;
            view.oriDeadStateShow = _deadStateShow;
            view.oriHurtStateShow = _hurtStateShow;
            view.oriBombIconShow = _bombIconShow;
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
            view.oriBombText = _bombText;
            view.oriBombColor = _bombColor;
            view.oriBadgeNormalBgShow = _badgeNormalBgShow;
            view.oriBadgeMySelfBgShow = _badgeMySelfBgShow;
            view.oriBadgeHurtBgShow = _badgeHurtBgShow;
            view.oriBadgeDeadBgShow = _badgeDeadBgShow;
            view.oriBadgeIconBundle = _badgeIconBundle;
            view.oriBadgeIconAsset = _badgeIconAsset;
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
			BadgeGroupShow = _view.oriBadgeGroupShow;
			MySelfMaskShow = _view.oriMySelfMaskShow;
			DeadStateShow = _view.oriDeadStateShow;
			HurtStateShow = _view.oriHurtStateShow;
			BombIconShow = _view.oriBombIconShow;
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
			BombText = _view.oriBombText;
			BombColor = _view.oriBombColor;
			BadgeNormalBgShow = _view.oriBadgeNormalBgShow;
			BadgeMySelfBgShow = _view.oriBadgeMySelfBgShow;
			BadgeHurtBgShow = _view.oriBadgeHurtBgShow;
			BadgeDeadBgShow = _view.oriBadgeDeadBgShow;
			BadgeIconBundle = _view.oriBadgeIconBundle;
			BadgeIconAsset = _view.oriBadgeIconAsset;
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
        public string ResourceBundleName { get { return "ui/client/prefab/blast"; } }
        public string ResourceAssetName { get { return "BlastRecordInfo"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
