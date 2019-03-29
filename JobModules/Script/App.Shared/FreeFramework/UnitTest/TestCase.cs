using com.wd.free.action;
using com.wd.free.ai;
using com.wd.free.@event;
using com.wd.free.trigger;
using Core.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.UnitTest
{
    public class TestCase
    {
        private OrderAiAction order;
        private bool started;

        public IUnitTestData data;

        public TestCase()
        {
            this.order = new OrderAiAction();
            this.order.repeat = 1;
            this.order.actions = new List<IGameAction>();
        }

        public void Merge(TriggerList triggers)
        {
            foreach (GameTrigger trigger in triggers)
            {
                if (trigger.GetKey() == FreeTriggerConstant.UNIT_TEST)
                {
                    foreach (IGameAction action in trigger)
                    {
                        if (action is OneCaseAction)
                        {
                            OneCaseAction one = (OneCaseAction)action;
                            one.trigger = trigger;
                            this.order.actions.Add(one);
                        }
                    }
                }
            }
        }

        public void AddCase(IGameAction ca)
        {
            this.order.actions.Add(ca);
        }

        public void Start(IEventArgs args)
        {
            started = true;
            this.order.Reset(args);
        }

        public void Pause(IEventArgs args)
        {
            started = false;
        }

        public void Resume(IEventArgs args)
        {
            started = true;
        }

        public void Frame(IEventArgs args)
        {
            if (started && this.order.actions.Count > 0)
            {
                this.order.Act(args);
            }
        }
    }
}
