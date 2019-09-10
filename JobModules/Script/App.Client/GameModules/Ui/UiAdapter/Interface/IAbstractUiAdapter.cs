using App.Shared.Components.Ui;
using UserInputManager.Lib;

namespace Assets.App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IAbstractUiAdapter : IUiAdapter
    {
        void RegisterKeyReceive(KeyHandler keyReceive);
        void UnRegisterKeyReceive(KeyHandler keyReceive);
        void RegisterPointerReceive(PointerKeyHandler pointReceive);
        void UnRegisterPointerReceive(PointerKeyHandler pointReceive);
        void RegisterOpenKey(KeyHandler keyHandler);
    }
}