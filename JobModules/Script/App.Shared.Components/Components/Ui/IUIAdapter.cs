using App.Shared.Components.UserInput;
using Core.Ui;

namespace App.Shared.Components.Ui
{
    public interface IUiAdapter
    {
        //UI是否可用
        bool Enable { set; get; }
        bool IsReady();//数据entity是否准备就绪

        UISessionComponent UiSessionComponent { get; set; }
        UserInputManagerComponent UserInputManager { get; set; }
        void HideUiGroup(UiGroup group);
        void ShowUiGroup(UiGroup group);
        bool CanOpenUiByKey { get; set; }

    }
}