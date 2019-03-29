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
    public class CommonFocusMaskViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonFocusMaskView : UIView
        {
            public Action<BaseEventData> OnMaskEventTriggerHover;
            public Action<BaseEventData> OnMaskEventTriggerClick;
            public Action<BaseEventData> OnMaskEventTriggerMouseDown;
            public Action<BaseEventData> OnMaskEventTriggerMouseUp;
            public Action<BaseEventData> OnMaskEventTriggerHoverExit;
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public EventTrigger MaskEventTrigger;
            public void GenerateTrigger()
            {
            	MaskEventTrigger.triggers.Clear();
            	var maskEventTriggerEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	maskEventTriggerEntry.callback.AddListener((data) => { if (null != OnMaskEventTriggerHover) OnMaskEventTriggerHover(data); });
            	MaskEventTrigger.triggers.Add(maskEventTriggerEntry);
            	maskEventTriggerEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	maskEventTriggerEntry.callback.AddListener((data) => { if (null != OnMaskEventTriggerClick) OnMaskEventTriggerClick(data); });
            	MaskEventTrigger.triggers.Add(maskEventTriggerEntry);
            	maskEventTriggerEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	maskEventTriggerEntry.callback.AddListener((data) => { if (null != OnMaskEventTriggerMouseDown) OnMaskEventTriggerMouseDown(data); });
            	MaskEventTrigger.triggers.Add(maskEventTriggerEntry);
            	maskEventTriggerEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	maskEventTriggerEntry.callback.AddListener((data) => { if (null != OnMaskEventTriggerMouseUp) OnMaskEventTriggerMouseUp(data); });
            	MaskEventTrigger.triggers.Add(maskEventTriggerEntry);
            	maskEventTriggerEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	maskEventTriggerEntry.callback.AddListener((data) => { if (null != OnMaskEventTriggerHoverExit) OnMaskEventTriggerHoverExit(data); });
            	MaskEventTrigger.triggers.Add(maskEventTriggerEntry);
            }
            
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

                EventTrigger[] eventtriggers = gameObject.GetComponentsInChildren<EventTrigger>(true);
                foreach (var v in eventtriggers)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            MaskEventTrigger = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private List<EventTrigger.Entry> _maskEventTrigger;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public List<EventTrigger.Entry> MaskEventTrigger{	get { return _maskEventTrigger; }	set	{if (_maskEventTrigger != null){foreach (var eventEntry in _maskEventTrigger){_view.MaskEventTrigger.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.MaskEventTrigger.triggers.Add(eventEntry);}}Set(ref _maskEventTrigger, value, "MaskEventTrigger");}}

		private Action<Action<BaseEventData>> _onOnMaskEventTriggerHoverChanged;
		private Action<BaseEventData> _onMaskEventTriggerHoverListener;
		public Action<BaseEventData> OnMaskEventTriggerHoverListener { get { return _onMaskEventTriggerHoverListener ;} set { if (null != _onOnMaskEventTriggerHoverChanged) _onOnMaskEventTriggerHoverChanged(value); _onMaskEventTriggerHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnMaskEventTriggerClickChanged;
		private Action<BaseEventData> _onMaskEventTriggerClickListener;
		public Action<BaseEventData> OnMaskEventTriggerClickListener { get { return _onMaskEventTriggerClickListener ;} set { if (null != _onOnMaskEventTriggerClickChanged) _onOnMaskEventTriggerClickChanged(value); _onMaskEventTriggerClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnMaskEventTriggerMouseDownChanged;
		private Action<BaseEventData> _onMaskEventTriggerMouseDownListener;
		public Action<BaseEventData> OnMaskEventTriggerMouseDownListener { get { return _onMaskEventTriggerMouseDownListener ;} set { if (null != _onOnMaskEventTriggerMouseDownChanged) _onOnMaskEventTriggerMouseDownChanged(value); _onMaskEventTriggerMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnMaskEventTriggerMouseUpChanged;
		private Action<BaseEventData> _onMaskEventTriggerMouseUpListener;
		public Action<BaseEventData> OnMaskEventTriggerMouseUpListener { get { return _onMaskEventTriggerMouseUpListener ;} set { if (null != _onOnMaskEventTriggerMouseUpChanged) _onOnMaskEventTriggerMouseUpChanged(value); _onMaskEventTriggerMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnMaskEventTriggerHoverExitChanged;
		private Action<BaseEventData> _onMaskEventTriggerHoverExitListener;
		public Action<BaseEventData> OnMaskEventTriggerHoverExitListener { get { return _onMaskEventTriggerHoverExitListener ;} set { if (null != _onOnMaskEventTriggerHoverExitChanged) _onOnMaskEventTriggerHoverExitChanged(value); _onMaskEventTriggerHoverExitListener = value; } }
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonFocusMaskView _view;
		
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

			var view = obj.GetComponent<CommonFocusMaskView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.GenerateTrigger();
				EventTriggerBind(view);
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonFocusMaskView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonFocusMaskView, CommonFocusMaskViewModel> bindingSet =
                view.CreateBindingSet<CommonFocusMaskView, CommonFocusMaskViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.GenerateTrigger();
            EventTriggerBind(view);
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonFocusMaskView view)
		{
			_onOnMaskEventTriggerHoverChanged = (val) => view.OnMaskEventTriggerHover = val;
			_onOnMaskEventTriggerClickChanged = (val) => view.OnMaskEventTriggerClick = val;
			_onOnMaskEventTriggerMouseDownChanged = (val) => view.OnMaskEventTriggerMouseDown = val;
			_onOnMaskEventTriggerMouseUpChanged = (val) => view.OnMaskEventTriggerMouseUp = val;
			_onOnMaskEventTriggerHoverExitChanged = (val) => view.OnMaskEventTriggerHoverExit = val;
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonFocusMaskViewModel()
        {
            Type type = typeof(CommonFocusMaskViewModel);
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
        public string ResourceAssetName { get { return "CommonFocusMask"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
