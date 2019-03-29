using System;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class NoticeInfoItem : INoticeInfoItem
    {
        public NoticeWindowStyle style { get; set; }
        public string Title { get; set; }
        public Action YesCallback { get; set; }
        public Action NoCallback { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }
        public float  CountDownTime { get; set; }
        public Action CountDownCallback { get ; set; }

        public NoticeInfoItem()
        {
            style = NoticeWindowStyle.NONE;
        }
    }


    public interface INoticeUiAdapter : IAbstractUiAdapter
    {
        INoticeInfoItem InfoItem { get; set; }
        NoticeWindowStyle Style { get; set; }
        void SetCrossVisible(bool isVisible);
    }
}
