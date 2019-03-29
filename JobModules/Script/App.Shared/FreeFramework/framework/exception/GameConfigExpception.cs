using System;
using Sharpen;

namespace com.wd.free.exception
{
	[System.Serializable]
	public class GameConfigExpception : Exception
	{
		private const long serialVersionUID = 4509393008035571056L;

		public GameConfigExpception()
			: base()
		{
		}

		public GameConfigExpception(string msg)
			: base(msg)
		{
		}

		public GameConfigExpception(Exception t)
			: base(t.Message)
		{
		}
	}
}
