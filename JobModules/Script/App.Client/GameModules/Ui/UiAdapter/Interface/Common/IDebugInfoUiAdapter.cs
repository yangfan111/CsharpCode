using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter.Interface.Common
{
    public interface IDebugInfoUiAdapter : IAbstractUiAdapter
    {
        string VersionDebugInfo { get; }
        string PingDebugInfo { get; }
    }
}
