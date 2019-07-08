using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Utils;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;
using Sharpen;
using System;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace com.wd.free.action
{
    [System.Serializable]
	public abstract class AbstractGameAction : IGameAction
	{
		private const long serialVersionUID = 1542536984902004118L;

		protected string desc;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractGameAction));
        private static StringBuilder _tempStringBuilder = new StringBuilder();

        public virtual void Act(IEventArgs args)
		{
			long s = FreeLog.Start(this, args);
            string actName = this.GetName();
            long startTime = FreeTimeDebug.RecordStart(actName);
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(actName);
                DoAction(args);
            }
            catch (Exception e)
            {
                int length = _tempStringBuilder.Length;
                _tempStringBuilder.Remove(0, length);
                _tempStringBuilder.Append("action failed\nat ");
                _tempStringBuilder.Append(FreeLog.ActionMark);
                _tempStringBuilder.Append("\n at ");
                _tempStringBuilder.Append(this.ToMessage(args));
                _tempStringBuilder.Append("\nat ");
                _tempStringBuilder.Append(ExceptionUtil.GetExceptionContent(e));
#if UNITY_EDITOR
                string err = _tempStringBuilder.ToString();
                FreeLog.Error(err, this);
                Debug.LogError(err);
                _logger.Error(err);
#else
                string err = _tempStringBuilder.ToString();
                _logger.Error(err);
#endif

                if (args.FreeContext.DebugMode)
                {
                    FreeUIUpdateAction update = new FreeUIUpdateAction();
                    update.SetKey("testUI");
                    //update.SetPlayer(FreeUtil.ReplaceVar(player, args));
                    update.SetScope(FreeUtil.ReplaceInt(SendMessageAction.SCOPE_ALL.ToString(), args));

                    FreeUITextValue textValue = new FreeUITextValue();
                    textValue.SetSeq("1");
                    update.AddValue(textValue);

                    FreeUIShowAction show = new FreeUIShowAction();
                    show.SetKey("testUI");
                    //show.SetPlayer(FreeUtil.ReplaceVar(player, args));
                    show.SetScope(FreeUtil.ReplaceInt(SendMessageAction.SCOPE_ALL.ToString(), args));
                    show.SetTime("3000");

                    long lastTime = 0L;
                    if (Runtime.CurrentTimeMillis(false) - lastTime >= 3000L)
                    {
                        textValue.SetText(FreeUtil.ReplaceVar(err, args));
                        update.DoAction(args);
                        show.DoAction(args);
                        lastTime = Runtime.CurrentTimeMillis(false);
                    }
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(actName);
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

        public virtual string GetName()
        {
            return "UnKnow";
        }
	}
}
