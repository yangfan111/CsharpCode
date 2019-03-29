using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The arc sine function.</summary>
	/// <seealso cref="Sharpen.MyMath.Asin(double)"/>
	public class AsinFunction : Function
	{
		public AsinFunction()
		{
		}

		/// <summary>Returns the arc sine of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Asin(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "asin(x)";
		}
	}
}
