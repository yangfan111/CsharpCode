using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class UIDeleteMessageHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_UI_DELETE;
        }

        public void Handle(SimpleProto simpleUI)
        {

            var key = simpleUI.Ss[0];

            var old = SingletonManager.Get<FreeUiManager>().GetUi(key);

            if (old != null)
            {
                SingletonManager.Get<FreeUiManager>().RemoveUi(old);
                UnityEngine.Object.Destroy(old.gameObject);
            }
        }

    }
}
