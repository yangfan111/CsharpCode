using System;
using Sharpen;

namespace com.wd.free.exception
{
	[System.Serializable]
	public class GameActionExpception : Exception
	{
		private const long serialVersionUID = 4509393008035571056L;

		public GameActionExpception()
			: base()
		{
		}

		public GameActionExpception(string msg)
			: base(msg)
		{
		}

		public GameActionExpception(Exception t)
			: base(t.Message)
		{
		}
	}
}
