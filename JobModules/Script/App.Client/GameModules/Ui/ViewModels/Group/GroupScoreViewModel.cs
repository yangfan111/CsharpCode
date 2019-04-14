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
    public class GroupScoreViewModel : ViewModelBase, IUiViewModel
    {
        private class GroupScoreView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text WinContionText;
            [HideInInspector]
            public string oriWinContionText;
            public Text WinContionColor;
            [HideInInspector]
            public Color oriWinContionColor;
            public Text CampKillCountText1;
            [HideInInspector]
            public string oriCampKillCountText1;
            public Text CampKillCountText2;
            [HideInInspector]
            public string oriCampKillCountText2;
            
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
                        case "WinContion":
                            WinContionText = v;
                            WinContionColor = v;
                            break;
                        case "CampKillCount1":
                            CampKillCountText1 = v;
                            break;
                        case "CampKillCount2":
                            CampKillCountText2 = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _winContionText;
        private Color _winContionColor;
        private string _campKillCountText1;
        private string _campKillCountText2;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string WinContionText { get { return _winContionText; } set {if(_winContionText != value) Set(ref _winContionText, value, "WinContionText"); } }
        public Color WinContionColor { get { return _winContionColor; } set {if(_winContionColor != value) Set(ref _winContionColor, value, "WinContionColor"); } }
        public string CampKillCountText1 { get { return _campKillCountText1; } set {if(_campKillCountText1 != value) Set(ref _campKillCountText1, value, "CampKillCountText1"); } }
        public string CampKillCountText2 { get { return _campKillCountText2; } set {if(_campKillCountText2 != value) Set(ref _campKillCountText2, value, "CampKillCountText2"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private GroupScoreView _view;
		
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
			var view = obj.GetComponent<GroupScoreView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<GroupScoreView>();
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
		private void EventTriggerBind(GroupScoreView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static GroupScoreViewModel()
        {
            Type type = typeof(GroupScoreViewModel);
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

		void ViewBind(GroupScoreView view)
		{
		     BindingSet<GroupScoreView, GroupScoreViewModel> bindingSet =
                view.CreateBindingSet<GroupScoreView, GroupScoreViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.WinContionText).For(v => v.text).To(vm => vm.WinContionText).OneWay();
            bindingSet.Bind(view.WinContionColor).For(v => v.color).To(vm => vm.WinContionColor).OneWay();
            bindingSet.Bind(view.CampKillCountText1).For(v => v.text).To(vm => vm.CampKillCountText1).OneWay();
            bindingSet.Bind(view.CampKillCountText2).For(v => v.text).To(vm => vm.CampKillCountText2).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(GroupScoreView view)
		{
            _show = view.Show.activeSelf;
            _winContionText = view.WinContionText.text;
            _winContionColor = view.WinContionColor.color;
            _campKillCountText1 = view.CampKillCountText1.text;
            _campKillCountText2 = view.CampKillCountText2.text;
		}


		void SaveOriData(GroupScoreView view)
		{
            view.oriShow = _show;
            view.oriWinContionText = _winContionText;
            view.oriWinContionColor = _winContionColor;
            view.oriCampKillCountText1 = _campKillCountText1;
            view.oriCampKillCountText2 = _campKillCountText2;
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
			WinContionText = _view.oriWinContionText;
			WinContionColor = _view.oriWinContionColor;
			CampKillCountText1 = _view.oriCampKillCountText1;
			CampKillCountText2 = _view.oriCampKillCountText2;
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
        public string ResourceAssetName { get { return "GroupScore"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
