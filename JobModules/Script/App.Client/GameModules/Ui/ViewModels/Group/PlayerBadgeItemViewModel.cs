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
    public class PlayerBadgeItemViewModel : ViewModelBase, IUiViewModel
    {
        private class PlayerBadgeItemView : UIView
        {
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
            public GameObject BadgeGroupShow;
            [HideInInspector]
            public bool oriBadgeGroupShow;
            
            public void FillField()
            {
                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
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

                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BadgeGroup":
                            BadgeGroupShow = v.gameObject;
                            break;
                    }
                }

            }
        }


        private bool _badgeNormalBgShow;
        private bool _badgeMySelfBgShow;
        private bool _badgeHurtBgShow;
        private bool _badgeDeadBgShow;
        private string _badgeIconBundle;
        private string _badgeIconAsset;
        private bool _badgeGroupShow;
        public bool BadgeNormalBgShow { get { return _badgeNormalBgShow; } set {if(_badgeNormalBgShow != value) Set(ref _badgeNormalBgShow, value, "BadgeNormalBgShow"); } }
        public bool BadgeMySelfBgShow { get { return _badgeMySelfBgShow; } set {if(_badgeMySelfBgShow != value) Set(ref _badgeMySelfBgShow, value, "BadgeMySelfBgShow"); } }
        public bool BadgeHurtBgShow { get { return _badgeHurtBgShow; } set {if(_badgeHurtBgShow != value) Set(ref _badgeHurtBgShow, value, "BadgeHurtBgShow"); } }
        public bool BadgeDeadBgShow { get { return _badgeDeadBgShow; } set {if(_badgeDeadBgShow != value) Set(ref _badgeDeadBgShow, value, "BadgeDeadBgShow"); } }
        public string BadgeIconBundle { get { return _badgeIconBundle; } set {if(_badgeIconBundle != value) Set(ref _badgeIconBundle, value, "BadgeIconBundle"); } }
        public string BadgeIconAsset { get { return _badgeIconAsset; } set {if(_badgeIconAsset != value) Set(ref _badgeIconAsset, value, "BadgeIconAsset"); } }
        public bool BadgeGroupShow { get { return _badgeGroupShow; } set {if(_badgeGroupShow != value) Set(ref _badgeGroupShow, value, "BadgeGroupShow"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private PlayerBadgeItemView _view;
		
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
			var view = obj.GetComponent<PlayerBadgeItemView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<PlayerBadgeItemView>();
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
		private void EventTriggerBind(PlayerBadgeItemView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static PlayerBadgeItemViewModel()
        {
            Type type = typeof(PlayerBadgeItemViewModel);
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

		void ViewBind(PlayerBadgeItemView view)
		{
		     BindingSet<PlayerBadgeItemView, PlayerBadgeItemViewModel> bindingSet =
                view.CreateBindingSet<PlayerBadgeItemView, PlayerBadgeItemViewModel>();
            bindingSet.Bind(view.BadgeNormalBgShow).For(v => v.enabled).To(vm => vm.BadgeNormalBgShow).OneWay();
            bindingSet.Bind(view.BadgeMySelfBgShow).For(v => v.enabled).To(vm => vm.BadgeMySelfBgShow).OneWay();
            bindingSet.Bind(view.BadgeHurtBgShow).For(v => v.enabled).To(vm => vm.BadgeHurtBgShow).OneWay();
            bindingSet.Bind(view.BadgeDeadBgShow).For(v => v.enabled).To(vm => vm.BadgeDeadBgShow).OneWay();
            bindingSet.Bind(view.BadgeIconBundle).For(v => v.BundleName).To(vm => vm.BadgeIconBundle).OneWay();
            bindingSet.Bind(view.BadgeIconAsset).For(v => v.AssetName).To(vm => vm.BadgeIconAsset).OneWay();
            bindingSet.Bind(view.BadgeGroupShow).For(v => v.activeSelf).To(vm => vm.BadgeGroupShow).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(PlayerBadgeItemView view)
		{
            _badgeNormalBgShow = view.BadgeNormalBgShow.enabled;
            _badgeMySelfBgShow = view.BadgeMySelfBgShow.enabled;
            _badgeHurtBgShow = view.BadgeHurtBgShow.enabled;
            _badgeDeadBgShow = view.BadgeDeadBgShow.enabled;
            _badgeIconBundle = view.BadgeIconBundle.BundleName;
            _badgeIconAsset = view.BadgeIconAsset.AssetName;
            _badgeGroupShow = view.BadgeGroupShow.activeSelf;
		}


		void SaveOriData(PlayerBadgeItemView view)
		{
            view.oriBadgeNormalBgShow = _badgeNormalBgShow;
            view.oriBadgeMySelfBgShow = _badgeMySelfBgShow;
            view.oriBadgeHurtBgShow = _badgeHurtBgShow;
            view.oriBadgeDeadBgShow = _badgeDeadBgShow;
            view.oriBadgeIconBundle = _badgeIconBundle;
            view.oriBadgeIconAsset = _badgeIconAsset;
            view.oriBadgeGroupShow = _badgeGroupShow;
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
			BadgeNormalBgShow = _view.oriBadgeNormalBgShow;
			BadgeMySelfBgShow = _view.oriBadgeMySelfBgShow;
			BadgeHurtBgShow = _view.oriBadgeHurtBgShow;
			BadgeDeadBgShow = _view.oriBadgeDeadBgShow;
			BadgeIconBundle = _view.oriBadgeIconBundle;
			BadgeIconAsset = _view.oriBadgeIconAsset;
			BadgeGroupShow = _view.oriBadgeGroupShow;
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

        public string ResourceBundleName { get { return "ui/client/prefab/group"; } }
        public string ResourceAssetName { get { return "PlayerBadgeItem"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
