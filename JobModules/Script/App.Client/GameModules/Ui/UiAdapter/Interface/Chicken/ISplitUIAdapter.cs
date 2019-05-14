using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ISplitUiAdapter : IAbstractUiAdapter
    {
        SplitPropInfo Info { get; set; }
        void SendSplitMessage(int number, string itemKey);
        void SetCrossActive(bool isActive);
        void ShowIllegalTip();
    }
}
