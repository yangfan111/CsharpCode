using Core.Free;
using Free.framework;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace Assets.Sources.Free.UI
{
    public class UIRemoveAllHandler : ISimpleMesssageHandler
    {


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_REMOVE_ALL_UI;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var canvas = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(x => x.name == "Canvas");
            if (canvas != null)
            {
                var root = canvas.transform.Find("FreeUI");
                if (root != null)
                {
                    GameObject.Destroy(root.gameObject);
                }
            }

            SingletonManager.Get<FreeUiManager>().RemoveAllUi();
        }
    }
}
