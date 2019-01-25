using Sharpen;
using com.graphbuilder.math;

namespace com.graphbuilder.math.func
{
	/// <summary>The combination function.</summary>
	/// <seealso cref="com.graphbuilder.math.PascalsTriangle"/>
	public class CombinFunction : Function
	{
		public CombinFunction()
		{
		}

		/// <summary>Returns the number of ways r items can be chosen from n items.</summary>
		/// <remarks>
		/// Returns the number of ways r items can be chosen from n items.  The value of
		/// n is (int) d[0] and the value of r is (int) d[1].
		/// </remarks>
		public virtual double Of(double[] d, int numParam)
		{
			int n = (int)d[0];
			int r = (int)d[1];
			return PascalsTriangle.NCr(n, r);
		}

		/// <summary>Returns true only for 2 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 2;
		}

		public override string ToString()
		{
			return "combin(n, r)";
		}
	}
}
