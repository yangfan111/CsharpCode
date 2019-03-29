using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The ceiling function.</summary>
	/// <seealso cref="Sharpen.MyMath.Ceil(double)"/>
	public class CeilFunction : Function
	{
		public CeilFunction()
		{
		}

		/// <summary>Returns the ceiling of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Ceil(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "ceil(x)";
		}
	}
}
