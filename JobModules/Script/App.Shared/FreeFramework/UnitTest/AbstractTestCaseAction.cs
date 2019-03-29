using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public abstract class AbstractTestCaseAction : AbstractPlayerAction
    {
        protected const string Unknown = "Unknow";

        protected string name;

        protected string Name { get { return name; } }

        protected void RecordResult(IEventArgs args, GameTrigger trigger, TestValue[] tvs)
        {
            CaseKey key = new CaseKey();

            if (trigger != null)
            {
                key.rule = trigger.GetRule();
                key.suit = trigger.GetGroup() + "|" + trigger.GetName();
            }
            else
            {
                key.rule = Unknown;
                key.suit = Unknown;
            }

            key.caseName = name;
            key.field = tvs[0].Name;

            if(args.FreeContext.TestCase.data != null)
            {
                args.FreeContext.TestCase.data.RecordResult(key, tvs);
            }
        }
    }
}
