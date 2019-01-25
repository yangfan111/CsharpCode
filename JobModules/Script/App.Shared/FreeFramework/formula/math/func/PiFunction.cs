using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The constant Pi.</summary>
	/// <seealso cref="Sharpen.MyMath.PI"/>
	public class PiFunction : Function
	{
		public PiFunction()
		{
		}

		/// <summary>Returns the constant Pi regardless of the input.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.PI;
		}

		/// <summary>Returns true only for 0 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 0;
		}

		public override string ToString()
		{
			return "pi()";
		}
	}
}
