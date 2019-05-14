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
    public class CommonMiniMapViewModel : IUiViewModel
    {
        private class CommonMiniMapView : MonoBehaviour
        {
			public RectTransform rootRectTransform;
			public UIEventTriggerListener BgUIEventTriggerListener;
			public RectTransform BgRectTransform;
			public RectTransform mapRectTransform;



			public void FillField()
			{
				UIBind uibind = GetComponent<UIBind>();
				rootRectTransform =  uibind.AllObjs[0] as RectTransform;
				BgUIEventTriggerListener =  uibind.AllObjs[1] as UIEventTriggerListener;
				BgRectTransform =  uibind.AllObjs[2] as RectTransform;
				mapRectTransform =  uibind.AllObjs[3] as RectTransform;
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





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMiniMapView _view;
		
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

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			
			SpriteReset();
		}

		public bool IsPropertyExist(string name)
        {
            return false;
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
        public string ResourceAssetName { get { return "CommonMiniMap"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
