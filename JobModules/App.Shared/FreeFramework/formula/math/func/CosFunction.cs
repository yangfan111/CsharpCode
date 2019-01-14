using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The cosine function.</summary>
	/// <seealso cref="Sharpen.MyMath.Cos(double)"/>
	public class CosFunction : Function
	{
		public CosFunction()
		{
		}

		/// <summary>Returns the cosine of the angle value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Cos(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "cos(x)";
		}
	}
}
