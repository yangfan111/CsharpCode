using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The log base 2 function.</summary>
	public class LgFunction : Function
	{
		public LgFunction()
		{
		}

		/// <summary>Returns the log base 2 of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Log(d[0]) / MyMath.Log(2);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "lg(x)";
		}
	}
}
