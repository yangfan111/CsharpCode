using App.Server.GameModules.GamePlay.Free.player;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerAnimationHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerAnimation;
        }

        public void Handle(SimpleProto data)
        {
            int ani = data.Ins[0];
            var contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            switch (ani)
            {
                case PlayerAnimationAction.SuccessPose:
                    contexts.player.flagSelfEntity.stateInterface.State.StartSuccessPose(data.Ins[1]);
                    break;
                default:
                    PlayerAnimationAction.DoAnimation(contexts, ani, contexts.player.flagSelfEntity, false);
                    break;
            }
        }
    }
}
