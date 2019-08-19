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

namespace App.Client.GameModules.Ui.ViewModels.Blast
{
    public class BlastC4TipViewModel : ViewModelBase, IUiViewModel
    {
        private class BlastC4TipView : UIView
        {
            public Image C4TipValue;
            [HideInInspector]
            public float oriC4TipValue;
            
            public void FillField()
            {
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


        private float _c4TipValue;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float C4TipValue { get { return _c4TipValue; } set {if(_c4TipValue != value) Set(ref _c4TipValue, value, "C4TipValue"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private BlastC4TipView _view;
		
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
			var view = obj.GetComponent<BlastC4TipView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<BlastC4TipView>();
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
		private void EventTriggerBind(BlastC4TipView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static BlastC4TipViewModel()
        {
            Type type = typeof(BlastC4TipViewModel);
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

		void ViewBind(BlastC4TipView view)
		{
		     BindingSet<BlastC4TipView, BlastC4TipViewModel> bindingSet =
                view.CreateBindingSet<BlastC4TipView, BlastC4TipViewModel>();
            bindingSet.Bind(view.C4TipValue).For(v => v.fillAmount).To(vm => vm.C4TipValue).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(BlastC4TipView view)
		{
            _c4TipValue = view.C4TipValue.fillAmount;
		}


		void SaveOriData(BlastC4TipView view)
		{
            view.oriC4TipValue = _c4TipValue;
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

        public string ResourceBundleName { get { return "ui/client/prefab/blast"; } }
        public string ResourceAssetName { get { return "BlastC4Tip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
