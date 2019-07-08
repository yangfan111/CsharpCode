using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using Core.Free;
using System;

namespace App.Shared.FreeFramework.Free.Map
{
    [Serializable]
    public class PlaySoundAction : AbstractPlayerAction, IRule
    {
        public string stop;
        public string loop;
        public string key;
        public string id;
        public string entity;
        public IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            
        }

        /*protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.PlaySound;
        }*/


        public int GetRuleID()
        {
            return (int)ERuleIds.PlaySoundAction;
        }
    }
}
