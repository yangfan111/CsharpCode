using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The random function.</summary>
	/// <seealso cref="Sharpen.MyMath.Random()"/>
	public class RandFunction : Function
	{
		public RandFunction()
		{
		}

		/// <summary>Returns a random value in the range [0, 1) that does not depend on the input.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Random();
		}

		/// <summary>Returns true only for 0 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 0;
		}

		public override string ToString()
		{
			return "rand()";
		}
	}
}
