using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The arc tangent function.</summary>
	/// <seealso cref="Sharpen.MyMath.Atan(double)"/>
	public class AtanFunction : Function
	{
		public AtanFunction()
		{
		}

		/// <summary>Returns the arc tangent of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Atan(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "atan(x)";
		}
	}
}
