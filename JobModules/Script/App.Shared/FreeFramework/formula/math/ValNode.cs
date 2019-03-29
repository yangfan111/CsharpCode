using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree that represents a value.</summary>
	/// <remarks>A node of an expression tree that represents a value.  A ValNode cannot have any children.</remarks>
	public class ValNode : Expression
	{
		protected internal double val = 0.0;

		public ValNode(double d)
		{
			val = d;
		}

		/// <summary>Returns the value.</summary>
		public override double Eval(VarMap v, FuncMap f)
		{
			return val;
		}

		public virtual double GetValue()
		{
			return val;
		}

		public virtual void SetValue(double d)
		{
			val = d;
		}
	}
}
