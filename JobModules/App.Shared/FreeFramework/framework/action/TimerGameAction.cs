using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;
using UnityEngine;
using System;

namespace com.wd.free.action
{
    [System.Serializable]
    public class TimerGameAction : AbstractGameAction
    {
        private const long serialVersionUID = 6879644291778679633L;

        private static int unique = 1;

        private string name;

        public string time;

        private IGameAction action;

        private IGameAction startAction;

        private IParaCondition condition;

        public string count;

        [System.NonSerialized]
        private int key;

        [System.NonSerialized]
        private MyDictionary<long, int> currentTime;

        [System.NonSerialized]
        private MyDictionary<long, MyDictionary<string, IParable>> map;

        [System.NonSerialized]
        private MyDictionary<long, MyDictionary<string, ParaList>> vMap;

        [System.NonSerialized]
        private MyDictionary<long, int> currentCount;

        [System.NonSerialized]
        private MyDictionary<string, long> keyMap;

        private static long executeCount = 1;

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual IGameAction GetAction()
        {
            return action;
        }

        public virtual void SetAction(IGameAction action)
        {
            this.action = action;
        }

        public virtual IGameAction GetStartAction()
        {
            return startAction;
        }

        public virtual void SetStartAction(IGameAction startAction)
        {
            this.startAction = startAction;
        }

        public virtual void TimeElapse(IEventArgs args, int time)
        {
            IList<long> remove = new List<long>();
            int countInt = FreeUtil.ReplaceInt(count, args);
            int timeInt = FreeUtil.ReplaceInt(this.time, args);
            List<long> sList = new List<long>(map.Keys);
            foreach (long s in sList)
            {
                if (currentCount[s] < countInt)
                {
                    currentTime[s] = currentTime[s] - time;
                    if (currentTime[s] <= 0)
                    {
                        SetArg(args, s);
                        if (condition != null && !condition.Meet(args))
                        {
                            remove.Add(s);
                            ResumeArg(args, s);
                            continue;
                        }
                        if (condition == null || condition.Meet(args))
                        {
                            action.Act(args);
                            //Debug.LogError("time-game-action " + this.time + " " + this.name);
                        }
                        ResumeArg(args, s);
                        currentCount[s] = currentCount[s] + 1;
                        currentTime[s] = timeInt;
                    }
                }
                else
                {
                    remove.Add(s);
                }
            }
            foreach (long s_1 in remove)
            {
                map.Remove(s_1);
                vMap.Remove(s_1);
                currentCount.Remove(s_1);
                currentTime.Remove(s_1);
            }
        }

        private void SetArg(IEventArgs fr, long s)
        {
            foreach (string key in map[s].Keys)
            {
                IParable pa = map[s][key];
                ParaList snapshotParaList = vMap[s][key];
                if (pa != null && snapshotParaList != null)
                {
                    foreach (string field in snapshotParaList.GetFields())
                    {
                        pa.GetParameters().TempUse(snapshotParaList.Get(field));
                    }
                }
                fr.TempUse(key, pa);
            }
        }

        private void ResumeArg(IEventArgs fr, long s)
        {
            foreach (string key in map[s].Keys)
            {
                fr.Resume(key);
                IParable pa = map[s][key];
                ParaList snapshotParaList = vMap[s][key];
                if (pa != null && snapshotParaList != null)
                {
                    foreach (string field in snapshotParaList.GetFields())
                    {
                        pa.GetParameters().Resume(field);
                    }
                }
            }
        }

        public virtual bool HasKey(string key)
        {
            return keyMap.ContainsKey(key);
        }

        public virtual void Stop(string key)
        {
            long s = keyMap[key];
            this.currentCount.Remove(s);
            this.currentTime.Remove(s);
            this.map.Remove(s);
            this.vMap.Remove(s);
            keyMap.Remove(key);
        }

        public override void DoAction(IEventArgs args)
        {
            if (key == 0)
            {
                key = unique;
                unique++;
                if (StringUtil.IsNullOrEmpty(count) || "0".Equals(count))
                {
                    count = "1";
                }
                args.FreeContext.TimerTask.Register(key, this);
                this.map = new MyDictionary<long, MyDictionary<string, IParable>>();
                this.vMap = new MyDictionary<long, MyDictionary<string, ParaList>>();
                this.currentTime = new MyDictionary<long, int>();
                this.currentCount = new MyDictionary<long, int>();
                this.keyMap = new MyDictionary<string, long>();
            }
            long s = executeCount++;
            currentTime[s] = FreeUtil.ReplaceInt(time, args);
            currentCount[s] = 0;
            if (FreeUtil.ReplaceVar(name, args) != null)
            {
                keyMap[FreeUtil.ReplaceVar(name, args)] = s;
            }
            map[s] = new MyDictionary<string, IParable>();
            vMap[s] = new MyDictionary<string, ParaList>();
            foreach (string unit in args.GetUnitKeys())
            {
                IParable pa = args.GetUnit(unit);
                if (pa != null)
                {
                    map[s][unit] = pa;
                    vMap[s][unit] = pa.GetParameters().Clone();
                }
            }
            if (startAction != null)
            {
                startAction.Act(args);
            }
        }

        public override String ToString()
        {
            return this.time + " " + this.name;
        }
    }
}
