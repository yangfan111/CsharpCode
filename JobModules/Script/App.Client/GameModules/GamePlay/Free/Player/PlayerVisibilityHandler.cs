using App.Shared.Player;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
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
            PlayerEntity pe = null;
            foreach (PlayerEntity playerEntity in contexts.player.GetEntities())
            {
                if (playerEntity.playerInfo.PlayerId == data.Ls[0])
                {
                    pe = playerEntity;
                    break;
                }
            }
            if (pe != null)
            {
                if (data.Bs[0])
                {
                    pe.RootGo().SetActive(true);
                }
                else
                {
                    pe.RootGo().SetActive(false);
                }
            }
        }
    }
}
