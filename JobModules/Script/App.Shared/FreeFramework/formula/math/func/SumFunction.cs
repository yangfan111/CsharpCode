using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The sum function.</summary>
	public class SumFunction : Function
	{
		public SumFunction()
		{
		}

		/// <summary>Returns the sum of the values in the array from [0, numParam).</summary>
		public virtual double Of(double[] d, int numParam)
		{
			double sum = 0;
			for (int i = 0; i < numParam; i++)
			{
				sum += d[i];
			}
			return sum;
		}

		/// <summary>Returns true for 1 or more parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam > 0;
		}

		public override string ToString()
		{
			return "sum(x1, x2, ..., xn)";
		}
	}
}
