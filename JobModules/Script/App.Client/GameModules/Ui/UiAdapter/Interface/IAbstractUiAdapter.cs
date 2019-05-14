using App.Shared.Components.Ui;
using UserInputManager.Lib;

namespace Assets.App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IAbstractUiAdapter : IUiAdapter
    {
        void RegisterKeyReceive(IKeyReceiver keyReceive);
        void UnRegisterKeyReceive(IKeyReceiver keyReceive);
        void RegisterPointerReceive(IPointerReceiver pointReceive);
        void UnRegisterPointerReceive(IPointerReceiver pointReceive);
        void RegisterOpenKey(IKeyReceiver keyReceiver);
    }
}