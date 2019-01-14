using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>The sign function.</summary>
	public class SignFunction : Function
	{
		public SignFunction()
		{
		}

		/// <summary>The sign function returns 1 if the d[0] &gt; 0, -1 if d[0] &lt; 0, else 0.</summary>
		public virtual double Of(double[] d, int numParam)
		{
			if (d[0] > 0)
			{
				return 1;
			}
			if (d[0] < 0)
			{
				return -1;
			}
			return 0;
		}

		/// <summary>Returns true only for 1 parameter, false otherwise.</summary>
		public virtual bool AcceptNumParam(int numParam)
		{
			return numParam == 1;
		}

		public override string ToString()
		{
			return "sign(x)";
		}
	}
}
