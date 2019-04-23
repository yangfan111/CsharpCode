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
    public class ChickenPlaneViewModel : ViewModelBase, IUiViewModel
    {
        private class ChickenPlaneView : UIView
        {
            public Text CurCount;
            [HideInInspector]
            public string oriCurCount;
            public Text TotalCount;
            [HideInInspector]
            public string oriTotalCount;
            
            public void FillField()
            {
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "CurCount":
                            CurCount = v;
                            break;
                        case "TotalCount":
                            TotalCount = v;
                            break;
                    }
                }

            }
        }


        private string _curCount;
        private string _totalCount;
        public string CurCount { get { return _curCount; } set {if(_curCount != value) Set(ref _curCount, value, "CurCount"); } }
        public string TotalCount { get { return _totalCount; } set {if(_totalCount != value) Set(ref _totalCount, value, "TotalCount"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private ChickenPlaneView _view;
		
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
			var view = obj.GetComponent<ChickenPlaneView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<ChickenPlaneView>();
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
		private void EventTriggerBind(ChickenPlaneView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static ChickenPlaneViewModel()
        {
            Type type = typeof(ChickenPlaneViewModel);
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

		void ViewBind(ChickenPlaneView view)
		{
		     BindingSet<ChickenPlaneView, ChickenPlaneViewModel> bindingSet =
                view.CreateBindingSet<ChickenPlaneView, ChickenPlaneViewModel>();
            bindingSet.Bind(view.CurCount).For(v => v.text).To(vm => vm.CurCount).OneWay();
            bindingSet.Bind(view.TotalCount).For(v => v.text).To(vm => vm.TotalCount).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(ChickenPlaneView view)
		{
            _curCount = view.CurCount.text;
            _totalCount = view.TotalCount.text;
		}


		void SaveOriData(ChickenPlaneView view)
		{
            view.oriCurCount = _curCount;
            view.oriTotalCount = _totalCount;
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
			CurCount = _view.oriCurCount;
			TotalCount = _view.oriTotalCount;
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
        public string ResourceAssetName { get { return "ChickenPlane"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
