using System.Collections.Generic;
using System.Linq;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.collection;

namespace com.wd.free.action
{
	public class FreeTimeDebug
	{
		private static Accumulator<string> totalTime;

		private static Accumulator<string> count;

		private static Accumulator<string> realTotalTime;

		private static LinkedHashMap<FreeTimeDebug.ActionTime, int> triggerTime;

		private static bool enable;

		private static int deep;

		static FreeTimeDebug()
		{
			totalTime = new Accumulator<string>();
			count = new Accumulator<string>();
			triggerTime = new LinkedHashMap<FreeTimeDebug.ActionTime, int>();
			realTotalTime = new Accumulator<string>();
		}

		public static void Reset()
		{
			if (enable)
			{
				count.Clear();
				totalTime.Clear();
				triggerTime.Clear();
				realTotalTime.Clear();
				deep = 0;
			}
		}

		public static void Enable()
		{
			enable = true;
		}

		public static void Disable()
		{
			enable = false;
		}

		public static long RecordStart(string action)
		{
			if (enable)
			{
				long s = Runtime.NanoTime();
				triggerTime[new FreeTimeDebug.ActionTime(action, deep, s, 0)] = 0;
				// System.out.println(deep + " start " + action + " "
				// + FreeLog.getCurrentTrigger().getName());
				deep++;
				return s;
			}
			return 0L;
		}

		public static void RecordEnd(string action, long startNanoTime)
		{
			if (enable)
			{
				deep--;
				// System.out.println(deep + " stop " + action + " "
				// + FreeLog.getCurrentTrigger().getName());
				int time = (int)((Runtime.NanoTime() - startNanoTime)) / 100;
				triggerTime[new FreeTimeDebug.ActionTime(action, deep, startNanoTime, 0)] = time;
				count.AddKey(action, 1);
				totalTime.AddKey(action, time);
				if (deep == 0)
				{
					ComputeRealTime();
					// Accumulator<String> acc = new Accumulator<>();
					// if (time > 10000) {
					// for (ActionTime at : triggerTime.keySet()) {
					// acc.addKey(at.action, at.realTime);
					// if (at.realTime > 10000) {
					// System.out.println(
					// StringUtil.getBlankString(at.deep * 2)
					// + at.deep + " " + at.action + "="
					// + at.time + "," + at.realTime);
					// }
					// }
					//
					// for (String ac : acc.keysSortedByValue()) {
					// if (acc.getCount(ac) > 1000) {
					// System.out.println(ac + "=" + acc.getCount(ac));
					// }
					// }
					// }
					triggerTime.Clear();
				}
			}
		}

		public static string ToMessage()
		{
			IList<string> list = new List<string>();
			if (enable)
			{
				list.Add(GetLenString("    动作", 58) + GetLenString("总时间", 8) + GetLenString("实际时间", 8) + GetLenString("调用次数", 8));
				foreach (string action in realTotalTime.KeysSortedByValue())
				{
					if (realTotalTime.GetCount(action) > 10000)
					{
						list.Add(GetLenString(action, 60) + GetLenString((totalTime.GetCount(action) / 10000).ToString(), 12) + GetLenString((realTotalTime.GetCount(action) / 10000).ToString(), 12) + GetLenString(count.GetCount(action).ToString(), 12));
					}
				}
			}
			return StringUtil.GetStringFromStrings(list, "\n");
		}

		private static void ComputeRealTime()
		{
			FreeTimeDebug.ActionTime last = null;
		    FreeTimeDebug.ActionTime[] ats = triggerTime.Keys.ToArray();
			foreach (FreeTimeDebug.ActionTime at in ats)
			{
				at.time = triggerTime[at];
				at.realTime = at.time;
				if (last == null)
				{
					last = at;
				}
				if (last != at)
				{
					if (last.deep < at.deep)
					{
						at.parent = last;
						last.AddChild(at);
					}
					else
					{
						if (last.deep == at.deep)
						{
							at.parent = last.parent;
							if (at.parent != null)
							{
								at.parent.AddChild(at);
							}
						}
						else
						{
							if (last.deep > at.deep)
							{
								FreeTimeDebug.ActionTime root = last;
								while (root != null)
								{
									if (root.deep == at.deep - 1)
									{
										at.parent = root;
										root.AddChild(at);
									}
									root = root.parent;
								}
							}
						}
					}
					last = at;
				}
			}
			foreach (FreeTimeDebug.ActionTime at_1 in triggerTime.Keys)
			{
				at_1.Compute();
				realTotalTime.AddKey(at_1.action, at_1.realTime);
			}
		}

		private static string GetLenString(string s, int len)
		{
			for (int i = s.Length; i < len - StringUtil.GetChineseLength(s); i++)
			{
				s = s + " ";
			}
			return s;
		}

		internal class ActionTime
		{
			internal string action;

			internal int deep;

			internal int time;

			internal int realTime;

			internal long startTime;

			internal FreeTimeDebug.ActionTime parent;

			internal IList<FreeTimeDebug.ActionTime> children;

			public ActionTime(string action, int deep, long startTime, int time)
				: base()
			{
				this.action = action;
				this.deep = deep;
				this.time = time;
				this.realTime = time;
				this.startTime = startTime;
				this.children = new List<FreeTimeDebug.ActionTime>();
			}

			public virtual void Compute()
			{
				foreach (FreeTimeDebug.ActionTime child in children)
				{
					this.realTime -= child.time;
				}
			}

			public virtual void AddChild(FreeTimeDebug.ActionTime time)
			{
				this.children.Add(time);
			}

			public override string ToString()
			{
				return action + ", " + deep + ", " + time + ", " + realTime;
			}

			public override int GetHashCode()
			{
				int prime = 31;
				int result = 1;
				result = prime * result + ((action == null) ? 0 : action.GetHashCode());
				result = prime * result + (int)(startTime ^ ((long)(((ulong)startTime) >> 32)));
				return result;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null)
				{
					return false;
				}
				if (GetType() != obj.GetType())
				{
					return false;
				}
				FreeTimeDebug.ActionTime other = (FreeTimeDebug.ActionTime)obj;
				if (action == null)
				{
					if (other.action != null)
					{
						return false;
					}
				}
				else
				{
					if (!action.Equals(other.action))
					{
						return false;
					}
				}
				if (startTime != other.startTime)
				{
					return false;
				}
				return true;
			}
		}
	}
}
