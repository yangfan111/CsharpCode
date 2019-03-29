using Core.Enums;
using System;
using Utils.AssetManager;

namespace App.Shared.Components.Ui
{
    public enum NoticeWindowStyle
    {
        NONE,
        YESNO,
        YES,
        COUNTDOWN
    }

    public interface INoticeInfoItem
    {
        NoticeWindowStyle style { get; set; }
        string Title { get; set; }
        Action YesCallback { get; set; }
        Action NoCallback { get; set; }
        string YesText{get; set;}
        string NoText { get; set; }
        float CountDownTime { get; set; }
        Action CountDownCallback { get; set; }
    }
}