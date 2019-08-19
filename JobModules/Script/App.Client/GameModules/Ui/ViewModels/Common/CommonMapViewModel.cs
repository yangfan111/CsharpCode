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
    public class CommonMapViewModel : UIViewModelBase
    {
        private class CommonMapView : UIViewBase
        {
			public UIText MapNameUIText;
			public UIText RoomInfoUIText;
			public UIText NumberUIText;
			public UIText WinConditionUIText;
			public UIText TeamUIText;
			public GameObject Tip;
			public GameObject MapRoot;



			public override void FillField()
			{
				base.FillField();
				UIBind uibind = GetComponent<UIBind>();
				MapNameUIText =  uibind.AllObjs[0] as UIText;
				RoomInfoUIText =  uibind.AllObjs[1] as UIText;
				NumberUIText =  uibind.AllObjs[2] as UIText;
				WinConditionUIText =  uibind.AllObjs[3] as UIText;
				TeamUIText =  uibind.AllObjs[4] as UIText;
				Tip =  uibind.AllObjs[5] as GameObject;
				MapRoot =  uibind.AllObjs[6] as GameObject;
            }

        }



			public UIText MapNameUIText { 
			get { return _view.MapNameUIText;} 
			}

			public UIText RoomInfoUIText { 
			get { return _view.RoomInfoUIText;} 
			}

			public UIText NumberUIText { 
			get { return _view.NumberUIText;} 
			}

			public UIText WinConditionUIText { 
			get { return _view.WinConditionUIText;} 
			}

			public UIText TeamUIText { 
			get { return _view.TeamUIText;} 
			}

			public GameObject Tip { 
			get { return _view.Tip;} 
			}

			public GameObject MapRoot { 
			get { return _view.MapRoot;} 
			}





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMapView _view;
		
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
			var view = obj.GetComponent<CommonMapView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonMapView>();
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
		
		private void EventTriggerBind(CommonMapView view)
		{

		

		}
		

		
		

		void DataInit(CommonMapView view)
		{
		}


		void SaveOriData(CommonMapView view)
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
        public override string ResourceAssetName { get { return "CommonMap"; } }
        public override string ConfigBundleName { get { return ""; } }
        public override string ConfigAssetName { get { return ""; } }
    }
}
