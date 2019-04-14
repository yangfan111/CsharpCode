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
    public class CommonNoticeViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonNoticeView : UIView
        {
            public Action<BaseEventData> OnYesBtnHover;
            public Action<BaseEventData> OnYesBtnClick;
            public Action<BaseEventData> OnYesBtnMouseDown;
            public Action<BaseEventData> OnYesBtnMouseUp;
            public Action<BaseEventData> OnYesBtnHoverExit;
            public Action<BaseEventData> OnNoBtnHover;
            public Action<BaseEventData> OnNoBtnClick;
            public Action<BaseEventData> OnNoBtnMouseDown;
            public Action<BaseEventData> OnNoBtnMouseUp;
            public Action<BaseEventData> OnNoBtnHoverExit;
            public GameObject rootActive;
            [HideInInspector]
            public bool orirootActive;
            public EventTrigger YesBtn;
            public GameObject YesBtnActive;
            [HideInInspector]
            public bool oriYesBtnActive;
            public Text yNameText;
            [HideInInspector]
            public string oriyNameText;
            public EventTrigger NoBtn;
            public GameObject NoBtnActive;
            [HideInInspector]
            public bool oriNoBtnActive;
            public Text nNameText;
            [HideInInspector]
            public string orinNameText;
            public Text TitleText;
            [HideInInspector]
            public string oriTitleText;
            public void GenerateTrigger()
            {
            	YesBtn.triggers.Clear();
            	var yesBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	yesBtnEntry.callback.AddListener((data) => { if (null != OnYesBtnHover) OnYesBtnHover(data); });
            	YesBtn.triggers.Add(yesBtnEntry);
            	yesBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	yesBtnEntry.callback.AddListener((data) => { if (null != OnYesBtnClick) OnYesBtnClick(data); });
            	YesBtn.triggers.Add(yesBtnEntry);
            	yesBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	yesBtnEntry.callback.AddListener((data) => { if (null != OnYesBtnMouseDown) OnYesBtnMouseDown(data); });
            	YesBtn.triggers.Add(yesBtnEntry);
            	yesBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	yesBtnEntry.callback.AddListener((data) => { if (null != OnYesBtnMouseUp) OnYesBtnMouseUp(data); });
            	YesBtn.triggers.Add(yesBtnEntry);
            	yesBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	yesBtnEntry.callback.AddListener((data) => { if (null != OnYesBtnHoverExit) OnYesBtnHoverExit(data); });
            	YesBtn.triggers.Add(yesBtnEntry);
            	NoBtn.triggers.Clear();
            	var noBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerEnter};
            	noBtnEntry.callback.AddListener((data) => { if (null != OnNoBtnHover) OnNoBtnHover(data); });
            	NoBtn.triggers.Add(noBtnEntry);
            	noBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerClick};
            	noBtnEntry.callback.AddListener((data) => { if (null != OnNoBtnClick) OnNoBtnClick(data); });
            	NoBtn.triggers.Add(noBtnEntry);
            	noBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerDown};
            	noBtnEntry.callback.AddListener((data) => { if (null != OnNoBtnMouseDown) OnNoBtnMouseDown(data); });
            	NoBtn.triggers.Add(noBtnEntry);
            	noBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerUp};
            	noBtnEntry.callback.AddListener((data) => { if (null != OnNoBtnMouseUp) OnNoBtnMouseUp(data); });
            	NoBtn.triggers.Add(noBtnEntry);
            	noBtnEntry = new EventTrigger.Entry() { callback = new EventTrigger.TriggerEvent(), eventID = EventTriggerType.PointerExit};
            	noBtnEntry.callback.AddListener((data) => { if (null != OnNoBtnHoverExit) OnNoBtnHoverExit(data); });
            	NoBtn.triggers.Add(noBtnEntry);
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
                            rootActive = v.gameObject;
                            break;
                        case "YesBtn":
                            YesBtnActive = v.gameObject;
                            break;
                        case "NoBtn":
                            NoBtnActive = v.gameObject;
                            break;
                    }
                }

                EventTrigger[] eventtriggers = gameObject.GetComponentsInChildren<EventTrigger>(true);
                foreach (var v in eventtriggers)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "YesBtn":
                            YesBtn = v;
                            break;
                        case "NoBtn":
                            NoBtn = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "yName":
                            yNameText = v;
                            break;
                        case "nName":
                            nNameText = v;
                            break;
                        case "Title":
                            TitleText = v;
                            break;
                    }
                }

            }
        }


        private bool _rootActive;
        private List<EventTrigger.Entry> _yesBtn;
        private bool _yesBtnActive;
        private string _yNameText;
        private List<EventTrigger.Entry> _noBtn;
        private bool _noBtnActive;
        private string _nNameText;
        private string _titleText;
        public bool rootActive { get { return _rootActive; } set {if(_rootActive != value) Set(ref _rootActive, value, "rootActive"); } }
        public List<EventTrigger.Entry> YesBtn{	get { return _yesBtn; }	set	{if (_yesBtn != null){foreach (var eventEntry in _yesBtn){_view.YesBtn.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.YesBtn.triggers.Add(eventEntry);}}Set(ref _yesBtn, value, "YesBtn");}}
        public bool YesBtnActive { get { return _yesBtnActive; } set {if(_yesBtnActive != value) Set(ref _yesBtnActive, value, "YesBtnActive"); } }
        public string yNameText { get { return _yNameText; } set {if(_yNameText != value) Set(ref _yNameText, value, "yNameText"); } }
        public List<EventTrigger.Entry> NoBtn{	get { return _noBtn; }	set	{if (_noBtn != null){foreach (var eventEntry in _noBtn){_view.NoBtn.triggers.Remove(eventEntry);}}if (value != null){foreach (var eventEntry in value){_view.NoBtn.triggers.Add(eventEntry);}}Set(ref _noBtn, value, "NoBtn");}}
        public bool NoBtnActive { get { return _noBtnActive; } set {if(_noBtnActive != value) Set(ref _noBtnActive, value, "NoBtnActive"); } }
        public string nNameText { get { return _nNameText; } set {if(_nNameText != value) Set(ref _nNameText, value, "nNameText"); } }
        public string TitleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "TitleText"); } }

		private Action<Action<BaseEventData>> _onOnYesBtnHoverChanged;
		private Action<BaseEventData> _onYesBtnHoverListener;
		public Action<BaseEventData> OnYesBtnHoverListener { get { return _onYesBtnHoverListener ;} set { if (null != _onOnYesBtnHoverChanged) _onOnYesBtnHoverChanged(value); _onYesBtnHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnYesBtnClickChanged;
		private Action<BaseEventData> _onYesBtnClickListener;
		public Action<BaseEventData> OnYesBtnClickListener { get { return _onYesBtnClickListener ;} set { if (null != _onOnYesBtnClickChanged) _onOnYesBtnClickChanged(value); _onYesBtnClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnYesBtnMouseDownChanged;
		private Action<BaseEventData> _onYesBtnMouseDownListener;
		public Action<BaseEventData> OnYesBtnMouseDownListener { get { return _onYesBtnMouseDownListener ;} set { if (null != _onOnYesBtnMouseDownChanged) _onOnYesBtnMouseDownChanged(value); _onYesBtnMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnYesBtnMouseUpChanged;
		private Action<BaseEventData> _onYesBtnMouseUpListener;
		public Action<BaseEventData> OnYesBtnMouseUpListener { get { return _onYesBtnMouseUpListener ;} set { if (null != _onOnYesBtnMouseUpChanged) _onOnYesBtnMouseUpChanged(value); _onYesBtnMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnYesBtnHoverExitChanged;
		private Action<BaseEventData> _onYesBtnHoverExitListener;
		public Action<BaseEventData> OnYesBtnHoverExitListener { get { return _onYesBtnHoverExitListener ;} set { if (null != _onOnYesBtnHoverExitChanged) _onOnYesBtnHoverExitChanged(value); _onYesBtnHoverExitListener = value; } }
		private Action<Action<BaseEventData>> _onOnNoBtnHoverChanged;
		private Action<BaseEventData> _onNoBtnHoverListener;
		public Action<BaseEventData> OnNoBtnHoverListener { get { return _onNoBtnHoverListener ;} set { if (null != _onOnNoBtnHoverChanged) _onOnNoBtnHoverChanged(value); _onNoBtnHoverListener = value; } }
		private Action<Action<BaseEventData>> _onOnNoBtnClickChanged;
		private Action<BaseEventData> _onNoBtnClickListener;
		public Action<BaseEventData> OnNoBtnClickListener { get { return _onNoBtnClickListener ;} set { if (null != _onOnNoBtnClickChanged) _onOnNoBtnClickChanged(value); _onNoBtnClickListener = value; } }
		private Action<Action<BaseEventData>> _onOnNoBtnMouseDownChanged;
		private Action<BaseEventData> _onNoBtnMouseDownListener;
		public Action<BaseEventData> OnNoBtnMouseDownListener { get { return _onNoBtnMouseDownListener ;} set { if (null != _onOnNoBtnMouseDownChanged) _onOnNoBtnMouseDownChanged(value); _onNoBtnMouseDownListener = value; } }
		private Action<Action<BaseEventData>> _onOnNoBtnMouseUpChanged;
		private Action<BaseEventData> _onNoBtnMouseUpListener;
		public Action<BaseEventData> OnNoBtnMouseUpListener { get { return _onNoBtnMouseUpListener ;} set { if (null != _onOnNoBtnMouseUpChanged) _onOnNoBtnMouseUpChanged(value); _onNoBtnMouseUpListener = value; } }
		private Action<Action<BaseEventData>> _onOnNoBtnHoverExitChanged;
		private Action<BaseEventData> _onNoBtnHoverExitListener;
		public Action<BaseEventData> OnNoBtnHoverExitListener { get { return _onNoBtnHoverExitListener ;} set { if (null != _onOnNoBtnHoverExitChanged) _onOnNoBtnHoverExitChanged(value); _onNoBtnHoverExitListener = value; } }
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonNoticeView _view;
		
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
			var view = obj.GetComponent<CommonNoticeView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonNoticeView>();
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
		private void EventTriggerBind(CommonNoticeView view)
		{
			_onOnYesBtnHoverChanged = (val) => view.OnYesBtnHover = val;
			_onOnYesBtnClickChanged = (val) => view.OnYesBtnClick = val;
			_onOnYesBtnMouseDownChanged = (val) => view.OnYesBtnMouseDown = val;
			_onOnYesBtnMouseUpChanged = (val) => view.OnYesBtnMouseUp = val;
			_onOnYesBtnHoverExitChanged = (val) => view.OnYesBtnHoverExit = val;
			_onOnNoBtnHoverChanged = (val) => view.OnNoBtnHover = val;
			_onOnNoBtnClickChanged = (val) => view.OnNoBtnClick = val;
			_onOnNoBtnMouseDownChanged = (val) => view.OnNoBtnMouseDown = val;
			_onOnNoBtnMouseUpChanged = (val) => view.OnNoBtnMouseUp = val;
			_onOnNoBtnHoverExitChanged = (val) => view.OnNoBtnHoverExit = val;
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonNoticeViewModel()
        {
            Type type = typeof(CommonNoticeViewModel);
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

		void ViewBind(CommonNoticeView view)
		{
		     BindingSet<CommonNoticeView, CommonNoticeViewModel> bindingSet =
                view.CreateBindingSet<CommonNoticeView, CommonNoticeViewModel>();
            bindingSet.Bind(view.rootActive).For(v => v.activeSelf).To(vm => vm.rootActive).OneWay();
            bindingSet.Bind(view.YesBtnActive).For(v => v.activeSelf).To(vm => vm.YesBtnActive).OneWay();
            bindingSet.Bind(view.yNameText).For(v => v.text).To(vm => vm.yNameText).OneWay();
            bindingSet.Bind(view.NoBtnActive).For(v => v.activeSelf).To(vm => vm.NoBtnActive).OneWay();
            bindingSet.Bind(view.nNameText).For(v => v.text).To(vm => vm.nNameText).OneWay();
            bindingSet.Bind(view.TitleText).For(v => v.text).To(vm => vm.TitleText).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonNoticeView view)
		{
            _rootActive = view.rootActive.activeSelf;
            _yesBtnActive = view.YesBtnActive.activeSelf;
            _yNameText = view.yNameText.text;
            _noBtnActive = view.NoBtnActive.activeSelf;
            _nNameText = view.nNameText.text;
            _titleText = view.TitleText.text;
		}


		void SaveOriData(CommonNoticeView view)
		{
            view.orirootActive = _rootActive;
            view.oriYesBtnActive = _yesBtnActive;
            view.oriyNameText = _yNameText;
            view.oriNoBtnActive = _noBtnActive;
            view.orinNameText = _nNameText;
            view.oriTitleText = _titleText;
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
			rootActive = _view.orirootActive;
			YesBtnActive = _view.oriYesBtnActive;
			yNameText = _view.oriyNameText;
			NoBtnActive = _view.oriNoBtnActive;
			nNameText = _view.orinNameText;
			TitleText = _view.oriTitleText;
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
        public string ResourceAssetName { get { return "CommonNotice"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
