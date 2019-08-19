using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using Core.Free;
using gameplay.gamerule.free.ui;
using System;

namespace App.Shared.FreeFramework.Free.Map
{
    [Serializable]
    public class PlaySoundAction : SendMessageAction, IRule
    {
        public int sound;
        public bool play;
        public bool self;
        public IPosSelector pos;

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.PlaySound;
            if (null == pos)
            {
                if (play)
                {
                    builder.Ks.Add(2);
                    builder.Ins.Add(sound);
                    builder.Bs.Add(self);
                }
                else
                {

                }
            }
            else
            {
                UnitPosition up = pos.Select(args);
                if (up != null)
                {
                    builder.Ks.Add(3);
                    builder.Fs.Add(up.GetX());
                    builder.Fs.Add(up.GetY());
                    builder.Fs.Add(up.GetZ());
                    builder.Ins.Add(sound);
                }
            }
        }


        public int GetRuleID()
        {
            return (int)ERuleIds.PlaySoundAction;
        }
    }
}
