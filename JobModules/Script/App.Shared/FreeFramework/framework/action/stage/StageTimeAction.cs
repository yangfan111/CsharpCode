using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using System;

namespace com.wd.free.action.stage
{
	[System.Serializable]
	public class StageTimeAction : AbstractGameAction
	{
		private const long serialVersionUID = 1L;

		private IList<OneTimeStageAction> stages;

		[System.NonSerialized]
		private int index;

		[System.NonSerialized]
		private bool initialed;

        [NonSerialized]
        private long startTime;

		public override void DoAction(IEventArgs args)
		{
			if (!initialed)
			{
				if (stages != null)
				{
				    stages.Sort(new StageComparer(args));
				}
				index = 0;
				initialed = true;
                startTime = args.Rule.ServerTime;
            }

			if (index < stages.Count)
			{
				OneTimeStageAction current = stages[index];

				if (args.Rule.ServerTime - startTime >= current.GetRealTime(args))
				{
				    if (current.action != null)
				    {
				        current.action.Act(args);
                    }
					
					index++;
                }

			    if (current.frameAction != null)
			    {
                    current.frameAction.Act(args);
			    }
                
			}
		}

		private sealed class StageComparer : IComparer<OneTimeStageAction>
		{
		    private IEventArgs args;

            public StageComparer(IEventArgs args)
			{
				this.args = args;
			}

			public int Compare(OneTimeStageAction o1, OneTimeStageAction o2)
			{
				return o1.GetRealTime(args) - o2.GetRealTime(args);
			}

			
		}
	}
}
