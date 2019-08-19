using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.para.exp;
using System;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class PlayerRaycastCondition : IParaCondition, IRule
    {
        public int key;
        public string player;

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerRaycastCondition;
        }

        public bool Meet(IEventArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit(player);
            if (fd != null)
            {
                if (fd.Player.stateInterface.State.GetNextActionKeepState() == ActionKeepInConfig.Sight)
                {
                    return false;
                }
                var states = fd.Player.StateInteractController().GetCurrStates();
                var manager = SingletonManager.Get<StateTransitionConfigManager>();
                foreach (var state in states)
                {
                    StateTransitionConfigItem condition = manager.GetConditionByState(state);
                    if (condition == null) continue;
                    if (!condition.GetTransition(Transition.IsUseAction)/*IsUseAction*/)
                    {
                        return false;
                    }
                }
                return fd.Player.hasRaycastTarget && fd.Player.raycastTarget.Key == key;
            }
            return false;
        }
    }
}
