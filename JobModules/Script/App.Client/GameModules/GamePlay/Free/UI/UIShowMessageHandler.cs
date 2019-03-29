using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class UIShowMessageHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_UI_SHOW || key == FreeMessageConstant.MSG_UI_VALUE;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var ui = SingletonManager.Get<FreeUiManager>().GetUi(simpleUI.Ss[0]);

            if (ui != null)
            {
                if (simpleUI.Key == FreeMessageConstant.MSG_UI_SHOW)
                {
                    ui.Show(simpleUI);
                }

                if (simpleUI.Key == FreeMessageConstant.MSG_UI_VALUE)
                {
                    ui.SetValue(simpleUI);
                }
            }
        }

    }
}
