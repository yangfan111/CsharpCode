using System;
using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>Exception thrown if expression cannot be parsed correctly.</summary>
	/// <seealso cref="ExpressionTree"/>
	[System.Serializable]
	public class ExpressionParseException : Exception
	{
		private string descrip = null;

		private int index = 0;

		public ExpressionParseException(string descrip, int index)
		{
			this.descrip = descrip;
			this.index = index;
		}

		public virtual string GetDescription()
		{
			return descrip;
		}

		public virtual int GetIndex()
		{
			return index;
		}

		public override string ToString()
		{
			return "(" + index + ") " + descrip;
		}
	}
}
