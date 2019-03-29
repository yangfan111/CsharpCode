using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The log function.</summary>
	public class LogFunction : Function
	{
		public LogFunction()
		{
		}

		/// <summary>
		/// If the number of parameters specified is 1, then the log base 10 is taken of the
		/// value at index location 0.
		/// </summary>
		/// <remarks>
		/// If the number of parameters specified is 1, then the log base 10 is taken of the
		/// value at index location 0.  If the number of parameters specified is 2, then the
		/// base value is at index location 1.
		/// </remarks>
		public virtual double Of(double[] d, int numParam)
		{
			if (numParam == 1)
			{
				return MyMath.Log(d[0]) / MyMath.Log(10);
			}
			return MyMath.Log(d[0]) / MyMath.Log(d[1]);
		}

		/// <summary>Returns true only for 1 or 2 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1 || numParam == 2;
		}

		public override string ToString()
		{
			return "log(x):log(x, y)";
		}
	}
}
