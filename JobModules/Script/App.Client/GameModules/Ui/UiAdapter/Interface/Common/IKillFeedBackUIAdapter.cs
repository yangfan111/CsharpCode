using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IKillFeedBackUiAdapter : IAbstractUiAdapter
    {
        List<int> Types { get; }  //击杀类型ID数组
    }
}
