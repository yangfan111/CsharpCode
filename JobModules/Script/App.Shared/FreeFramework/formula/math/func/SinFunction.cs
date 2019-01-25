using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The sine function.</summary>
	/// <seealso cref="Sharpen.MyMath.Sin(double)"/>
	public class SinFunction : Function
	{
		public SinFunction()
		{
		}

		/// <summary>Returns the sine of the angle value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Sin(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "sin(x)";
		}
	}
}
