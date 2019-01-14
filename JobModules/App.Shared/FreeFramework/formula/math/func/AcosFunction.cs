using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The arc cosine function.</summary>
	/// <seealso cref="Sharpen.MyMath.Acos(double)"/>
	public class AcosFunction : Function
	{
		public AcosFunction()
		{
		}

		/// <summary>Returns the arc cosine of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Acos(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "acos(x)";
		}
	}
}
