using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The mod function.</summary>
	public class ModFunction : Function
	{
		public ModFunction()
		{
		}

		/// <summary>Returns the value of x % y, where x = d[0] and y = d[1].</summary>
		/// <remarks>
		/// Returns the value of x % y, where x = d[0] and y = d[1].  More precisely, the value returned is
		/// x minus the value of x / y, where x / y is rounded to the closest integer value towards 0.
		/// </remarks>
		public virtual double Of(double[] d, int numParam)
		{
			return d[0] % d[1];
		}

		/// <summary>Returns true only for 2 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 2;
		}

		public override string ToString()
		{
			return "mod(x, y)";
		}
	}
}
