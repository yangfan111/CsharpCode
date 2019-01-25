using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The hyperbolic tangent sine function.</summary>
	public class AtanhFunction : Function
	{
		public AtanhFunction()
		{
		}

		/// <summary>Returns the value of (ln(1+x) - ln(1-x)) / 2, where x is the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return (MyMath.Log(1 + d[0]) - MyMath.Log(1 - d[0])) / 2;
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "atanh(x)";
		}
	}
}
