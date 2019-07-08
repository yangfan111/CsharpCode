using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using com.wd.free.skill;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.FreeFramework.Free.Chicken
{
    public class PlayerStateInterrupter : ISkillInterrupter
    {
        public bool IsInterrupted(ISkillArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit("current");
            if(fd != null)
            {
                if (PlayerStateUtil.HasPlayerState(EPlayerGameState.InterruptItem, fd.Player.gamePlay))
                {
                    return true;
                }

                var manager = SingletonManager.Get<StateTransitionConfigManager>();

                if (fd.Player.stateInterface.State.GetCurrentPostureState() != fd.Player.stateInterface.State.GetNextPostureState())
                    return true;

                foreach (EPlayerState state in fd.Player.StateInteractController().GetCurrStates())
                {
                    StateTransitionConfigItem condition = manager.GetConditionByState(state);
                    if (condition == null) continue;
                    if (!condition.IsUseItem)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }
    }
}
