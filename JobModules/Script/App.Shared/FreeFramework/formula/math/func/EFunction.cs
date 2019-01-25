using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>Euler's number, <i>e</i>, also called the base of natural logarithms.</summary>
	/// <seealso cref="Sharpen.MyMath.E"/>
	public class EFunction : Function
	{
		public EFunction()
		{
		}

		/// <summary>Returns the constant <i>e</i> regardless of the input.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.E;
		}

		/// <summary>Returns true only for 0 parameters, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 0;
		}

		public override string ToString()
		{
			return "e()";
		}
	}
}
