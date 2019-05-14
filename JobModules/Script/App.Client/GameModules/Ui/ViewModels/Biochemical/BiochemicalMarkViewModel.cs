using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.UiFramework.Libs;
using UnityEngine.UI;
using UIComponent.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace App.Client.GameModules.Ui.ViewModels.Biochemical
{
    public class BiochemicalMarkViewModel : IUiViewModel
    {
        private class BiochemicalMarkView : MonoBehaviour
        {



			public void FillField()
			{
				UIBind uibind = GetComponent<UIBind>();
            }

        }







		
		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private BiochemicalMarkView _view;
		
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
			var view = obj.GetComponent<BiochemicalMarkView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<BiochemicalMarkView>();
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
		
		private void EventTriggerBind(BiochemicalMarkView view)
		{

		

		}
		

		
		

		void DataInit(BiochemicalMarkView view)
		{
		}


		void SaveOriData(BiochemicalMarkView view)
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



       

        public string ResourceBundleName { get { return "ui/client/prefab/biochemical"; } }
        public string ResourceAssetName { get { return "BiochemicalMark"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
