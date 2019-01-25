using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The hyperbolic arc sine function.</summary>
	public class AsinhFunction : Function
	{
		public AsinhFunction()
		{
		}

		/// <summary>
		/// Returns the value of ln(x + sqrt(1 + x<sup>2</sup>)), where x is the value at index
		/// location 0.
		/// </summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Log(d[0] + MyMath.Sqrt(1 + d[0] * d[0]));
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "asinh(x)";
		}
	}
}
