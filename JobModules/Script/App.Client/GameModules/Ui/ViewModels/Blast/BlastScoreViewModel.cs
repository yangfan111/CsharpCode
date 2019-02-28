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

namespace App.Client.GameModules.Ui.ViewModels.Blast
{
    public class BlastScoreViewModel : ViewModelBase, IUiViewModel
    {
        private class BlastScoreView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text TimeText;
            [HideInInspector]
            public string oriTimeText;
            public Text TimeColor;
            [HideInInspector]
            public Color oriTimeColor;
            public Text ScoreText;
            [HideInInspector]
            public string oriScoreText;
            public Text CampKillCountText1;
            [HideInInspector]
            public string oriCampKillCountText1;
            public Text CampKillCountText2;
            [HideInInspector]
            public string oriCampKillCountText2;
            public GameObject C4TipGroupShow;
            [HideInInspector]
            public bool oriC4TipGroupShow;
            public Image C4TipValue;
            [HideInInspector]
            public float oriC4TipValue;
            
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
                        case "C4TipGroup":
                            C4TipGroupShow = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Time":
                            TimeText = v;
                            TimeColor = v;
                            break;
                        case "ScoreText":
                            ScoreText = v;
                            break;
                        case "CampKillCount1":
                            CampKillCountText1 = v;
                            break;
                        case "CampKillCount2":
                            CampKillCountText2 = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "C4TipMask":
                            C4TipValue = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _timeText;
        private Color _timeColor;
        private string _scoreText;
        private string _campKillCountText1;
        private string _campKillCountText2;
        private bool _c4TipGroupShow;
        private float _c4TipValue;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string TimeText { get { return _timeText; } set {if(_timeText != value) Set(ref _timeText, value, "TimeText"); } }
        public Color TimeColor { get { return _timeColor; } set {if(_timeColor != value) Set(ref _timeColor, value, "TimeColor"); } }
        public string ScoreText { get { return _scoreText; } set {if(_scoreText != value) Set(ref _scoreText, value, "ScoreText"); } }
        public string CampKillCountText1 { get { return _campKillCountText1; } set {if(_campKillCountText1 != value) Set(ref _campKillCountText1, value, "CampKillCountText1"); } }
        public string CampKillCountText2 { get { return _campKillCountText2; } set {if(_campKillCountText2 != value) Set(ref _campKillCountText2, value, "CampKillCountText2"); } }
        public bool C4TipGroupShow { get { return _c4TipGroupShow; } set {if(_c4TipGroupShow != value) Set(ref _c4TipGroupShow, value, "C4TipGroupShow"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float C4TipValue { get { return _c4TipValue; } set {if(_c4TipValue != value) Set(ref _c4TipValue, value, "C4TipValue"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private BlastScoreView _view;
		
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

			var view = obj.GetComponent<BlastScoreView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<BlastScoreView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<BlastScoreView, BlastScoreViewModel> bindingSet =
                view.CreateBindingSet<BlastScoreView, BlastScoreViewModel>();

            view.oriShow = _show = view.Show.activeSelf;
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            view.oriTimeText = _timeText = view.TimeText.text;
            bindingSet.Bind(view.TimeText).For(v => v.text).To(vm => vm.TimeText).OneWay();
            view.oriTimeColor = _timeColor = view.TimeColor.color;
            bindingSet.Bind(view.TimeColor).For(v => v.color).To(vm => vm.TimeColor).OneWay();
            view.oriScoreText = _scoreText = view.ScoreText.text;
            bindingSet.Bind(view.ScoreText).For(v => v.text).To(vm => vm.ScoreText).OneWay();
            view.oriCampKillCountText1 = _campKillCountText1 = view.CampKillCountText1.text;
            bindingSet.Bind(view.CampKillCountText1).For(v => v.text).To(vm => vm.CampKillCountText1).OneWay();
            view.oriCampKillCountText2 = _campKillCountText2 = view.CampKillCountText2.text;
            bindingSet.Bind(view.CampKillCountText2).For(v => v.text).To(vm => vm.CampKillCountText2).OneWay();
            view.oriC4TipGroupShow = _c4TipGroupShow = view.C4TipGroupShow.activeSelf;
            bindingSet.Bind(view.C4TipGroupShow).For(v => v.activeSelf).To(vm => vm.C4TipGroupShow).OneWay();
            view.oriC4TipValue = _c4TipValue = view.C4TipValue.fillAmount;
            bindingSet.Bind(view.C4TipValue).For(v => v.fillAmount).To(vm => vm.C4TipValue).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(BlastScoreView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static BlastScoreViewModel()
        {
            Type type = typeof(BlastScoreViewModel);
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
			TimeText = _view.oriTimeText;
			TimeColor = _view.oriTimeColor;
			ScoreText = _view.oriScoreText;
			CampKillCountText1 = _view.oriCampKillCountText1;
			CampKillCountText2 = _view.oriCampKillCountText2;
			C4TipGroupShow = _view.oriC4TipGroupShow;
			C4TipValue = _view.oriC4TipValue;
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

        public string ResourceBundleName { get { return "uiprefabs/blast"; } }
        public string ResourceAssetName { get { return "BlastScore"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
