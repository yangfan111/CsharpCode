using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.UiFramework.Libs;
using UnityEngine.UI;
using UIComponent.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonLocateViewModel : UIViewModelBase
    {
        private class CommonLocateView : UIViewBase
        {
			public RawImage RulerRawImage;
			public GameObject Show;
			public GameObject Mark;
			public RectTransform markRootRectTransform;
			public UIText AngelTextUIText;
			[HideInInspector]
			public string oriAngelTextUIText;
			public GameObject markRoot;



			public override void FillField()
			{
				base.FillField();
				UIBind uibind = GetComponent<UIBind>();
				RulerRawImage =  uibind.AllObjs[0] as RawImage;
				Show =  uibind.AllObjs[1] as GameObject;
				Mark =  uibind.AllObjs[2] as GameObject;
				markRootRectTransform =  uibind.AllObjs[3] as RectTransform;
				AngelTextUIText =  uibind.AllObjs[4] as UIText;
				markRoot =  uibind.AllObjs[5] as GameObject;
            }

        }



			public RawImage RulerRawImage { 
			get { return _view.RulerRawImage;} 
			}

			public GameObject Show { 
			get { return _view.Show;} 
			}

			public GameObject Mark { 
			get { return _view.Mark;} 
			}

			public RectTransform markRootRectTransform { 
			get { return _view.markRootRectTransform;} 
			}

			public string AngelTextUITextText { 
			get { return _view.AngelTextUIText.text; }
			set { if(_view.AngelTextUIText.text!=value) _view.AngelTextUIText.text=value ; }
	}

			public GameObject markRoot { 
			get { return _view.markRoot;} 
			}





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonLocateView _view;
		
		public override void Destory()
        {
            if (_viewGameObject != null)
            {
				UnityEngine.Object.Destroy(_viewGameObject);
            }
        }
		public override void Visible(bool isViaible)
		{
		    if (_viewGameObject != null)
            {
				_viewGameObject.SetActive(isViaible);
            }
		
		}
		public override void SetCanvasEnabled(bool value)
        {
            if (_viewCanvas != null)
            {
                _viewCanvas.enabled = value;
            }
        }
        public override void CreateBinding(GameObject obj)
        {
			_viewGameObject = obj;
			_viewCanvas = _viewGameObject.GetComponent<Canvas>();

			bool bFirst = false;
			var view = obj.GetComponent<CommonLocateView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonLocateView>();
				view.FillField();
			}
			DataInit(view);
			SpriteReset();

			if(bFirst)
			{
				SaveOriData(view);
			}
			_view = view;
			viewBase = view;

			
        }
		
		private void EventTriggerBind(CommonLocateView view)
		{

		

		}
		

		
		

		void DataInit(CommonLocateView view)
		{
		}


		void SaveOriData(CommonLocateView view)
		{
        
		}




		private void SpriteReset()
		{
			
		}

		public override void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			
			SpriteReset();
		}

		public override bool IsPropertyExist(string name)
        {
            return false;
        }

        public override Transform GetParentLinkNode()
        {
            return null;
        }

        public override Transform GetChildLinkNode()
        {
            return null;
        }



       

        public override string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public override string ResourceAssetName { get { return "CommonLocate"; } }
        public override string ConfigBundleName { get { return ""; } }
        public override string ConfigAssetName { get { return ""; } }
    }
}
