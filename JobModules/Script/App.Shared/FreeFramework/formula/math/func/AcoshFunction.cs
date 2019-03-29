using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The hyperbolic arc cosine function.</summary>
	public class AcoshFunction : Function
	{
		public AcoshFunction()
		{
		}

		/// <summary>
		/// Returns the value of 2 * ln(sqrt((x+1)/2) + sqrt((x-1)/2)), where x is the
		/// value at index location 0.
		/// </summary>
		public virtual double Of(double[] d, int numParam)
		{
			double a = MyMath.Sqrt((d[0] + 1) / 2);
			double b = MyMath.Sqrt((d[0] - 1) / 2);
			return 2 * MyMath.Log(a + b);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "acosh(x)";
		}
	}
}
