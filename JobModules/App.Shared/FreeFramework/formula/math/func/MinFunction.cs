using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The min function.</summary>
	public class MinFunction : Function
	{
		public MinFunction()
		{
		}

		/// <summary>Returns the minimum value of the specified inputs.</summary>
		/// <remarks>Returns the minimum value of the specified inputs.  Double.MIN_VALUE is returned for 0 parameters.</remarks>
		public virtual double Of(double[] d, int numParam)
		{
			if (numParam == 0)
			{
				return double.MinValue;
			}
			double min = double.MaxValue;
			for (int i = 0; i < numParam; i++)
			{
				if (d[i] < min)
				{
					min = d[i];
				}
			}
			return min;
		}

		/// <summary>Returns true for 0 or more parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam >= 0;
		}

		public override string ToString()
		{
			return "min(x1, x2, ..., xn)";
		}
	}
}
