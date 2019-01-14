using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using gameplay.gamerule.free.action;
using UnityEngine;
using com.cpkf.yyjd.tools.util;

namespace com.wd.free.action
{
    public class TimerTask
    {
        private List<TimerGameAction> map;
        private int last;

        private List<TimerGameAction> removes;
        private List<TimerGameAction> adds;

        public TimerTask()
        {
            this.map = new List<TimerGameAction>();
            this.removes = new List<TimerGameAction>();
            this.adds = new List<TimerGameAction>();
        }

        public virtual void Register(int key, TimerGameAction action)
        {
            adds.Add(action);
        }

        public void Remove(TimerGameAction action)
        {
            removes.Add(action);
        }

        public virtual void Stop(string name)
        {
            foreach (TimerGameAction tga in map)
            {
                if (tga.HasKey(name))
                {
                    tga.Stop(name);
                }
            }
        }

        public virtual void TimeElapse(IEventArgs args, int time)
        {
            if (map.Count != last)
            {
                last = map.Count;
                //List<string> list = new List<string>();
                //foreach (TimerGameAction ta in map)
                //{
                //    list.Add(ta.ToString());
                //}
                //Debug.LogError("all:" + StringUtil.GetStringFromStrings(list, ","));
            }
            Iterator<TimerGameAction> it = map.Iterator();
            while (it.HasNext())
            {
                TimerGameAction action = it.Next();
                action.TimeElapse(args, time);
            }

            if (removes.Count > 0)
            {
                foreach (var remove in removes)
                {
                    map.Remove(remove);
                }
                removes.Clear();
            }

            foreach (TimerGameAction action in adds)
            {
                map.Add(action);
            }
            adds.Clear();
        }
    }
}
