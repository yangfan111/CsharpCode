using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The tangent function.</summary>
	/// <seealso cref="Sharpen.MyMath.Tan(double)"/>
	public class TanFunction : Function
	{
		public TanFunction()
		{
		}

		/// <summary>Returns the tangent of the angle value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Tan(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "tan(x)";
		}
	}
}
