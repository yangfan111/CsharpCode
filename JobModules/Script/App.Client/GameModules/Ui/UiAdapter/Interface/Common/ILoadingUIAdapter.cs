using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ILoadingUiAdapter : IAbstractUiAdapter
    {
        float CurValue { get; set; }
        string CurText { get; set; }
    }
}
