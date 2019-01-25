using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The power function.</summary>
	/// <seealso cref="Sharpen.MyMath.Pow(double, double)"/>
	public class PowFunction : Function
	{
		public PowFunction()
		{
		}

		/// <summary>
		/// Returns the value at index location 0 to the exponent of the value
		/// at index location 1.
		/// </summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Pow(d[0], d[1]);
		}

		/// <summary>Returns true only for 2 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 2;
		}

		public override string ToString()
		{
			return "pow(x, y)";
		}
	}
}
