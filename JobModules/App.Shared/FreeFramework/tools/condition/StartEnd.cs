using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	public class StartEnd
	{
		private int start;

		private int end;

		public StartEnd(int start, int end)
		{
			this.start = start;
			this.end = end;
		}

		public virtual int GetStart()
		{
			return start;
		}

		public virtual void SetStart(int start)
		{
			this.start = start;
		}

		public virtual int GetEnd()
		{
			return end;
		}

		public virtual void SetEnd(int end)
		{
			this.end = end;
		}

		public virtual string[] GetStrings(string[] ss, com.cpkf.yyjd.tools.condition.StartEnd[] subStartEnds)
		{
			IList<string> list = new List<string>();
			for (int i = start + 1; i < end; i++)
			{
				com.cpkf.yyjd.tools.condition.StartEnd se = GetStartEnd(subStartEnds, i);
				if (se == null)
				{
					list.Add(ss[i]);
				}
				else
				{
					if (!list.Contains(se.ToKey()))
					{
						list.Add(se.ToKey());
					}
				}
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		private com.cpkf.yyjd.tools.condition.StartEnd GetStartEnd(com.cpkf.yyjd.tools.condition.StartEnd[] ses, int i)
		{
			foreach (com.cpkf.yyjd.tools.condition.StartEnd se in ses)
			{
				if (se.start <= i && se.end >= i)
				{
					return se;
				}
			}
			return null;
		}

		public virtual com.cpkf.yyjd.tools.condition.StartEnd[] GetChildren(com.cpkf.yyjd.tools.condition.StartEnd[] startEnds)
		{
			IList<com.cpkf.yyjd.tools.condition.StartEnd> list = new List<com.cpkf.yyjd.tools.condition.StartEnd>();
			foreach (com.cpkf.yyjd.tools.condition.StartEnd se in startEnds)
			{
				if (Contains(se))
				{
					list.Add(se);
				}
			}
			return Sharpen.Collections.ToArray(list, new com.cpkf.yyjd.tools.condition.StartEnd[] {  });
		}

		public virtual string ToKey()
		{
			return start + "-" + end;
		}

		public static com.cpkf.yyjd.tools.condition.StartEnd FromKey(string key)
		{
			string[] ss = key.Split("-");
			return new com.cpkf.yyjd.tools.condition.StartEnd(int.Parse(ss[0]), int.Parse(ss[1]));
		}

		public virtual bool Contains(com.cpkf.yyjd.tools.condition.StartEnd startEnd)
		{
			if (this.Equals(startEnd))
			{
				return false;
			}
			if (start <= startEnd.start)
			{
				if (end >= startEnd.end)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + end;
			result = prime * result + start;
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
			com.cpkf.yyjd.tools.condition.StartEnd other = (com.cpkf.yyjd.tools.condition.StartEnd)obj;
			if (end != other.end)
			{
				return false;
			}
			if (start != other.start)
			{
				return false;
			}
			return true;
		}
	}
}
