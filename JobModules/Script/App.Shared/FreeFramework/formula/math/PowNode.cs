using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree, represented by the symbol "^".</summary>
	public class PowNode : OpNode
	{
		public PowNode(Expression leftChild, Expression rightChild)
			: base(leftChild, rightChild)
		{
		}

		/// <summary>Raises the evaluation of the left side to the power of the evaluation of the right side and returns the result.</summary>
		public override double Eval(VarMap v, FuncMap f)
		{
			double a = leftChild.Eval(v, f);
			double b = rightChild.Eval(v, f);
			return MyMath.Pow(a, b);
		}

		public override string GetSymbol()
		{
			return "^";
		}
	}
}
