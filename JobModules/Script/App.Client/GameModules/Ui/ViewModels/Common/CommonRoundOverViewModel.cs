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
    public class CommonRoundOverViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonRoundOverView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public GameObject AnimeGroupShow;
            [HideInInspector]
            public bool oriAnimeGroupShow;
            public GameObject RoundGroupShow;
            [HideInInspector]
            public bool oriRoundGroupShow;
            public Text TitleText;
            [HideInInspector]
            public string oriTitleText;
            public Text CampScoreText1;
            [HideInInspector]
            public string oriCampScoreText1;
            public Text CampScoreText2;
            [HideInInspector]
            public string oriCampScoreText2;
            
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
                        case "AnimeGroup":
                            AnimeGroupShow = v.gameObject;
                            break;
                        case "RoundGroup":
                            RoundGroupShow = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Title":
                            TitleText = v;
                            break;
                        case "Left":
                            CampScoreText1 = v;
                            break;
                        case "Right":
                            CampScoreText2 = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private bool _animeGroupShow;
        private bool _roundGroupShow;
        private string _titleText;
        private string _campScoreText1;
        private string _campScoreText2;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public bool AnimeGroupShow { get { return _animeGroupShow; } set {if(_animeGroupShow != value) Set(ref _animeGroupShow, value, "AnimeGroupShow"); } }
        public bool RoundGroupShow { get { return _roundGroupShow; } set {if(_roundGroupShow != value) Set(ref _roundGroupShow, value, "RoundGroupShow"); } }
        public string TitleText { get { return _titleText; } set {if(_titleText != value) Set(ref _titleText, value, "TitleText"); } }
        public string CampScoreText1 { get { return _campScoreText1; } set {if(_campScoreText1 != value) Set(ref _campScoreText1, value, "CampScoreText1"); } }
        public string CampScoreText2 { get { return _campScoreText2; } set {if(_campScoreText2 != value) Set(ref _campScoreText2, value, "CampScoreText2"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonRoundOverView _view;
		
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
			var view = obj.GetComponent<CommonRoundOverView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonRoundOverView>();
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
		private void EventTriggerBind(CommonRoundOverView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonRoundOverViewModel()
        {
            Type type = typeof(CommonRoundOverViewModel);
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

		void ViewBind(CommonRoundOverView view)
		{
		     BindingSet<CommonRoundOverView, CommonRoundOverViewModel> bindingSet =
                view.CreateBindingSet<CommonRoundOverView, CommonRoundOverViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.AnimeGroupShow).For(v => v.activeSelf).To(vm => vm.AnimeGroupShow).OneWay();
            bindingSet.Bind(view.RoundGroupShow).For(v => v.activeSelf).To(vm => vm.RoundGroupShow).OneWay();
            bindingSet.Bind(view.TitleText).For(v => v.text).To(vm => vm.TitleText).OneWay();
            bindingSet.Bind(view.CampScoreText1).For(v => v.text).To(vm => vm.CampScoreText1).OneWay();
            bindingSet.Bind(view.CampScoreText2).For(v => v.text).To(vm => vm.CampScoreText2).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonRoundOverView view)
		{
            _show = view.Show.activeSelf;
            _animeGroupShow = view.AnimeGroupShow.activeSelf;
            _roundGroupShow = view.RoundGroupShow.activeSelf;
            _titleText = view.TitleText.text;
            _campScoreText1 = view.CampScoreText1.text;
            _campScoreText2 = view.CampScoreText2.text;
		}


		void SaveOriData(CommonRoundOverView view)
		{
            view.oriShow = _show;
            view.oriAnimeGroupShow = _animeGroupShow;
            view.oriRoundGroupShow = _roundGroupShow;
            view.oriTitleText = _titleText;
            view.oriCampScoreText1 = _campScoreText1;
            view.oriCampScoreText2 = _campScoreText2;
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
			AnimeGroupShow = _view.oriAnimeGroupShow;
			RoundGroupShow = _view.oriRoundGroupShow;
			TitleText = _view.oriTitleText;
			CampScoreText1 = _view.oriCampScoreText1;
			CampScoreText2 = _view.oriCampScoreText2;
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
        public string ResourceAssetName { get { return "CommonRoundOver"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
