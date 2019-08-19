using App.Shared.Components.Ui;
using UserInputManager.Lib;

namespace Assets.App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IAbstractUiAdapter : IUiAdapter
    {
        void RegisterKeyReceive(KeyReceiver keyReceive);
        void UnRegisterKeyReceive(KeyReceiver keyReceive);
        void RegisterPointerReceive(PointerReceiver pointReceive);
        void UnRegisterPointerReceive(PointerReceiver pointReceive);
        void RegisterOpenKey(KeyReceiver keyReceiver);
    }
}