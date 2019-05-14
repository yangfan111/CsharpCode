using App.Shared;
using App.Shared.Player;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.EntityComponent;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerVisibilityHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerVisibility;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            PlayerEntity pe = contexts.player.GetEntityWithEntityKey(new EntityKey(data.Ins[0], (short) EEntityType.Player));
            if (null != pe)
            {
                pe.RootGo().SetActive(data.Bs[0]);
            }
        }
    }
}
