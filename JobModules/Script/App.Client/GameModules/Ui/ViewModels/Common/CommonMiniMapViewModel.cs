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
    public class CommonMiniMapViewModel : UIViewModelBase
    {
        private class CommonMiniMapView : UIViewBase
        {
			public RectTransform rootRectTransform;
			public UIEventTriggerListener BgUIEventTriggerListener;
			public RectTransform BgRectTransform;
			public RectTransform mapRectTransform;
			public GameObject CommonMiniMap;



			public override void FillField()
			{
				base.FillField();
				UIBind uibind = GetComponent<UIBind>();
				rootRectTransform =  uibind.AllObjs[0] as RectTransform;
				BgUIEventTriggerListener =  uibind.AllObjs[1] as UIEventTriggerListener;
				BgRectTransform =  uibind.AllObjs[2] as RectTransform;
				mapRectTransform =  uibind.AllObjs[3] as RectTransform;
				CommonMiniMap =  uibind.AllObjs[4] as GameObject;
            }

        }



			public RectTransform rootRectTransform { 
			get { return _view.rootRectTransform;} 
			}

			public UIEventTriggerListener BgUIEventTriggerListener { 
			get { return _view.BgUIEventTriggerListener;} 
			}

			public RectTransform BgRectTransform { 
			get { return _view.BgRectTransform;} 
			}

			public RectTransform mapRectTransform { 
			get { return _view.mapRectTransform;} 
			}

			public GameObject CommonMiniMap { 
			get { return _view.CommonMiniMap;} 
			}





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMiniMapView _view;
		
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
			var view = obj.GetComponent<CommonMiniMapView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonMiniMapView>();
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
		
		private void EventTriggerBind(CommonMiniMapView view)
		{

		

		}
		

		
		

		void DataInit(CommonMiniMapView view)
		{
		}


		void SaveOriData(CommonMiniMapView view)
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
        public override string ResourceAssetName { get { return "CommonMiniMap"; } }
        public override string ConfigBundleName { get { return ""; } }
        public override string ConfigAssetName { get { return ""; } }
    }
}
