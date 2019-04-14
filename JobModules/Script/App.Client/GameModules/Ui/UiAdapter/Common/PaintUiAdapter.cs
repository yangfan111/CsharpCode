using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class PaintUiAdapter : UIAdapter, IPaintUiAdapter
    {

        private Contexts _contexts;
        public PaintUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }
        public List<int> PaintIdList
        {
            get
            {
                return _contexts.ui.uI.PaintIdList;
            }
        }

        public int SelectedPaintIndex
        {
            get
            {
                return _contexts.ui.uI.SelectedPaintIndex;
            }
            set
            {
                _contexts.ui.uI.SelectedPaintIndex = value;
            }
        }

        public bool CanOpen
        {
            get
            {
                return true;
            }
        }

        public void Paint()
        {
            Debug.Log("Paint" + SelectedPaintIndex);
        }

        public override bool Enable
        {
            get { return CanOpen && _enable; }
        }

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }

    }

}
