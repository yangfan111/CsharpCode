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
using UnityEngine.EventSystems;

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonMaxMapViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonMaxMapView : UIView
        {
            public Action<BaseEventData> OnmaskBgETHover;
            public Action<BaseEventData> OnmaskBgETClick;
            public Action<BaseEventData> OnmaskBgETMouseDown;
            public Action<BaseEventData> OnmaskBgETMouseUp;
            public Action<BaseEventData> OnmaskBgETHoverExit;
            public RectTransform rootLocation;
            [HideInInspector]
            public Vector2 orirootLocation;
            public EventTrigger maskBgET;
            public void GenerateTrigger()
            {
            	maskBgET.triggers.Clear();
            	var maskBgETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	maskBgETEntry.callback.AddListener((data) => { if (null != OnmaskBgETHover) OnmaskBgETHover(data); });
            	maskBgET.triggers.Add(maskBgETEntry);
            	maskBgETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	maskBgETEntry.callback.AddListener((data) => { if (null != OnmaskBgETClick) OnmaskBgETClick(data); });
            	maskBgET.triggers.Add(maskBgETEntry);
            	maskBgETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	maskBgETEntry.callback.AddListener((data) => { if (null != OnmaskBgETMouseDown) OnmaskBgETMouseDown(data); });
            	maskBgET.triggers.Add(maskBgETEntry);
            	maskBgETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	maskBgETEntry.callback.AddListener((data) => { if (null != OnmaskBgETMouseUp) OnmaskBgETMouseUp(data); });
            	maskBgET.triggers.Add(maskBgETEntry);
            	maskBgETEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	maskBgETEntry.callback.AddListener((data) => { if (null != OnmaskBgETHoverExit) OnmaskBgETHoverExit(data); });
            	maskBgET.triggers.Add(maskBgETEntry);
            }
            
            public void FillField()
            {
                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootLocation = v;
                            break;
                    }
                }

                EventTrigger[] eventtriggers = gameObject.GetComponentsInChildren<EventTrigger>(true);
                foreach (var v in eventtriggers)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Bg":
                            maskBgET = v;
                            break;
                    }
                }

            }
        }


        private Vector2 _rootLocation;
        private List<EventTrigger.Entry> _maskBgET;
        public Vector2 rootLocation { get { return _rootLocation; } set {if(_rootLocation != value) Set(ref _rootLocation, value, "rootLocation"); } }
        public List<EventTrigger.Entry> maskBgET{	get { return _maskBgET; }	set	{if (_maskBgET != null){foreach (var eventEntry in _maskBgET){_view.maskBgET.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.maskBgET.triggers.Add(eventEntry);}}Set(ref _maskBgET, value, "maskBgET");}}

		private Action<Action<BaseEventData>> _onOnmaskBgETHoverChanged;
		private Action<BaseEventData> _onmaskBgETHoverListener;
		public Action<BaseEventData> OnmaskBgETHoverListener { get { return _onmaskBgETHoverListener ;} set { if (null != _onOnmaskBgETHoverChanged) _onOnmaskBgETHoverChanged(value); _onmaskBgETHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnmaskBgETClickChanged;
		private Action<BaseEventData> _onmaskBgETClickListener;
		public Action<BaseEventData> OnmaskBgETClickListener { get { return _onmaskBgETClickListener ;} set { if (null != _onOnmaskBgETClickChanged) _onOnmaskBgETClickChanged(value); _onmaskBgETClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnmaskBgETMouseDownChanged;
		private Action<BaseEventData> _onmaskBgETMouseDownListener;
		public Action<BaseEventData> OnmaskBgETMouseDownListener { get { return _onmaskBgETMouseDownListener ;} set { if (null != _onOnmaskBgETMouseDownChanged) _onOnmaskBgETMouseDownChanged(value); _onmaskBgETMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnmaskBgETMouseUpChanged;
		private Action<BaseEventData> _onmaskBgETMouseUpListener;
		public Action<BaseEventData> OnmaskBgETMouseUpListener { get { return _onmaskBgETMouseUpListener ;} set { if (null != _onOnmaskBgETMouseUpChanged) _onOnmaskBgETMouseUpChanged(value); _onmaskBgETMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnmaskBgETHoverExitChanged;
		private Action<BaseEventData> _onmaskBgETHoverExitListener;
		public Action<BaseEventData> OnmaskBgETHoverExitListener { get { return _onmaskBgETHoverExitListener ;} set { if (null != _onOnmaskBgETHoverExitChanged) _onOnmaskBgETHoverExitChanged(value); _onmaskBgETHoverExitListener = value; } }
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMaxMapView _view;
		
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
			var view = obj.GetComponent<CommonMaxMapView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonMaxMapView>();
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

			view.GenerateTrigger();
			EventTriggerBind(view);
        }
		private void EventTriggerBind(CommonMaxMapView view)
		{
			_onOnmaskBgETHoverChanged = (val) => view.OnmaskBgETHover = val;
			_onOnmaskBgETClickChanged = (val) => view.OnmaskBgETClick = val;
			_onOnmaskBgETMouseDownChanged = (val) => view.OnmaskBgETMouseDown = val;
			_onOnmaskBgETMouseUpChanged = (val) => view.OnmaskBgETMouseUp = val;
			_onOnmaskBgETHoverExitChanged = (val) => view.OnmaskBgETHoverExit = val;
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonMaxMapViewModel()
        {
            Type type = typeof(CommonMaxMapViewModel);
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

		void ViewBind(CommonMaxMapView view)
		{
		     BindingSet<CommonMaxMapView, CommonMaxMapViewModel> bindingSet =
                view.CreateBindingSet<CommonMaxMapView, CommonMaxMapViewModel>();
            bindingSet.Bind(view.rootLocation).For(v => v.anchoredPosition).To(vm => vm.rootLocation).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonMaxMapView view)
		{
            _rootLocation = view.rootLocation.anchoredPosition;
		}


		void SaveOriData(CommonMaxMapView view)
		{
            view.orirootLocation = _rootLocation;
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
			rootLocation = _view.orirootLocation;
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
        public string ResourceAssetName { get { return "CommonMaxMap"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
