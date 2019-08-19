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
    public class CommonBlastTipsViewModel : UIViewModelBase
    {
        private class CommonBlastTipsView : UIViewBase
        {
			public GameObject subrootA;
			public GameObject subrootB;
			public GameObject subrootC4;



			public override void FillField()
			{
				base.FillField();
				UIBind uibind = GetComponent<UIBind>();
				subrootA =  uibind.AllObjs[0] as GameObject;
				subrootB =  uibind.AllObjs[1] as GameObject;
				subrootC4 =  uibind.AllObjs[2] as GameObject;
            }

        }



			public GameObject subrootA { 
			get { return _view.subrootA;} 
			}

			public GameObject subrootB { 
			get { return _view.subrootB;} 
			}

			public GameObject subrootC4 { 
			get { return _view.subrootC4;} 
			}





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonBlastTipsView _view;
		
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
			var view = obj.GetComponent<CommonBlastTipsView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonBlastTipsView>();
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
		
		private void EventTriggerBind(CommonBlastTipsView view)
		{

		

		}
		

		
		

		void DataInit(CommonBlastTipsView view)
		{
		}


		void SaveOriData(CommonBlastTipsView view)
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
        public override string ResourceAssetName { get { return "CommonBlastTips"; } }
        public override string ConfigBundleName { get { return ""; } }
        public override string ConfigAssetName { get { return ""; } }
    }
}
