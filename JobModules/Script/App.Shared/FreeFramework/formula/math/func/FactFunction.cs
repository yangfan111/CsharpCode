using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The factorial function.</summary>
	public class FactFunction : Function
	{
		public FactFunction()
		{
		}

		/// <summary>
		/// Takes the (int) of the value at index location 0 and computes the factorial
		/// of that number.
		/// </summary>
		public virtual double Of(double[] d, int numParam)
		{
			int n = (int)d[0];
			double result = 1;
			for (int i = n; i > 1; i--)
			{
				result *= i;
			}
			return result;
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "fact(n)";
		}
	}
}
