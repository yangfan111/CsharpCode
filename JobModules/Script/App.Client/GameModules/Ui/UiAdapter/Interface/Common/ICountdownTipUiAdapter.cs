using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter.Interface
{

    public interface ICountdownTipUiAdapter : IAbstractUiAdapter
    {
        List<ITipData> CountdownTipDataList { get;}
        long CurTime { get; }
    }
}
