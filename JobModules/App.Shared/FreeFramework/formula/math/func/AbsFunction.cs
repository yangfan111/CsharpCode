using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The absolute function.</summary>
	/// <seealso cref="Sharpen.MyMath.Abs(double)"/>
	public class AbsFunction : Function
	{
		public AbsFunction()
		{
		}

		/// <summary>Returns the positive value of the value stored at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Abs(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "abs(x)";
		}
	}
}
