using System.Collections.Generic;
using Sharpen;
using com.wd.free.para;
using com.wd.free.unit;
using gameplay.gamerule.free.action;
using Core.Free;
using com.wd.free.trigger;
using gameplay.gamerule.free.component;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.map;
using App.Shared.FreeFramework.framework.@event;
using com.wd.free.util;
using com.wd.free.action;

namespace com.wd.free.@event
{
    public class BaseEventArgs : IEventArgs
    {
        public const string DEFAULT = "default";

        public MyDictionary<string, IParable> map;

        protected MyDictionary<string, IParable> directMap;

        protected GameUnitSet units;

        protected CommonActions _functions;

        protected GameTriggers _triggers;

        protected GameComponentMap _componentMap;

        protected GameComponents _components;

        protected Contexts _gameContext;

        protected FreeBufManager _bufs;

        protected FreeContext _freeContext;

        private MyDictionary<string, Stack<IParable>> temp;

        public BaseEventArgs()
        {
            this.map = new MyDictionary<string, IParable>();
            this.directMap = new MyDictionary<string, IParable>();
            this.units = new GameUnitSet();
            this.temp = new MyDictionary<string, Stack<IParable>>();
            this._functions = new CommonActions();
            this._triggers = new GameTriggers();
            this._componentMap = new GameComponentMap();
            this._components = new GameComponents();
            this._bufs = new FreeBufManager();
            this._freeContext = new FreeContext(_gameContext);
        }

        public virtual GameTriggers Triggers
        {
            get { return _triggers; }
        }

        public virtual IFreeRule Rule { get { return null; } set { } }


        public virtual Contexts GameContext { get { return _gameContext; } }

        public virtual FreeContext FreeContext
        {
            get { return _freeContext; }
        }

        public virtual CommonActions Functions
        {
            get { return _functions; }
        }

        public virtual GameComponentMap ComponentMap
        {
            get { return _componentMap; }
            set { _componentMap = value; }
        }

        public void MergeFunctions(CommonActions functions)
        {
            this._functions.Merge(functions);
        }

        public virtual void SetPara(string key, IParable paras)
        {
            this.directMap[key] = paras;
        }

        public virtual IParable RemovePara(string key)
        {
            IParable para = this.map[key];
            this.map.Remove(key);

            this.directMap.Remove(key);
            if (temp.ContainsKey(key))
            {
                this.temp.Remove(key);
            }

            return para;
        }

        public virtual void AddDefault(IParable paras)
        {
            this.map[DEFAULT] = paras;
        }

        public virtual void ClearDefault()
        {
            RemovePara(DEFAULT);
        }

        public virtual IParable GetDefault()
        {
            return GetUnit(DEFAULT);
        }

        public virtual IParable GetUnit(string key)
        {
            if (directMap.Count > 0 && directMap.ContainsKey(key))
            {
                return directMap[key];
            }
            else
            {
                return map[key];
            }
        }

        public virtual string[] GetUnitKeys()
        {
            return Sharpen.Collections.ToArray(map.Keys, new string[0]);
        }

        public virtual GameUnitSet GetGameUnits()
        {
            return units;
        }

        public virtual void SetGameUnits(GameUnitSet units)
        {
            this.units = units;
        }

        public virtual void TempUse(string key, IParable paras)
        {
            if (!temp.ContainsKey(key))
            {
                temp[key] = new Stack<IParable>();
            }
            temp[key].Push(GetUnit(key));

            this.map[key] = paras;
        }

        public virtual void Resume(string key)
        {
            IParable old = temp[key].Pop();

            this.map[key] = old;
        }

        public void TempUsePara(IPara para)
        {
            if (map.ContainsKey(DEFAULT))
            {
                map[DEFAULT].GetParameters().TempUse(para);
            }
        }

        public void ResumePara(string paraName)
        {
            if (map.ContainsKey(DEFAULT))
            {
                map[DEFAULT].GetParameters().Resume(paraName);
            }
        }

        private TriggerArgs triggerArgs = new TriggerArgs();

        public void Trigger(int eventId)
        {
            _triggers.Trigger(eventId, this);
        }

        public void Trigger(int eventId, TriggerArgs args)
        {
            args.Trigger(this, _triggers, eventId);
        }

        public void Trigger(int eventId, IPara para)
        {
            triggerArgs.Reset();
            triggerArgs.AddPara(para);

            triggerArgs.Trigger(this, _triggers, eventId);
        }

        public void Trigger(int eventId, params TempUnit[] units)
        {
            triggerArgs.Reset();
            for (int i = 0; i < units.Length; i++)
            {
                triggerArgs.AddUnit(units[i]);
            }

            triggerArgs.Trigger(this, _triggers, eventId);
        }

        public void Trigger(int eventId, TempUnit[] units, IPara[] paras)
        {
            triggerArgs.Reset();
            for (int i = 0; i < units.Length; i++)
            {
                triggerArgs.AddUnit(units[i]);
            }
            for (int i = 0; i < paras.Length; i++)
            {
                triggerArgs.AddPara(paras[i]);
            }

            triggerArgs.Trigger(this, _triggers, eventId);
        }

        public void Act(IGameAction action)
        {
            if (action != null)
            {
                action.Act(this);
            }
        }

        public void Act(IGameAction action, TriggerArgs args)
        {
            if (action != null)
            {
                args.Act(this, action);
            }
        }

        public void Act(IGameAction action, TempUnit unit)
        {

            if (action != null)
            {
                triggerArgs.Reset();
                triggerArgs.AddUnit(unit);

                triggerArgs.Act(this, action);
            }
        }

        public void Act(IGameAction action, params TempUnit[] units)
        {
            if (action != null)
            {
                triggerArgs.Reset();
                for (int i = 0; i < units.Length; i++)
                {
                    triggerArgs.AddUnit(units[i]);
                }

                triggerArgs.Act(this, action);
            }
        }

        public int GetInt(string v)
        {
            return FreeUtil.ReplaceInt(v, this);
        }

        public float GetFloat(string v)
        {
            return FreeUtil.ReplaceFloat(v, this);
        }

        public bool GetBool(string v)
        {
            return FreeUtil.ReplaceBool(v, this);
        }

        public double getDouble(string v)
        {
            return FreeUtil.ReplaceDouble(v, this);
        }

        public long GetLong(string v)
        {
            return (long)FreeUtil.ReplaceDouble(v, this);
        }

        public string GetString(string v)
        {
            return FreeUtil.ReplaceVar(v, this);
        }
    }
}
