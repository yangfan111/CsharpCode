using System;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using UnityEngine;
using Core.Utils;

namespace com.wd.free.action
{
	[System.Serializable]
	public abstract class AbstractGameAction : IGameAction
	{
		private const long serialVersionUID = 1542536984902004118L;

		protected string desc;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractGameAction));

        public virtual void Act(IEventArgs args)
		{
			long s = FreeLog.Start(this, args);
			long startTime = FreeTimeDebug.RecordStart(this.GetType().Name);
			try
			{
				DoAction(args);
			}
			catch (Exception e)
			{
				string err = "action failed\nat    " + FreeLog.ActionMark + "\n at" + this.ToMessage(args) + "\nat    " + ExceptionUtil.GetExceptionContent(e);
				FreeLog.Error(err, this);
                Debug.LogError(err);
                _logger.Error(err);
			}
			FreeTimeDebug.RecordEnd(this.GetType().Name, startTime);
			FreeLog.Stop(s, this, args);
		}

		public abstract void DoAction(IEventArgs args);

		public override string ToString()
		{
			return this.GetType().Name;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

        public virtual void Reset(IEventArgs args)
        {

        }

		public virtual string ToMessage(IEventArgs args)
		{
			return this.ToString();
		}
	}
}
