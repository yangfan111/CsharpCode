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

namespace App.Client.GameModules.Ui.ViewModels.Chicken
{
    public class ChickenScoreViewModel : ViewModelBase, IUiViewModel
    {
        private class ChickenScoreView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public GameObject TotalPlayerInfoShow;
            [HideInInspector]
            public bool oriTotalPlayerInfoShow;
            public GameObject BeatGroupShow;
            [HideInInspector]
            public bool oriBeatGroupShow;
            public Text BeatPlayerCountString;
            [HideInInspector]
            public string oriBeatPlayerCountString;
            public GameObject SurvivalGroupShow;
            [HideInInspector]
            public bool oriSurvivalGroupShow;
            public Text SurvivalPlayerCountString;
            [HideInInspector]
            public string oriSurvivalPlayerCountString;
            public GameObject JoinGroupShow;
            [HideInInspector]
            public bool oriJoinGroupShow;
            public Text JoinPlayerCountString;
            [HideInInspector]
            public string oriJoinPlayerCountString;
            
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
                        case "TotalPlayerInfo":
                            TotalPlayerInfoShow = v.gameObject;
                            break;
                        case "BeatGroup":
                            BeatGroupShow = v.gameObject;
                            break;
                        case "SurvivalGroup":
                            SurvivalGroupShow = v.gameObject;
                            break;
                        case "JoinGroup":
                            JoinGroupShow = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BeatPlayerCount":
                            BeatPlayerCountString = v;
                            break;
                        case "SurvivalPlayerCount":
                            SurvivalPlayerCountString = v;
                            break;
                        case "JoinPlayerCount":
                            JoinPlayerCountString = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private bool _totalPlayerInfoShow;
        private bool _beatGroupShow;
        private string _beatPlayerCountString;
        private bool _survivalGroupShow;
        private string _survivalPlayerCountString;
        private bool _joinGroupShow;
        private string _joinPlayerCountString;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public bool TotalPlayerInfoShow { get { return _totalPlayerInfoShow; } set {if(_totalPlayerInfoShow != value) Set(ref _totalPlayerInfoShow, value, "TotalPlayerInfoShow"); } }
        public bool BeatGroupShow { get { return _beatGroupShow; } set {if(_beatGroupShow != value) Set(ref _beatGroupShow, value, "BeatGroupShow"); } }
        public string BeatPlayerCountString { get { return _beatPlayerCountString; } set {if(_beatPlayerCountString != value) Set(ref _beatPlayerCountString, value, "BeatPlayerCountString"); } }
        public bool SurvivalGroupShow { get { return _survivalGroupShow; } set {if(_survivalGroupShow != value) Set(ref _survivalGroupShow, value, "SurvivalGroupShow"); } }
        public string SurvivalPlayerCountString { get { return _survivalPlayerCountString; } set {if(_survivalPlayerCountString != value) Set(ref _survivalPlayerCountString, value, "SurvivalPlayerCountString"); } }
        public bool JoinGroupShow { get { return _joinGroupShow; } set {if(_joinGroupShow != value) Set(ref _joinGroupShow, value, "JoinGroupShow"); } }
        public string JoinPlayerCountString { get { return _joinPlayerCountString; } set {if(_joinPlayerCountString != value) Set(ref _joinPlayerCountString, value, "JoinPlayerCountString"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private ChickenScoreView _view;
		
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
			var view = obj.GetComponent<ChickenScoreView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<ChickenScoreView>();
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
		private void EventTriggerBind(ChickenScoreView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static ChickenScoreViewModel()
        {
            Type type = typeof(ChickenScoreViewModel);
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

		void ViewBind(ChickenScoreView view)
		{
		     BindingSet<ChickenScoreView, ChickenScoreViewModel> bindingSet =
                view.CreateBindingSet<ChickenScoreView, ChickenScoreViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.TotalPlayerInfoShow).For(v => v.activeSelf).To(vm => vm.TotalPlayerInfoShow).OneWay();
            bindingSet.Bind(view.BeatGroupShow).For(v => v.activeSelf).To(vm => vm.BeatGroupShow).OneWay();
            bindingSet.Bind(view.BeatPlayerCountString).For(v => v.text).To(vm => vm.BeatPlayerCountString).OneWay();
            bindingSet.Bind(view.SurvivalGroupShow).For(v => v.activeSelf).To(vm => vm.SurvivalGroupShow).OneWay();
            bindingSet.Bind(view.SurvivalPlayerCountString).For(v => v.text).To(vm => vm.SurvivalPlayerCountString).OneWay();
            bindingSet.Bind(view.JoinGroupShow).For(v => v.activeSelf).To(vm => vm.JoinGroupShow).OneWay();
            bindingSet.Bind(view.JoinPlayerCountString).For(v => v.text).To(vm => vm.JoinPlayerCountString).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(ChickenScoreView view)
		{
            _show = view.Show.activeSelf;
            _totalPlayerInfoShow = view.TotalPlayerInfoShow.activeSelf;
            _beatGroupShow = view.BeatGroupShow.activeSelf;
            _beatPlayerCountString = view.BeatPlayerCountString.text;
            _survivalGroupShow = view.SurvivalGroupShow.activeSelf;
            _survivalPlayerCountString = view.SurvivalPlayerCountString.text;
            _joinGroupShow = view.JoinGroupShow.activeSelf;
            _joinPlayerCountString = view.JoinPlayerCountString.text;
		}


		void SaveOriData(ChickenScoreView view)
		{
            view.oriShow = _show;
            view.oriTotalPlayerInfoShow = _totalPlayerInfoShow;
            view.oriBeatGroupShow = _beatGroupShow;
            view.oriBeatPlayerCountString = _beatPlayerCountString;
            view.oriSurvivalGroupShow = _survivalGroupShow;
            view.oriSurvivalPlayerCountString = _survivalPlayerCountString;
            view.oriJoinGroupShow = _joinGroupShow;
            view.oriJoinPlayerCountString = _joinPlayerCountString;
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
			Show = _view.oriShow;
			TotalPlayerInfoShow = _view.oriTotalPlayerInfoShow;
			BeatGroupShow = _view.oriBeatGroupShow;
			BeatPlayerCountString = _view.oriBeatPlayerCountString;
			SurvivalGroupShow = _view.oriSurvivalGroupShow;
			SurvivalPlayerCountString = _view.oriSurvivalPlayerCountString;
			JoinGroupShow = _view.oriJoinGroupShow;
			JoinPlayerCountString = _view.oriJoinPlayerCountString;
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

        public string ResourceBundleName { get { return "ui/client/prefab/chicken"; } }
        public string ResourceAssetName { get { return "ChickenScore"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
