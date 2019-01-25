using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.framework.trigger
{
    public class TriggerArgs
    {
        private List<TempUnit> units;
        private List<IPara> paras;

        public TriggerArgs()
        {
            this.units = new List<TempUnit>();
            this.paras = new List<IPara>();
        }

        public void AddPara(IPara para)
        {
            this.paras.Add(para);
        }

        public void AddUnit(string name, IParable parable)
        {
            units.Add(new TempUnit(name, parable));
        }

        public void AddUnit(TempUnit unit)
        {
            units.Add(unit);
        }

        public void Reset()
        {
            units.Clear();
            paras.Clear();
        }

        public void Act(IEventArgs args, IGameAction action)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].unit != null)
                {
                    args.TempUse(units[i].key, units[i].unit);
                }

            }
            for (int i = 0; i < paras.Count; i++)
            {
                args.TempUsePara(paras[i]);
            }

            action.Act(args);

            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].unit != null)
                {
                    args.Resume(units[i].key);
                }
            }
            for (int i = 0; i < paras.Count; i++)
            {
                args.ResumePara(paras[i].GetName());
            }

            Reset();
        }

        public void Trigger(IEventArgs args, GameTriggers triggers, int trigger)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].unit != null)
                {
                    args.TempUse(units[i].key, units[i].unit);
                }

            }
            for (int i = 0; i < paras.Count; i++)
            {
                args.TempUsePara(paras[i]);
            }

            triggers.Trigger(trigger, args);

            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].unit != null)
                {
                    args.Resume(units[i].key);
                }
            }
            for (int i = 0; i < paras.Count; i++)
            {
                args.ResumePara(paras[i].GetName());
            }

            Reset();
        }
    }

    public struct TempUnit
    {
        public string key;
        public IParable unit;

        public TempUnit(string key, IParable unit)
        {
            this.key = key;
            this.unit = unit;
        }
    }
}
