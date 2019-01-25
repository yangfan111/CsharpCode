using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The round function.</summary>
	/// <seealso cref="Sharpen.MyMath.Round(double)"/>
	public class RoundFunction : Function
	{
		public RoundFunction()
		{
		}

		/// <summary>Returns the value at d[0] rounded to the nearest integer value.</summary>
		/// <remarks>
		/// Returns the value at d[0] rounded to the nearest integer value.
		/// If the value exceeds the capabilities of long precision then
		/// the value itself is returned.
		/// </remarks>
		public virtual double Of(double[] d, int numParam)
		{
			if (d[0] >= long.MaxValue || d[0] <= long.MinValue)
			{
				return d[0];
			}
			return MyMath.Round(d[0]);
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "round(x)";
		}
	}
}
