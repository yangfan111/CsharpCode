using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class LoadingUiAdapter : UIAdapter, ILoadingUiAdapter
    {
        private Contexts _contexts;
        public LoadingUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        private float curValue = 0;
        public float CurValue
        {
            get { return _contexts.ui.uI.LoadingRate; }         
            set { curValue = value; }
        }                                           

        private string curText = "";        
        public string CurText
        {
            get { return _contexts.ui.uI.LoadingText; }
            set { curText = value; }
        }
    }
}