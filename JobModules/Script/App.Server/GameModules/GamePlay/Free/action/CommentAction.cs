using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class CommentAction : AbstractGameAction, IRule
    {
        private string text;

        public override void DoAction(IEventArgs args)
        {
            
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.CommentAction;
        }
    }
}
