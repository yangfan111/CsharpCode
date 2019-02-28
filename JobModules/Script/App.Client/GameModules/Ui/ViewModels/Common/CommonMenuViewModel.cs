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
using UnityEngine.EventSystems;

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonMenuViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonMenuView : UIView
        {
            public Action<BaseEventData> OnBackGameETHover;
            public Action<BaseEventData> OnBackGameETClick;
            public Action<BaseEventData> OnBackGameETMouseDown;
            public Action<BaseEventData> OnBackGameETMouseUp;
            public Action<BaseEventData> OnBackGameETHoverExit;
            public Action<BaseEventData> OnSettingETHover;
            public Action<BaseEventData> OnSettingETClick;
            public Action<BaseEventData> OnSettingETMouseDown;
            public Action<BaseEventData> OnSettingETMouseUp;
            public Action<BaseEventData> OnSettingETHoverExit;
            public Action<BaseEventData> OnBackHallETHover;
            public Action<BaseEventData> OnBackHallETClick;
            public Action<BaseEventData> OnBackHallETMouseDown;
            public Action<BaseEventData> OnBackHallETMouseUp;
            public Action<BaseEventData> OnBackHallETHoverExit;
            public GameObject rootActiveSelf;
            [HideInInspector]
            public bool orirootActiveSelf;
            public EventTrigger BackGameET;
            public EventTrigger SettingET;
            public EventTrigger BackHallET;
            public void GenerateTrigger()
            {
            	BackGameET.triggers.Clear();
            	var backGameETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	backGameETEntry.callback.AddListener((data) => { if (null != OnBackGameETHover) OnBackGameETHover(data); });
            	BackGameET.triggers.Add(backGameETEntry);
            	backGameETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	backGameETEntry.callback.AddListener((data) => { if (null != OnBackGameETClick) OnBackGameETClick(data); });
            	BackGameET.triggers.Add(backGameETEntry);
            	backGameETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	backGameETEntry.callback.AddListener((data) => { if (null != OnBackGameETMouseDown) OnBackGameETMouseDown(data); });
            	BackGameET.triggers.Add(backGameETEntry);
            	backGameETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	backGameETEntry.callback.AddListener((data) => { if (null != OnBackGameETMouseUp) OnBackGameETMouseUp(data); });
            	BackGameET.triggers.Add(backGameETEntry);
            	backGameETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	backGameETEntry.callback.AddListener((data) => { if (null != OnBackGameETHoverExit) OnBackGameETHoverExit(data); });
            	BackGameET.triggers.Add(backGameETEntry);
            	SettingET.triggers.Clear();
            	var settingETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	settingETEntry.callback.AddListener((data) => { if (null != OnSettingETHover) OnSettingETHover(data); });
            	SettingET.triggers.Add(settingETEntry);
            	settingETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	settingETEntry.callback.AddListener((data) => { if (null != OnSettingETClick) OnSettingETClick(data); });
            	SettingET.triggers.Add(settingETEntry);
            	settingETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	settingETEntry.callback.AddListener((data) => { if (null != OnSettingETMouseDown) OnSettingETMouseDown(data); });
            	SettingET.triggers.Add(settingETEntry);
            	settingETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	settingETEntry.callback.AddListener((data) => { if (null != OnSettingETMouseUp) OnSettingETMouseUp(data); });
            	SettingET.triggers.Add(settingETEntry);
            	settingETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	settingETEntry.callback.AddListener((data) => { if (null != OnSettingETHoverExit) OnSettingETHoverExit(data); });
            	SettingET.triggers.Add(settingETEntry);
            	BackHallET.triggers.Clear();
            	var backHallETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	backHallETEntry.callback.AddListener((data) => { if (null != OnBackHallETHover) OnBackHallETHover(data); });
            	BackHallET.triggers.Add(backHallETEntry);
            	backHallETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	backHallETEntry.callback.AddListener((data) => { if (null != OnBackHallETClick) OnBackHallETClick(data); });
            	BackHallET.triggers.Add(backHallETEntry);
            	backHallETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	backHallETEntry.callback.AddListener((data) => { if (null != OnBackHallETMouseDown) OnBackHallETMouseDown(data); });
            	BackHallET.triggers.Add(backHallETEntry);
            	backHallETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	backHallETEntry.callback.AddListener((data) => { if (null != OnBackHallETMouseUp) OnBackHallETMouseUp(data); });
            	BackHallET.triggers.Add(backHallETEntry);
            	backHallETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	backHallETEntry.callback.AddListener((data) => { if (null != OnBackHallETHoverExit) OnBackHallETHoverExit(data); });
            	BackHallET.triggers.Add(backHallETEntry);
            }
            
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

                EventTrigger[] eventtriggers = gameObject.GetComponentsInChildren<EventTrigger>(true);
                foreach (var v in eventtriggers)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BackGame":
                            BackGameET = v;
                            break;
                        case "Setting":
                            SettingET = v;
                            break;
                        case "BackHall":
                            BackHallET = v;
                            break;
                    }
                }

            }
        }


        private bool _rootActiveSelf;
        private List<EventTrigger.Entry> _backGameET;
        private List<EventTrigger.Entry> _settingET;
        private List<EventTrigger.Entry> _backHallET;
        public bool rootActiveSelf { get { return _rootActiveSelf; } set {if(_rootActiveSelf != value) Set(ref _rootActiveSelf, value, "rootActiveSelf"); } }
        public List<EventTrigger.Entry> BackGameET{	get { return _backGameET; }	set	{if (_backGameET != null){foreach (var eventEntry in _backGameET){_view.BackGameET.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.BackGameET.triggers.Add(eventEntry);}}Set(ref _backGameET, value, "BackGameET");}}
        public List<EventTrigger.Entry> SettingET{	get { return _settingET; }	set	{if (_settingET != null){foreach (var eventEntry in _settingET){_view.SettingET.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.SettingET.triggers.Add(eventEntry);}}Set(ref _settingET, value, "SettingET");}}
        public List<EventTrigger.Entry> BackHallET{	get { return _backHallET; }	set	{if (_backHallET != null){foreach (var eventEntry in _backHallET){_view.BackHallET.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.BackHallET.triggers.Add(eventEntry);}}Set(ref _backHallET, value, "BackHallET");}}

		private Action<Action<BaseEventData>> _onOnBackGameETHoverChanged;
		private Action<BaseEventData> _onBackGameETHoverListener;
		public Action<BaseEventData> OnBackGameETHoverListener { get { return _onBackGameETHoverListener ;} set { if (null != _onOnBackGameETHoverChanged) _onOnBackGameETHoverChanged(value); _onBackGameETHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackGameETClickChanged;
		private Action<BaseEventData> _onBackGameETClickListener;
		public Action<BaseEventData> OnBackGameETClickListener { get { return _onBackGameETClickListener ;} set { if (null != _onOnBackGameETClickChanged) _onOnBackGameETClickChanged(value); _onBackGameETClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackGameETMouseDownChanged;
		private Action<BaseEventData> _onBackGameETMouseDownListener;
		public Action<BaseEventData> OnBackGameETMouseDownListener { get { return _onBackGameETMouseDownListener ;} set { if (null != _onOnBackGameETMouseDownChanged) _onOnBackGameETMouseDownChanged(value); _onBackGameETMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackGameETMouseUpChanged;
		private Action<BaseEventData> _onBackGameETMouseUpListener;
		public Action<BaseEventData> OnBackGameETMouseUpListener { get { return _onBackGameETMouseUpListener ;} set { if (null != _onOnBackGameETMouseUpChanged) _onOnBackGameETMouseUpChanged(value); _onBackGameETMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackGameETHoverExitChanged;
		private Action<BaseEventData> _onBackGameETHoverExitListener;
		public Action<BaseEventData> OnBackGameETHoverExitListener { get { return _onBackGameETHoverExitListener ;} set { if (null != _onOnBackGameETHoverExitChanged) _onOnBackGameETHoverExitChanged(value); _onBackGameETHoverExitListener = value; } }
		private Action<Action<BaseEventData>> _onOnSettingETHoverChanged;
		private Action<BaseEventData> _onSettingETHoverListener;
		public Action<BaseEventData> OnSettingETHoverListener { get { return _onSettingETHoverListener ;} set { if (null != _onOnSettingETHoverChanged) _onOnSettingETHoverChanged(value); _onSettingETHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnSettingETClickChanged;
		private Action<BaseEventData> _onSettingETClickListener;
		public Action<BaseEventData> OnSettingETClickListener { get { return _onSettingETClickListener ;} set { if (null != _onOnSettingETClickChanged) _onOnSettingETClickChanged(value); _onSettingETClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnSettingETMouseDownChanged;
		private Action<BaseEventData> _onSettingETMouseDownListener;
		public Action<BaseEventData> OnSettingETMouseDownListener { get { return _onSettingETMouseDownListener ;} set { if (null != _onOnSettingETMouseDownChanged) _onOnSettingETMouseDownChanged(value); _onSettingETMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnSettingETMouseUpChanged;
		private Action<BaseEventData> _onSettingETMouseUpListener;
		public Action<BaseEventData> OnSettingETMouseUpListener { get { return _onSettingETMouseUpListener ;} set { if (null != _onOnSettingETMouseUpChanged) _onOnSettingETMouseUpChanged(value); _onSettingETMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnSettingETHoverExitChanged;
		private Action<BaseEventData> _onSettingETHoverExitListener;
		public Action<BaseEventData> OnSettingETHoverExitListener { get { return _onSettingETHoverExitListener ;} set { if (null != _onOnSettingETHoverExitChanged) _onOnSettingETHoverExitChanged(value); _onSettingETHoverExitListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackHallETHoverChanged;
		private Action<BaseEventData> _onBackHallETHoverListener;
		public Action<BaseEventData> OnBackHallETHoverListener { get { return _onBackHallETHoverListener ;} set { if (null != _onOnBackHallETHoverChanged) _onOnBackHallETHoverChanged(value); _onBackHallETHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackHallETClickChanged;
		private Action<BaseEventData> _onBackHallETClickListener;
		public Action<BaseEventData> OnBackHallETClickListener { get { return _onBackHallETClickListener ;} set { if (null != _onOnBackHallETClickChanged) _onOnBackHallETClickChanged(value); _onBackHallETClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackHallETMouseDownChanged;
		private Action<BaseEventData> _onBackHallETMouseDownListener;
		public Action<BaseEventData> OnBackHallETMouseDownListener { get { return _onBackHallETMouseDownListener ;} set { if (null != _onOnBackHallETMouseDownChanged) _onOnBackHallETMouseDownChanged(value); _onBackHallETMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackHallETMouseUpChanged;
		private Action<BaseEventData> _onBackHallETMouseUpListener;
		public Action<BaseEventData> OnBackHallETMouseUpListener { get { return _onBackHallETMouseUpListener ;} set { if (null != _onOnBackHallETMouseUpChanged) _onOnBackHallETMouseUpChanged(value); _onBackHallETMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnBackHallETHoverExitChanged;
		private Action<BaseEventData> _onBackHallETHoverExitListener;
		public Action<BaseEventData> OnBackHallETHoverExitListener { get { return _onBackHallETHoverExitListener ;} set { if (null != _onOnBackHallETHoverExitChanged) _onOnBackHallETHoverExitChanged(value); _onBackHallETHoverExitListener = value; } }
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMenuView _view;
		
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

			var view = obj.GetComponent<CommonMenuView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.GenerateTrigger();
				EventTriggerBind(view);
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonMenuView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonMenuView, CommonMenuViewModel> bindingSet =
                view.CreateBindingSet<CommonMenuView, CommonMenuViewModel>();

            view.orirootActiveSelf = _rootActiveSelf = view.rootActiveSelf.activeSelf;
            bindingSet.Bind(view.rootActiveSelf).For(v => v.activeSelf).To(vm => vm.rootActiveSelf).OneWay();
            view.GenerateTrigger();
            EventTriggerBind(view);
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonMenuView view)
		{
			_onOnBackGameETHoverChanged = (val) => view.OnBackGameETHover = val;
			_onOnBackGameETClickChanged = (val) => view.OnBackGameETClick = val;
			_onOnBackGameETMouseDownChanged = (val) => view.OnBackGameETMouseDown = val;
			_onOnBackGameETMouseUpChanged = (val) => view.OnBackGameETMouseUp = val;
			_onOnBackGameETHoverExitChanged = (val) => view.OnBackGameETHoverExit = val;
			_onOnSettingETHoverChanged = (val) => view.OnSettingETHover = val;
			_onOnSettingETClickChanged = (val) => view.OnSettingETClick = val;
			_onOnSettingETMouseDownChanged = (val) => view.OnSettingETMouseDown = val;
			_onOnSettingETMouseUpChanged = (val) => view.OnSettingETMouseUp = val;
			_onOnSettingETHoverExitChanged = (val) => view.OnSettingETHoverExit = val;
			_onOnBackHallETHoverChanged = (val) => view.OnBackHallETHover = val;
			_onOnBackHallETClickChanged = (val) => view.OnBackHallETClick = val;
			_onOnBackHallETMouseDownChanged = (val) => view.OnBackHallETMouseDown = val;
			_onOnBackHallETMouseUpChanged = (val) => view.OnBackHallETMouseUp = val;
			_onOnBackHallETHoverExitChanged = (val) => view.OnBackHallETHoverExit = val;
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonMenuViewModel()
        {
            Type type = typeof(CommonMenuViewModel);
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
        public string ResourceAssetName { get { return "CommonMenu"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
