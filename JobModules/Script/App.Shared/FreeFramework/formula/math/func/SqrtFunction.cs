using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The square root function.</summary>
	/// <seealso cref="Sharpen.MyMath.Sqrt(double)"/>
	public class SqrtFunction : Function
	{
		public SqrtFunction()
		{
		}

		/// <summary>Returns the square root of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Sqrt(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "sqrt(x)";
		}
	}
}
