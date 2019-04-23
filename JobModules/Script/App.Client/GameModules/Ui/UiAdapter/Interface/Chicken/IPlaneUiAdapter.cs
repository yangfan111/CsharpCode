using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IPlaneUiAdapter : IAbstractUiAdapter
    {
        int CurCount { get; }
        int TotalCount { get;}
    }
}
