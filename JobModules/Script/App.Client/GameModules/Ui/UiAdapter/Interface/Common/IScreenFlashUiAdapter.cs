using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IScreenFlashUiAdapter : IAbstractUiAdapter
    {
        bool IsShow { get;}
        bool IsDirty{ get; }
        float Alpha { get; }
        float KeepTime { get; }
        float DecayTime { get; }
        void Clear();
    }
}
