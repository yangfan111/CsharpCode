using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class NoticeUiAdapter : UIAdapter, INoticeUiAdapter
    {
        private Contexts _contexts;
        public NoticeUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public INoticeInfoItem InfoItem { get { return _contexts.ui.uI.NoticeInfoItem; } set { } } 
        public NoticeWindowStyle Style { get { return _contexts.ui.uI.NoticeInfoItem.style; } set { _contexts.ui.uI.NoticeInfoItem.style = value; } }
       

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }
    }
}