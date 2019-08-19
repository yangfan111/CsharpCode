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
    public class CommonMaxMapViewModel : UIViewModelBase
    {
        private class CommonMaxMapView : UIViewBase
        {
			public UIText MapNameUIText;
			public UIText RoomInfoUIText;
			public UIText NumberUIText;
			public UIText WinConditionUIText;
			public UIText TeamUIText;
			public UIText TipUIText;
			public GameObject MapRoot;
			public GameObject RoomInfoGroup;
			public GameObject NumberGroup;
			public GameObject WinConditionGroup;
			public GameObject TeamGroup;
			public GameObject TipGroup;
			public VerticalLayoutGroup GroupVerticalLayoutGroup;
			public VerticalLayoutGroup RoomInfoGroupVerticalLayoutGroup;
			public VerticalLayoutGroup WinConditionGroupVerticalLayoutGroup;



			public override void FillField()
			{
				base.FillField();
				UIBind uibind = GetComponent<UIBind>();
				MapNameUIText =  uibind.AllObjs[0] as UIText;
				RoomInfoUIText =  uibind.AllObjs[1] as UIText;
				NumberUIText =  uibind.AllObjs[2] as UIText;
				WinConditionUIText =  uibind.AllObjs[3] as UIText;
				TeamUIText =  uibind.AllObjs[4] as UIText;
				TipUIText =  uibind.AllObjs[5] as UIText;
				MapRoot =  uibind.AllObjs[6] as GameObject;
				RoomInfoGroup =  uibind.AllObjs[7] as GameObject;
				NumberGroup =  uibind.AllObjs[8] as GameObject;
				WinConditionGroup =  uibind.AllObjs[9] as GameObject;
				TeamGroup =  uibind.AllObjs[10] as GameObject;
				TipGroup =  uibind.AllObjs[11] as GameObject;
				GroupVerticalLayoutGroup =  uibind.AllObjs[12] as VerticalLayoutGroup;
				RoomInfoGroupVerticalLayoutGroup =  uibind.AllObjs[13] as VerticalLayoutGroup;
				WinConditionGroupVerticalLayoutGroup =  uibind.AllObjs[14] as VerticalLayoutGroup;
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

			public UIText TipUIText { 
			get { return _view.TipUIText;} 
			}

			public GameObject MapRoot { 
			get { return _view.MapRoot;} 
			}

			public GameObject RoomInfoGroup { 
			get { return _view.RoomInfoGroup;} 
			}

			public GameObject NumberGroup { 
			get { return _view.NumberGroup;} 
			}

			public GameObject WinConditionGroup { 
			get { return _view.WinConditionGroup;} 
			}

			public GameObject TeamGroup { 
			get { return _view.TeamGroup;} 
			}

			public GameObject TipGroup { 
			get { return _view.TipGroup;} 
			}

			public VerticalLayoutGroup GroupVerticalLayoutGroup { 
			get { return _view.GroupVerticalLayoutGroup;} 
			}

			public VerticalLayoutGroup RoomInfoGroupVerticalLayoutGroup { 
			get { return _view.RoomInfoGroupVerticalLayoutGroup;} 
			}

			public VerticalLayoutGroup WinConditionGroupVerticalLayoutGroup { 
			get { return _view.WinConditionGroupVerticalLayoutGroup;} 
			}





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonMaxMapView _view;
		
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
			var view = obj.GetComponent<CommonMaxMapView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonMaxMapView>();
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
		
		private void EventTriggerBind(CommonMaxMapView view)
		{

		

		}
		

		
		

		void DataInit(CommonMaxMapView view)
		{
		}


		void SaveOriData(CommonMaxMapView view)
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
        public override string ResourceAssetName { get { return "CommonMaxMap"; } }
        public override string ConfigBundleName { get { return ""; } }
        public override string ConfigAssetName { get { return ""; } }
    }
}
