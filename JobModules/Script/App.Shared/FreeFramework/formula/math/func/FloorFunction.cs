using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The floor function.</summary>
	/// <seealso cref="Sharpen.MyMath.Floor(double)"/>
	public class FloorFunction : Function
	{
		public FloorFunction()
		{
		}

		/// <summary>Returns the floor of the value at index location 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			return MyMath.Floor(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "floor(x)";
		}
	}
}
