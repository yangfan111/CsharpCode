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
    public class CommonTechStatViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonTechStatView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text KillerNameString;
            [HideInInspector]
            public string oriKillerNameString;
            public Text KillerLvString;
            [HideInInspector]
            public string oriKillerLvString;
            public Image KillerTitleSprite;
            public Image BadgeIconSprite;
            public Image DeathTypeIconSprite;
            public GameObject DeathTypeGroupShow;
            [HideInInspector]
            public bool oriDeathTypeGroupShow;
            public Image KillerCardBgSprite;
            public GameObject KillerLvShow;
            [HideInInspector]
            public bool oriKillerLvShow;
            public GameObject KillerTitleShow;
            [HideInInspector]
            public bool oriKillerTitleShow;
            public GameObject KillerCardBgShow;
            [HideInInspector]
            public bool oriKillerCardBgShow;
            
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
                        case "DeathTypeGroup":
                            DeathTypeGroupShow = v.gameObject;
                            break;
                        case "KillerLv":
                            KillerLvShow = v.gameObject;
                            break;
                        case "KillerTitle":
                            KillerTitleShow = v.gameObject;
                            break;
                        case "KillerCardBg":
                            KillerCardBgShow = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "KillerName":
                            KillerNameString = v;
                            break;
                        case "KillerLv":
                            KillerLvString = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "KillerTitle":
                            KillerTitleSprite = v;
                            break;
                        case "BadgeIcon":
                            BadgeIconSprite = v;
                            break;
                        case "DeathTypeIcon":
                            DeathTypeIconSprite = v;
                            break;
                        case "KillerCardBg":
                            KillerCardBgSprite = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _killerNameString;
        private string _killerLvString;
        private Sprite _killerTitleSprite;
        private Sprite _badgeIconSprite;
        private Sprite _deathTypeIconSprite;
        private bool _deathTypeGroupShow;
        private Sprite _killerCardBgSprite;
        private bool _killerLvShow;
        private bool _killerTitleShow;
        private bool _killerCardBgShow;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string KillerNameString { get { return _killerNameString; } set {if(_killerNameString != value) Set(ref _killerNameString, value, "KillerNameString"); } }
        public string KillerLvString { get { return _killerLvString; } set {if(_killerLvString != value) Set(ref _killerLvString, value, "KillerLvString"); } }
        public Sprite KillerTitleSprite { get { return _killerTitleSprite; } set {if(_killerTitleSprite != value) Set(ref _killerTitleSprite, value, "KillerTitleSprite"); if(null != _view && null != _view.KillerTitleSprite && null == value) _view.KillerTitleSprite.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite BadgeIconSprite { get { return _badgeIconSprite; } set {if(_badgeIconSprite != value) Set(ref _badgeIconSprite, value, "BadgeIconSprite"); if(null != _view && null != _view.BadgeIconSprite && null == value) _view.BadgeIconSprite.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite DeathTypeIconSprite { get { return _deathTypeIconSprite; } set {if(_deathTypeIconSprite != value) Set(ref _deathTypeIconSprite, value, "DeathTypeIconSprite"); if(null != _view && null != _view.DeathTypeIconSprite && null == value) _view.DeathTypeIconSprite.sprite = ViewModelUtil.EmptySprite; } }
        public bool DeathTypeGroupShow { get { return _deathTypeGroupShow; } set {if(_deathTypeGroupShow != value) Set(ref _deathTypeGroupShow, value, "DeathTypeGroupShow"); } }
        public Sprite KillerCardBgSprite { get { return _killerCardBgSprite; } set {if(_killerCardBgSprite != value) Set(ref _killerCardBgSprite, value, "KillerCardBgSprite"); if(null != _view && null != _view.KillerCardBgSprite && null == value) _view.KillerCardBgSprite.sprite = ViewModelUtil.EmptySprite; } }
        public bool KillerLvShow { get { return _killerLvShow; } set {if(_killerLvShow != value) Set(ref _killerLvShow, value, "KillerLvShow"); } }
        public bool KillerTitleShow { get { return _killerTitleShow; } set {if(_killerTitleShow != value) Set(ref _killerTitleShow, value, "KillerTitleShow"); } }
        public bool KillerCardBgShow { get { return _killerCardBgShow; } set {if(_killerCardBgShow != value) Set(ref _killerCardBgShow, value, "KillerCardBgShow"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonTechStatView _view;
		
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
			var view = obj.GetComponent<CommonTechStatView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonTechStatView>();
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
		private void EventTriggerBind(CommonTechStatView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonTechStatViewModel()
        {
            Type type = typeof(CommonTechStatViewModel);
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

		void ViewBind(CommonTechStatView view)
		{
		     BindingSet<CommonTechStatView, CommonTechStatViewModel> bindingSet =
                view.CreateBindingSet<CommonTechStatView, CommonTechStatViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.KillerNameString).For(v => v.text).To(vm => vm.KillerNameString).OneWay();
            bindingSet.Bind(view.KillerLvString).For(v => v.text).To(vm => vm.KillerLvString).OneWay();
            bindingSet.Bind(view.KillerTitleSprite).For(v => v.sprite).To(vm => vm.KillerTitleSprite).OneWay();
            bindingSet.Bind(view.BadgeIconSprite).For(v => v.sprite).To(vm => vm.BadgeIconSprite).OneWay();
            bindingSet.Bind(view.DeathTypeIconSprite).For(v => v.sprite).To(vm => vm.DeathTypeIconSprite).OneWay();
            bindingSet.Bind(view.DeathTypeGroupShow).For(v => v.activeSelf).To(vm => vm.DeathTypeGroupShow).OneWay();
            bindingSet.Bind(view.KillerCardBgSprite).For(v => v.sprite).To(vm => vm.KillerCardBgSprite).OneWay();
            bindingSet.Bind(view.KillerLvShow).For(v => v.activeSelf).To(vm => vm.KillerLvShow).OneWay();
            bindingSet.Bind(view.KillerTitleShow).For(v => v.activeSelf).To(vm => vm.KillerTitleShow).OneWay();
            bindingSet.Bind(view.KillerCardBgShow).For(v => v.activeSelf).To(vm => vm.KillerCardBgShow).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonTechStatView view)
		{
            _show = view.Show.activeSelf;
            _killerNameString = view.KillerNameString.text;
            _killerLvString = view.KillerLvString.text;
            _deathTypeGroupShow = view.DeathTypeGroupShow.activeSelf;
            _killerLvShow = view.KillerLvShow.activeSelf;
            _killerTitleShow = view.KillerTitleShow.activeSelf;
            _killerCardBgShow = view.KillerCardBgShow.activeSelf;
		}


		void SaveOriData(CommonTechStatView view)
		{
            view.oriShow = _show;
            view.oriKillerNameString = _killerNameString;
            view.oriKillerLvString = _killerLvString;
            view.oriDeathTypeGroupShow = _deathTypeGroupShow;
            view.oriKillerLvShow = _killerLvShow;
            view.oriKillerTitleShow = _killerTitleShow;
            view.oriKillerCardBgShow = _killerCardBgShow;
		}




		private void SpriteReset()
		{
			KillerTitleSprite = ViewModelUtil.EmptySprite;
			BadgeIconSprite = ViewModelUtil.EmptySprite;
			DeathTypeIconSprite = ViewModelUtil.EmptySprite;
			KillerCardBgSprite = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			Show = _view.oriShow;
			KillerNameString = _view.oriKillerNameString;
			KillerLvString = _view.oriKillerLvString;
			DeathTypeGroupShow = _view.oriDeathTypeGroupShow;
			KillerLvShow = _view.oriKillerLvShow;
			KillerTitleShow = _view.oriKillerTitleShow;
			KillerCardBgShow = _view.oriKillerCardBgShow;
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
        public string ResourceAssetName { get { return "CommonTechStat"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
