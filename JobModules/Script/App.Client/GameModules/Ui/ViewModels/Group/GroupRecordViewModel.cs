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
    public class GroupRecordViewModel : ViewModelBase, IUiViewModel
    {
        private class GroupRecordView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public Text CampScore1;
            [HideInInspector]
            public string oriCampScore1;
            public Text CampScore2;
            [HideInInspector]
            public string oriCampScore2;
            
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
                        case "CampScore1":
                            CampScore1 = v;
                            break;
                        case "CampScore2":
                            CampScore2 = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private string _campScore1;
        private string _campScore2;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public string CampScore1 { get { return _campScore1; } set {if(_campScore1 != value) Set(ref _campScore1, value, "CampScore1"); } }
        public string CampScore2 { get { return _campScore2; } set {if(_campScore2 != value) Set(ref _campScore2, value, "CampScore2"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private GroupRecordView _view;
		
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
			var view = obj.GetComponent<GroupRecordView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<GroupRecordView>();
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
		private void EventTriggerBind(GroupRecordView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static GroupRecordViewModel()
        {
            Type type = typeof(GroupRecordViewModel);
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

		void ViewBind(GroupRecordView view)
		{
		     BindingSet<GroupRecordView, GroupRecordViewModel> bindingSet =
                view.CreateBindingSet<GroupRecordView, GroupRecordViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.CampScore1).For(v => v.text).To(vm => vm.CampScore1).OneWay();
            bindingSet.Bind(view.CampScore2).For(v => v.text).To(vm => vm.CampScore2).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(GroupRecordView view)
		{
            _show = view.Show.activeSelf;
            _campScore1 = view.CampScore1.text;
            _campScore2 = view.CampScore2.text;
		}


		void SaveOriData(GroupRecordView view)
		{
            view.oriShow = _show;
            view.oriCampScore1 = _campScore1;
            view.oriCampScore2 = _campScore2;
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
			CampScore1 = _view.oriCampScore1;
			CampScore2 = _view.oriCampScore2;
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
        public string ResourceAssetName { get { return "GroupRecord"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
