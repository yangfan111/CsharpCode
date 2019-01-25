using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The hyperbolic tangent function.</summary>
	public class TanhFunction : Function
	{
		public TanhFunction()
		{
		}

		/// <summary>
		/// Returns the value of (<i>e<sup>x</sup>&nbsp;-&nbsp;e<sup>-x</sup></i>)/(<i>e<sup>x</sup>&nbsp;+&nbsp;e<sup>-x</sup></i>),
		/// where x is the value at index location 0 and <i>e</i> is the base of natural logarithms.
		/// </summary>
		public virtual double Of(double[] d, int numParam)
		{
			double e = MyMath.Pow(MyMath.E, 2 * d[0]);
			return (e - 1) / (e + 1);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "tanh(x)";
		}
	}
}
