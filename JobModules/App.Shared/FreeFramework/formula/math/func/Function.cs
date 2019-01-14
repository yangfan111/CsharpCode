using Sharpen;

namespace com.graphbuilder.math.func
{
	/// <summary>
	/// The Function interface represents a function that takes a number of inputs
	/// and returns a value.
	/// </summary>
	/// <remarks>
	/// The Function interface represents a function that takes a number of inputs
	/// and returns a value.  The number of inputs expected depends on the function.
	/// For example, the PiFunction returns the value of Pi regardless of the input.
	/// The CosFunction uses the value at index location 0.  The PowFunction uses the
	/// values at index locations 0 and 1.  See the FuncMap class for more information.
	/// </remarks>
	/// <seealso cref="com.graphbuilder.math.FuncMap"/>
	public interface Function
	{
		/// <summary>Takes the specified double array as input and returns a double value.</summary>
		/// <remarks>
		/// Takes the specified double array as input and returns a double value.  Functions
		/// that accept a variable number of inputs can take numParam to be the number of inputs.
		/// </remarks>
		double Of(double[] param, int numParam);

		/// <summary>
		/// Returns true if the numParam is an accurate representation of the number of inputs
		/// the function processes.
		/// </summary>
		bool AcceptNumParam(int numParam);
	}
}
