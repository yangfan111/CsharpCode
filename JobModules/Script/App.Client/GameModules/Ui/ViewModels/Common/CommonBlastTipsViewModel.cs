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
    public class CommonBlastTipsViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonBlastTipsView : UIView
        {
            public GameObject rootActiveSelf;
            [HideInInspector]
            public bool orirootActiveSelf;
            public GameObject AactiveSelf;
            [HideInInspector]
            public bool oriAactiveSelf;
            public RectTransform AUIPos;
            [HideInInspector]
            public Vector3 oriAUIPos;
            public GameObject BactiveSelf;
            [HideInInspector]
            public bool oriBactiveSelf;
            public RectTransform BUIPos;
            [HideInInspector]
            public Vector3 oriBUIPos;
            public GameObject C4activeSelf;
            [HideInInspector]
            public bool oriC4activeSelf;
            public RectTransform C4UIPos;
            [HideInInspector]
            public Vector3 oriC4UIPos;
            public Text AtitleText;
            [HideInInspector]
            public string oriAtitleText;
            public Text BtitleText;
            [HideInInspector]
            public string oriBtitleText;
            public Text C4titleText;
            [HideInInspector]
            public string oriC4titleText;
            public GameObject iconredA;
            [HideInInspector]
            public bool oriiconredA;
            public GameObject iconredB;
            [HideInInspector]
            public bool oriiconredB;
            
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
                        case "subrootA":
                            AactiveSelf = v.gameObject;
                            break;
                        case "subrootB":
                            BactiveSelf = v.gameObject;
                            break;
                        case "subrootC4":
                            C4activeSelf = v.gameObject;
                            break;
                        case "iconredA":
                            iconredA = v.gameObject;
                            break;
                        case "iconredB":
                            iconredB = v.gameObject;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "subrootA":
                            AUIPos = v;
                            break;
                        case "subrootB":
                            BUIPos = v;
                            break;
                        case "subrootC4":
                            C4UIPos = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Atitle":
                            AtitleText = v;
                            break;
                        case "Btitle":
                            BtitleText = v;
                            break;
                        case "C4title":
                            C4titleText = v;
                            break;
                    }
                }

            }
        }


        private bool _rootActiveSelf;
        private bool _aactiveSelf;
        private Vector3 _aUIPos;
        private bool _bactiveSelf;
        private Vector3 _bUIPos;
        private bool _c4activeSelf;
        private Vector3 _c4UIPos;
        private string _atitleText;
        private string _btitleText;
        private string _c4titleText;
        private bool _iconredA;
        private bool _iconredB;
        public bool rootActiveSelf { get { return _rootActiveSelf; } set {if(_rootActiveSelf != value) Set(ref _rootActiveSelf, value, "rootActiveSelf"); } }
        public bool AactiveSelf { get { return _aactiveSelf; } set {if(_aactiveSelf != value) Set(ref _aactiveSelf, value, "AactiveSelf"); } }
        public Vector3 AUIPos { get { return _aUIPos; } set {if(_aUIPos != value) Set(ref _aUIPos, value, "AUIPos"); } }
        public bool BactiveSelf { get { return _bactiveSelf; } set {if(_bactiveSelf != value) Set(ref _bactiveSelf, value, "BactiveSelf"); } }
        public Vector3 BUIPos { get { return _bUIPos; } set {if(_bUIPos != value) Set(ref _bUIPos, value, "BUIPos"); } }
        public bool C4activeSelf { get { return _c4activeSelf; } set {if(_c4activeSelf != value) Set(ref _c4activeSelf, value, "C4activeSelf"); } }
        public Vector3 C4UIPos { get { return _c4UIPos; } set {if(_c4UIPos != value) Set(ref _c4UIPos, value, "C4UIPos"); } }
        public string AtitleText { get { return _atitleText; } set {if(_atitleText != value) Set(ref _atitleText, value, "AtitleText"); } }
        public string BtitleText { get { return _btitleText; } set {if(_btitleText != value) Set(ref _btitleText, value, "BtitleText"); } }
        public string C4titleText { get { return _c4titleText; } set {if(_c4titleText != value) Set(ref _c4titleText, value, "C4titleText"); } }
        public bool iconredA { get { return _iconredA; } set {if(_iconredA != value) Set(ref _iconredA, value, "iconredA"); } }
        public bool iconredB { get { return _iconredB; } set {if(_iconredB != value) Set(ref _iconredB, value, "iconredB"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonBlastTipsView _view;
		
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

			var view = obj.GetComponent<CommonBlastTipsView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonBlastTipsView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonBlastTipsView, CommonBlastTipsViewModel> bindingSet =
                view.CreateBindingSet<CommonBlastTipsView, CommonBlastTipsViewModel>();

            view.orirootActiveSelf = _rootActiveSelf = view.rootActiveSelf.activeSelf;
            bindingSet.Bind(view.rootActiveSelf).For(v => v.activeSelf).To(vm => vm.rootActiveSelf).OneWay();
            view.oriAactiveSelf = _aactiveSelf = view.AactiveSelf.activeSelf;
            bindingSet.Bind(view.AactiveSelf).For(v => v.activeSelf).To(vm => vm.AactiveSelf).OneWay();
            view.oriAUIPos = _aUIPos = view.AUIPos.anchoredPosition3D;
            bindingSet.Bind(view.AUIPos).For(v => v.anchoredPosition3D).To(vm => vm.AUIPos).OneWay();
            view.oriBactiveSelf = _bactiveSelf = view.BactiveSelf.activeSelf;
            bindingSet.Bind(view.BactiveSelf).For(v => v.activeSelf).To(vm => vm.BactiveSelf).OneWay();
            view.oriBUIPos = _bUIPos = view.BUIPos.anchoredPosition3D;
            bindingSet.Bind(view.BUIPos).For(v => v.anchoredPosition3D).To(vm => vm.BUIPos).OneWay();
            view.oriC4activeSelf = _c4activeSelf = view.C4activeSelf.activeSelf;
            bindingSet.Bind(view.C4activeSelf).For(v => v.activeSelf).To(vm => vm.C4activeSelf).OneWay();
            view.oriC4UIPos = _c4UIPos = view.C4UIPos.anchoredPosition3D;
            bindingSet.Bind(view.C4UIPos).For(v => v.anchoredPosition3D).To(vm => vm.C4UIPos).OneWay();
            view.oriAtitleText = _atitleText = view.AtitleText.text;
            bindingSet.Bind(view.AtitleText).For(v => v.text).To(vm => vm.AtitleText).OneWay();
            view.oriBtitleText = _btitleText = view.BtitleText.text;
            bindingSet.Bind(view.BtitleText).For(v => v.text).To(vm => vm.BtitleText).OneWay();
            view.oriC4titleText = _c4titleText = view.C4titleText.text;
            bindingSet.Bind(view.C4titleText).For(v => v.text).To(vm => vm.C4titleText).OneWay();
            view.oriiconredA = _iconredA = view.iconredA.activeSelf;
            bindingSet.Bind(view.iconredA).For(v => v.activeSelf).To(vm => vm.iconredA).OneWay();
            view.oriiconredB = _iconredB = view.iconredB.activeSelf;
            bindingSet.Bind(view.iconredB).For(v => v.activeSelf).To(vm => vm.iconredB).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonBlastTipsView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonBlastTipsViewModel()
        {
            Type type = typeof(CommonBlastTipsViewModel);
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
			AactiveSelf = _view.oriAactiveSelf;
			AUIPos = _view.oriAUIPos;
			BactiveSelf = _view.oriBactiveSelf;
			BUIPos = _view.oriBUIPos;
			C4activeSelf = _view.oriC4activeSelf;
			C4UIPos = _view.oriC4UIPos;
			AtitleText = _view.oriAtitleText;
			BtitleText = _view.oriBtitleText;
			C4titleText = _view.oriC4titleText;
			iconredA = _view.oriiconredA;
			iconredB = _view.oriiconredB;
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
        public string ResourceAssetName { get { return "CommonBlastTips"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
