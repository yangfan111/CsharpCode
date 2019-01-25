using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The average function.</summary>
	public class AvgFunction : Function
	{
		public AvgFunction()
		{
		}

		/// <summary>Returns the average of the values in the array from [0, numParam).</summary>
		public virtual double Of(double[] d, int numParam)
		{
			double sum = 0;
			for (int i = 0; i < numParam; i++)
			{
				sum += d[i];
			}
			return sum / numParam;
		}

		/// <summary>Returns true for 1 or more parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam > 0;
		}

		public override string ToString()
		{
			return "avg(x1, x2, ..., xn)";
		}
	}
}
