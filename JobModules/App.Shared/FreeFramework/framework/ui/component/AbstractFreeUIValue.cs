using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public abstract class AbstractFreeUIValue : IFreeUIValue
	{
		private const long serialVersionUID = 2169884841159232751L;

	    public const int INT = 1;

	    public const int STRING = 2;

	    public const int BOOL = 3;

	    public const int FLOAT = 4;

	    public const int DOUBLE = 5;

	    public const int LONG = 6;

	    public const int COMPLEX = 7;

	    public const int START_O = 1;

	    public const int START_N = 2;

	    public const int STOP_O = 3;

	    public const int STOP_N = 4;

        protected internal string seq;

		protected internal int autoStatus;

		protected internal int autoIndex;

		public virtual int GetSeq(IEventArgs args)
		{
			return FreeUtil.ReplaceInt(seq, args);
		}

		protected internal virtual string GetValue(IEventArgs args, string value)
		{
			return FreeUtil.ReplaceNumber(value, args);
		}

		public virtual int GetAutoStatus()
		{
			return autoStatus;
		}

		public virtual void SetAutoStatus(int autoStatus)
		{
			this.autoStatus = autoStatus;
		}

		public virtual int GetAutoIndex()
		{
			return autoIndex;
		}

		public virtual void SetAutoIndex(int autoIndex)
		{
			this.autoIndex = autoIndex;
		}

		public virtual string GetSeq()
		{
			return seq;
		}

		public virtual void SetSeq(string seq)
		{
			this.seq = seq;
		}

		public abstract int GetType();

		public abstract object GetValue(IEventArgs arg1);
	}
}
