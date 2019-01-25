using System;
using Sharpen;

namespace com.wd.free.exception
{
	[System.Serializable]
	public class EventTypeException : Exception
	{
		private const long serialVersionUID = 4509393008035571056L;

		public EventTypeException()
			: base()
		{
		}

		public EventTypeException(string msg)
			: base(msg)
		{
		}

		public EventTypeException(Exception t)
			: base(t.Message)
		{
		}
	}
}
