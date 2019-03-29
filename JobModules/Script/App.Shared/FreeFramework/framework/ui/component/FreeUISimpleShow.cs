using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeUISimpleShow : IFreeUIShow
	{
		private const long serialVersionUID = 8047838442261693835L;

		private IList<FreeUISimpleShow.UIChange> changes;

		public virtual int GetKey()
		{
			return 1;
		}

		public virtual void Handle(IEventArgs args, SimpleProto b)
		{
			if (changes != null)
			{
				foreach (FreeUISimpleShow.UIChange uc in changes)
				{
					b.Ins.Add(FreeUtil.ReplaceInt(uc.fromTime, args));
					b.Ins.Add(FreeUtil.ReplaceInt(uc.toTime, args));
					b.Fs.Add(FreeUtil.ReplaceFloat(uc.fromValue, args));
					b.Fs.Add(FreeUtil.ReplaceFloat(uc.toValue, args));
					b.Ss.Add(FreeUtil.ReplaceVar(uc.field, args));
				}
			}
		}

		[System.Serializable]
		public class UIChange
		{
			private const long serialVersionUID = -3885296221415812105L;

			public string fromTime;

		    public string toTime;

		    public string fromValue;

		    public string toValue;

		    public string field;

			public virtual string GetFromTime()
			{
				return fromTime;
			}

			public virtual void SetFromTime(string fromTime)
			{
				this.fromTime = fromTime;
			}

			public virtual string GetToTime()
			{
				return toTime;
			}

			public virtual void SetToTime(string toTime)
			{
				this.toTime = toTime;
			}

			public virtual string GetFromValue()
			{
				return fromValue;
			}

			public virtual void SetFromValue(string fromValue)
			{
				this.fromValue = fromValue;
			}

			public virtual string GetToValue()
			{
				return toValue;
			}

			public virtual void SetToValue(string toValue)
			{
				this.toValue = toValue;
			}

			public static long GetSerialversionuid()
			{
				return serialVersionUID;
			}

			public virtual string GetField()
			{
				return field;
			}

			public virtual void SetField(string field)
			{
				this.field = field;
			}
		}
	}
}
