using Assets.Sources.Free.Utility;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class UIDuplicateMessageHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.DuplicateUI;
        }

        public void Handle(SimpleProto simpleUI)
        {
            string[] ss = simpleUI.Ss[0].Split(FreeMessageConstant.SpilterField);

            var key = ss[0];
            string type = ss[1];

            SimpleProto data = SingletonManager.Get<FreeUiManager>().GetUIData(type);
            if (data != null)
            {
                // key
                data.Ss[0] = key;

                // show
                data.Bs[0] = simpleUI.Bs[0];

                // relative
                data.Ins[2] = simpleUI.Ks[0];

                // x,y
                data.Fs[0] = simpleUI.Fs[0];
                data.Fs[1] = simpleUI.Fs[1];

                simpleUI.Key = FreeMessageConstant.MSG_UI_VALUE;
                SimpleMessageManager.Instance.DoHandle(FreeMessageConstant.MSG_UI_CREATE, data);
                SimpleMessageManager.Instance.DoHandle(FreeMessageConstant.MSG_UI_VALUE, simpleUI);
            }
        }

    }
}
