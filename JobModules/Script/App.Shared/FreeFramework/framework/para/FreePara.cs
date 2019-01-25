using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace com.wd.free.para
{
	public class FreePara
	{
		private string para;

		private bool dynamic;

		[System.NonSerialized]
		private object realValue;

		[System.NonSerialized]
		private bool initialed;

		public FreePara()
			: base()
		{
		}

		public FreePara(string para)
			: base()
		{
			this.para = para;
		}

		public FreePara(string para, bool dynamic)
			: base()
		{
			this.para = para;
			this.dynamic = dynamic;
		}

		public virtual void SetPara(string para)
		{
			this.para = para;
		}

		public virtual string GetPara()
		{
			return para;
		}

		public virtual void SetDynamic(bool dynamic)
		{
			this.dynamic = dynamic;
		}

		public virtual bool IsDynamic()
		{
			return dynamic;
		}

		public virtual int GetInt(IEventArgs args)
		{
			if (!initialed || dynamic)
			{
				realValue = FreeUtil.ReplaceInt(para, args);
				initialed = true;
			}
			return (int)realValue;
		}

		public virtual float GetFloat(IEventArgs args)
		{
			if (!initialed || dynamic)
			{
				realValue = FreeUtil.ReplaceFloat(para, args);
				initialed = true;
			}
			return (float)realValue;
		}

		public virtual double GetDouble(IEventArgs args)
		{
			if (!initialed || dynamic)
			{
				realValue = FreeUtil.ReplaceDouble(para, args);
				initialed = true;
			}
			return (double)realValue;
		}

		public virtual bool GetBool(IEventArgs args)
		{
			if (!initialed || dynamic)
			{
				realValue = FreeUtil.ReplaceBool(para, args);
				initialed = true;
			}
			return (bool)realValue;
		}

		public virtual string GetString(IEventArgs args)
		{
			if (!initialed || dynamic)
			{
				realValue = FreeUtil.ReplaceVar(para, args);
				initialed = true;
			}
			return (string)realValue;
		}
	}
}
