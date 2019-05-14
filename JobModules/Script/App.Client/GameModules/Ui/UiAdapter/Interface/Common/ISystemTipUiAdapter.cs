using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter.Interface.Common
{
    public interface ISystemTipUiAdapter : IAbstractUiAdapter
    {
        Queue<ITipData> SystemTipDataQueue { get; }
    }
}
