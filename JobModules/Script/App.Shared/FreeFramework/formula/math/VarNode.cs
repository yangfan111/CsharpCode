using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree that represents a variable.</summary>
	/// <remarks>A node of an expression tree that represents a variable.  A VarNode cannot have any children.</remarks>
	public class VarNode : TermNode
	{
		public VarNode(string name, bool negate)
			: base(name, negate)
		{
		}

		/// <summary>Returns the value associated with the variable name in the VarMap.</summary>
		public override double Eval(VarMap v, FuncMap f)
		{
			double val = v.GetValue(name);
			if (negate)
			{
				val = -val;
			}
			return val;
		}
	}
}
