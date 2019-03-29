using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Assets.App.Server.GameModules.GamePlay.Free.chicken;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    /// <summary>
    /// 大逃杀模式中不通用的一些行为，通用的行为单独作为Action
    /// </summary>
    [Serializable]
    public class ChickenRuleAction : AbstractGameAction
    {
        private int code;
        private string value;

        private static IGameAction[] actions;

        static ChickenRuleAction()
        {
            actions = new IGameAction[100];
            actions[1] = new BagItemAction();
            actions[2] = new ChickenWeaponAction();
            actions[3] = new ComputeLineAction();
            actions[4] = new FlyLineShowAction();
            actions[5] = new FlyLineHideAction();
            actions[6] = new InitialItemAction();
        }

        public override void DoAction(IEventArgs args)
        {
            if(code < actions.Length)
            {
                IGameAction action = actions[code];
                if(action != null)
                {
                    action.Act(args);
                }
            }
        }
    }
}
