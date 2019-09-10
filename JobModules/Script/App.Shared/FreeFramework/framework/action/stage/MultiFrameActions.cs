using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.action;
using com.wd.free.@event;
using Sharpen;
using UnityEngine;

namespace com.wd.free.action.stage
{
    public class MultiFrameActions : Iterable<IGameAction>
    {
        private MyDictionary<string, IGameAction> _actions;

        public MultiFrameActions()
        {
            _actions = new MyDictionary<string, IGameAction>();
        }

        public void AddAction(string key, IGameAction action)
        {
            _actions[key] = action;
        }

        public void RemoveAction(string key)
        {
            _actions.Remove(key);
        }

        public void Act(IEventArgs args)
        {
            //Debug.LogFormat("actions {0}", _actions.Count);
            foreach(IGameAction action in _actions.Values)
            {
                action.Act(args);
            }
        }

        public override Iterator<IGameAction> Iterator()
        {
            return _actions.Values.Iterator();
        }
    }
}
