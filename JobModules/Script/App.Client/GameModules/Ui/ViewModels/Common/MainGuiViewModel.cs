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
    public class MainGuiViewModel : ViewModelBase, IUiViewModel
    {
        private class MainGuiView : UIView
        {
            public GameObject ShowGameObjectActiveSelf;
            [HideInInspector]
            public bool oriShowGameObjectActiveSelf;
            public Slider HpVal;
            [HideInInspector]
            public float oriHpVal;
            public Text HpNum;
            [HideInInspector]
            public string oriHpNum;
            public Text BulletCount;
            [HideInInspector]
            public string oriBulletCount;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            ShowGameObjectActiveSelf = v.gameObject;
                            break;
                    }
                }

                Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
                foreach (var v in sliders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "HpVal":
                            HpVal = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "HpNum":
                            HpNum = v;
                            break;
                        case "BulletNum":
                            BulletCount = v;
                            break;
                    }
                }

            }
        }


        private bool _showGameObjectActiveSelf;
        private float _hpVal;
        private string _hpNum;
        private string _bulletCount;
        public bool ShowGameObjectActiveSelf { get { return _showGameObjectActiveSelf; } set {if(_showGameObjectActiveSelf != value) Set(ref _showGameObjectActiveSelf, value, "ShowGameObjectActiveSelf"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float HpVal { get { return _hpVal; } set {if(_hpVal != value) Set(ref _hpVal, value, "HpVal"); } }
        public string HpNum { get { return _hpNum; } set {if(_hpNum != value) Set(ref _hpNum, value, "HpNum"); } }
        public string BulletCount { get { return _bulletCount; } set {if(_bulletCount != value) Set(ref _bulletCount, value, "BulletCount"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private MainGuiView _view;
		
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

			var view = obj.GetComponent<MainGuiView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<MainGuiView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<MainGuiView, MainGuiViewModel> bindingSet =
                view.CreateBindingSet<MainGuiView, MainGuiViewModel>();

            view.oriShowGameObjectActiveSelf = _showGameObjectActiveSelf = view.ShowGameObjectActiveSelf.activeSelf;
            bindingSet.Bind(view.ShowGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.ShowGameObjectActiveSelf).OneWay();
            view.oriHpVal = _hpVal = view.HpVal.value;
            bindingSet.Bind(view.HpVal).For(v => v.value).To(vm => vm.HpVal).OneWay();
            view.oriHpNum = _hpNum = view.HpNum.text;
            bindingSet.Bind(view.HpNum).For(v => v.text).To(vm => vm.HpNum).OneWay();
            view.oriBulletCount = _bulletCount = view.BulletCount.text;
            bindingSet.Bind(view.BulletCount).For(v => v.text).To(vm => vm.BulletCount).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(MainGuiView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static MainGuiViewModel()
        {
            Type type = typeof(MainGuiViewModel);
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
			ShowGameObjectActiveSelf = _view.oriShowGameObjectActiveSelf;
			HpVal = _view.oriHpVal;
			HpNum = _view.oriHpNum;
			BulletCount = _view.oriBulletCount;
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
        public string ResourceAssetName { get { return "MainGUI"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
