using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The exp function.</summary>
	/// <seealso cref="Sharpen.MyMath.Exp(double)"/>
	public class ExpFunction : Function
	{
		public ExpFunction()
		{
		}

		/// <summary>Returns Euler's number, <i>e</i>, raised to the exponent of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Exp(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "exp(x)";
		}
	}
}
