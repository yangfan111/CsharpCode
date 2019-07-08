using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using App.Shared.Components.Player;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class PaintUiAdapter : UIAdapter, IPaintUiAdapter
    {

        private Contexts _contexts;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(PaintUiAdapter));
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

        public bool FreshSelectedPaintIndex
        {
            get
            {
                return _contexts.ui.uI.FreshSelectedPaintIndex;
            }
            set
            {
                _contexts.ui.uI.FreshSelectedPaintIndex = value;
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
            var paintIdList = PaintIdList;
            if (paintIdList.Count <= SelectedPaintIndex) {
                _logger.ErrorFormat("Give me an error SelectedPaintIndex, Please check it !");
                return;
            }
            int id = paintIdList[SelectedPaintIndex];
            _logger.DebugFormat("id : " + id);
        }

        public override bool Enable
        {
            get { return CanOpen && _enable; }
        }

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }

        public void Select()
        {
            if (!FreshSelectedPaintIndex)
                return;
            var paintIdList = PaintIdList;
            SelectedPaintIndex = SelectValidIndex(paintIdList);
            FreshSelectedPaintIndex = false;
        }

        public static int SelectValidIndex(List<int> list) {
            for (int i = 0, maxi = list.Count; i < maxi; i++)
            {
                if (list[i] > 0)
                {
                    return i;
                }
            }
            return 0;
        }

        public GamePlayComponent gamePlay
        {
            get
            {
                return _contexts.player.flagSelfEntity.gamePlay;
            }
        }
    }

}
