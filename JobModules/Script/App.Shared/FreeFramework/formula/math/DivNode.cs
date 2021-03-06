using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree, represented by the symbol "/".</summary>
	public class DivNode : OpNode
	{
		public DivNode(Expression leftChild, Expression rightChild)
			: base(leftChild, rightChild)
		{
		}

		/// <summary>Divides the evaluation of the left side by the evaluation of the right side and returns the result.</summary>
		public override double Eval(VarMap v, FuncMap f)
		{
			double a = leftChild.Eval(v, f);
			double b = rightChild.Eval(v, f);
			return a / b;
		}

		public override string GetSymbol()
		{
			return "/";
		}
	}
}
