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
    public class CommonHealthGroupViewModel : UIViewModelBase
    {
        private class CommonHealthGroupView : UIViewBase
        {
			public RectTransform rootRectTransform;
			public GameObject HpGroup;
			public GameObject HpGroupInHurt;
			public Slider currentHpSlider;
			[HideInInspector]
			public float oricurrentHpSlider;
			public Image curO2Image;
			public Image HelmetImage;
			public Image BulletproofImage;
			public Image PowerBarImageImage;
			public Image retreatBuffImage;
			public Image speedBuffImage;
			public UIText PercentTextUIText;
			public Image currentHpFillImage;
			public Image HpGroupInHurtFillImage;
			public Image HeadIconImage;
			public Image ClothIconImage;
			public GameObject ShowPoseGroup;
			public Image currentPoseImage;



			public override void FillField()
			{
				base.FillField();
				UIBind uibind = GetComponent<UIBind>();
				rootRectTransform =  uibind.AllObjs[0] as RectTransform;
				HpGroup =  uibind.AllObjs[1] as GameObject;
				HpGroupInHurt =  uibind.AllObjs[2] as GameObject;
				currentHpSlider =  uibind.AllObjs[3] as Slider;
				curO2Image =  uibind.AllObjs[4] as Image;
				HelmetImage =  uibind.AllObjs[5] as Image;
				BulletproofImage =  uibind.AllObjs[6] as Image;
				PowerBarImageImage =  uibind.AllObjs[7] as Image;
				retreatBuffImage =  uibind.AllObjs[8] as Image;
				speedBuffImage =  uibind.AllObjs[9] as Image;
				PercentTextUIText =  uibind.AllObjs[10] as UIText;
				currentHpFillImage =  uibind.AllObjs[11] as Image;
				HpGroupInHurtFillImage =  uibind.AllObjs[12] as Image;
				HeadIconImage =  uibind.AllObjs[13] as Image;
				ClothIconImage =  uibind.AllObjs[14] as Image;
				ShowPoseGroup =  uibind.AllObjs[15] as GameObject;
				currentPoseImage =  uibind.AllObjs[16] as Image;
            }

        }



			public RectTransform rootRectTransform { 
			get { return _view.rootRectTransform;} 
			}

			public GameObject HpGroup { 
			get { return _view.HpGroup;} 
			}

			public GameObject HpGroupInHurt { 
			get { return _view.HpGroupInHurt;} 
			}

			public float CurrentHpSliderValue { 
			get { return _view.currentHpSlider.value; }
			set { if(_view.currentHpSlider.value!=value) _view.currentHpSlider.value=value ; }
	}

			public Image curO2Image { 
			get { return _view.curO2Image;} 
			}

			public Image HelmetImage { 
			get { return _view.HelmetImage;} 
			}

			public Image BulletproofImage { 
			get { return _view.BulletproofImage;} 
			}

			public Image PowerBarImageImage { 
			get { return _view.PowerBarImageImage;} 
			}

			public Image retreatBuffImage { 
			get { return _view.retreatBuffImage;} 
			}

			public Image speedBuffImage { 
			get { return _view.speedBuffImage;} 
			}

			public UIText PercentTextUIText { 
			get { return _view.PercentTextUIText;} 
			}

			public Image currentHpFillImage { 
			get { return _view.currentHpFillImage;} 
			}

			public Image HpGroupInHurtFillImage { 
			get { return _view.HpGroupInHurtFillImage;} 
			}

			public Image HeadIconImage { 
			get { return _view.HeadIconImage;} 
			}

			public Image ClothIconImage { 
			get { return _view.ClothIconImage;} 
			}

			public GameObject ShowPoseGroup { 
			get { return _view.ShowPoseGroup;} 
			}

			public Image currentPoseImage { 
			get { return _view.currentPoseImage;} 
			}





		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonHealthGroupView _view;
		
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
			var view = obj.GetComponent<CommonHealthGroupView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonHealthGroupView>();
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
		
		private void EventTriggerBind(CommonHealthGroupView view)
		{

		

		}
		

		
		

		void DataInit(CommonHealthGroupView view)
		{
		}


		void SaveOriData(CommonHealthGroupView view)
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
        public override string ResourceAssetName { get { return "CommonHealthGroup"; } }
        public override string ConfigBundleName { get { return ""; } }
        public override string ConfigAssetName { get { return ""; } }
    }
}
