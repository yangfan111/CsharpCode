using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ICountDownUiAdapter : IAbstractUiAdapter
    {
        bool StartCountDown { get; set; }
        float CountDownNum { get; set; }
        void SetCrossActive(bool isActive);
        bool CrossActiveStatue();
    }
}
