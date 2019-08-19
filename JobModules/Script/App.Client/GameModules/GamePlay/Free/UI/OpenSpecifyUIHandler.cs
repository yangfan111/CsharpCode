using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class OpenSpecifyUIHandler:ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.OpenSpecifyUI;
        }

        public void Handle(SimpleProto data)
        {
            var selfEntity = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            selfEntity.playerClientUpdate.OpenUIFrameSync = true;
        }
    }
}