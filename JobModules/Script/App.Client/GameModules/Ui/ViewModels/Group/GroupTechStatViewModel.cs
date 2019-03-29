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

namespace App.Client.GameModules.Ui.ViewModels.Group
{
    public class GroupTechStatViewModel : ViewModelBase, IUiViewModel
    {
        private class GroupTechStatView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text RoomInfoText;
            [HideInInspector]
            public string oriRoomInfoText;
            public Text PlayerCountText;
            [HideInInspector]
            public string oriPlayerCountText;
            
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
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "RoomInfo":
                            RoomInfoText = v;
                            break;
                        case "PlayerCount":
                            PlayerCountText = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _roomInfoText;
        private string _playerCountText;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string RoomInfoText { get { return _roomInfoText; } set {if(_roomInfoText != value) Set(ref _roomInfoText, value, "RoomInfoText"); } }
        public string PlayerCountText { get { return _playerCountText; } set {if(_playerCountText != value) Set(ref _playerCountText, value, "PlayerCountText"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private GroupTechStatView _view;
		
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

			var view = obj.GetComponent<GroupTechStatView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<GroupTechStatView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<GroupTechStatView, GroupTechStatViewModel> bindingSet =
                view.CreateBindingSet<GroupTechStatView, GroupTechStatViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.oriRoomInfoText = _roomInfoText = view.RoomInfoText.text;
            bindingSet.Bind(view.RoomInfoText).For(v => v.text).To(vm => vm.RoomInfoText).OneWay();
            view.oriPlayerCountText = _playerCountText = view.PlayerCountText.text;
            bindingSet.Bind(view.PlayerCountText).For(v => v.text).To(vm => vm.PlayerCountText).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(GroupTechStatView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static GroupTechStatViewModel()
        {
            Type type = typeof(GroupTechStatViewModel);
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
			RoomInfoText = _view.oriRoomInfoText;
			PlayerCountText = _view.oriPlayerCountText;
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

        public string ResourceBundleName { get { return "uiprefabs/group"; } }
        public string ResourceAssetName { get { return "GroupTechStat"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
