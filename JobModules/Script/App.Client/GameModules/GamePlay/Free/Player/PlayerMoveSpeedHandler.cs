using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerMoveSpeedHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerMoveSpeedSet || key == FreeMessageConstant.PlayerJumpSpeedSet;
        }

        public void Handle(SimpleProto data)
        {
            if (data.Key == FreeMessageConstant.PlayerMoveSpeedSet)
            {
                SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity.stateInterface.State
                    .SetSpeedAffect(data.Fs[0]);
            }

            if (data.Key == FreeMessageConstant.PlayerJumpSpeedSet)
            {
                SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity.stateInterface.State
                    .SetJumpAffect(data.Fs[0]);
            }
        }
    }
}
