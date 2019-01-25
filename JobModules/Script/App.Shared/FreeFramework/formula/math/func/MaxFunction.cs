using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The max function.</summary>
	public class MaxFunction : Function
	{
		public MaxFunction()
		{
		}

		/// <summary>Returns the maximum value of the specified inputs.</summary>
		/// <remarks>Returns the maximum value of the specified inputs.  Double.MAX_VALUE is returned for 0 parameters.</remarks>
		public virtual double Of(double[] d, int numParam)
		{
			if (numParam == 0)
			{
				return double.MaxValue;
			}
			double max = -double.MaxValue;
			for (int i = 0; i < numParam; i++)
			{
				if (d[i] > max)
				{
					max = d[i];
				}
			}
			return max;
		}

		/// <summary>Returns true for 0 or more parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam >= 0;
		}

		public override string ToString()
		{
			return "max(x1, x2, ..., xn)";
		}
	}
}
