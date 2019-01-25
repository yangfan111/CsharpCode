using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;

namespace com.wd.free.trigger
{
    [System.Serializable]
    public class GameTrigger : Iterable<IGameAction>
    {
        private const long serialVersionUID = -1671098087428358092L;

        private int key;

        private string name;

        private string desc;

        private string group;

        private IList<IGameAction> actions;

        [System.NonSerialized]
        private bool disable;

        [System.NonSerialized]
        private string rule;

        public GameTrigger()
            : base()
        {
            this.actions = new List<IGameAction>();
        }

        public GameTrigger(int key)
            : base()
        {
            this.key = key;
            this.actions = new List<IGameAction>();
        }

        public GameTrigger(int key, string name)
            : base()
        {
            this.key = key;
            this.name = name;
            this.actions = new List<IGameAction>();
        }

        public virtual bool IsDisable()
        {
            return disable;
        }

        public virtual void Disable()
        {
            this.disable = true;
        }

        public virtual void Enable()
        {
            this.disable = false;
        }

        public virtual string GetRule()
        {
            return rule;
        }

        public virtual void SetRule(string rule)
        {
            this.rule = rule;
        }

        public virtual IList<IGameAction> GetActions()
        {
            return actions;
        }

        public virtual int GetKey()
        {
            return key;
        }

        public virtual void SetKey(int key)
        {
            this.key = key;
        }

        public virtual string GetGroup()
        {
            return group;
        }

        public virtual void SetGroup(string group)
        {
            this.group = group;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual string GetDesc()
        {
            return desc;
        }

        public virtual void SetDesc(string desc)
        {
            this.desc = desc;
        }

        public virtual void Merge(com.wd.free.trigger.GameTrigger trigger)
        {
            if (this.key == trigger.key)
            {
                foreach (IGameAction t in trigger.actions)
                {
                    this.actions.Add(t);
                }
            }
        }

        public virtual void AddAction(IGameAction t)
        {
            this.actions.Add(t);
        }

        public virtual void RemoveDelete(IGameAction t)
        {
            this.actions.Remove(t);
        }

        public virtual void Trigger(IEventArgs args)
        {
            if (!disable)
            {
                FreeLog.SetTrigger(this);
                FreeLog.ActionMark = this.@group + " " + this.name + " " + this.key;
                long s = FreeTimeDebug.RecordStart("trigger " + name);
                foreach (IGameAction de in actions)
                {
                    de.Act(args);
                }
                FreeTimeDebug.RecordEnd("trigger " + name, s);
            }
        }

        public override Sharpen.Iterator<IGameAction> Iterator()
        {
            return actions.Iterator();
        }

        public override string ToString()
        {
            return this.rule + " " + this.key + " " + this.name + "{" + actions.ToString() + "}";
        }
    }
}
