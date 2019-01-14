using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The natural logarithm function.</summary>
	/// <seealso cref="Sharpen.MyMath.Log(double)"/>
	public class LnFunction : Function
	{
		public LnFunction()
		{
		}

		/// <summary>Returns the natural logarithm of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Log(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "ln(x)";
		}
	}
}
