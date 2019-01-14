using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.wd.free.action
{
	public class FieldPair
	{
		private const string TO = "->";

		private string from;

		private string to;

		public FieldPair()
			: base()
		{
		}

		public FieldPair(string from, string to)
			: base()
		{
			this.from = from;
			this.to = to;
		}

		public virtual string GetFrom()
		{
			return from;
		}

		public virtual void SetFrom(string from)
		{
			this.from = from;
		}

		public virtual string GetTo()
		{
			return to;
		}

		public virtual void SetTo(string to)
		{
			this.to = to;
		}

		public static com.wd.free.action.FieldPair[] Parse(string fields)
		{
			IList<com.wd.free.action.FieldPair> list = new List<com.wd.free.action.FieldPair>();
			if (!StringUtil.IsNullOrEmpty(fields))
			{
				foreach (string f in StringUtil.Split(fields, ","))
				{
					string[] vs = StringUtil.Split(f.Trim(), TO);
					com.wd.free.action.FieldPair fp = new com.wd.free.action.FieldPair();
					if (vs.Length == 2)
					{
						fp.from = vs[0].Trim();
						fp.to = vs[1].Trim();
					}
					else
					{
						if (vs.Length == 1)
						{
							fp.from = vs[0].Trim();
							fp.to = vs[0].Trim();
						}
					}
					list.Add(fp);
				}
			}
			return Sharpen.Collections.ToArray(list, new com.wd.free.action.FieldPair[0]);
		}
	}
}
