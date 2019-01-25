using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The hyperbolic cosine function.</summary>
	public class CoshFunction : Function
	{
		public CoshFunction()
		{
		}

		/// <summary>
		/// Returns the value of (<i>e<sup>x</sup>&nbsp;+&nbsp;e<sup>-x</sup></i>)/2, where x is the value
		/// at index location 0 and <i>e</i> is the base of natural logarithms.
		/// </summary>
		public virtual double Of(double[] d, int numParam)
		{
			return (MyMath.Pow(MyMath.E, d[0]) + MyMath.Pow(MyMath.E, -d[0])) / 2;
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "cosh(x)";
		}
	}
}
