using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree, represented by the symbol "-".</summary>
	public class SubNode : OpNode
	{
		public SubNode(Expression leftChild, Expression rightChild)
			: base(leftChild, rightChild)
		{
		}

		/// <summary>Subtracts the evaluation of the right side from the evaluation of the left side and returns the result.</summary>
		public override double Eval(VarMap v, FuncMap f)
		{
			double a = leftChild.Eval(v, f);
			double b = rightChild.Eval(v, f);
			return a - b;
		}

		public override string GetSymbol()
		{
			return "-";
		}
	}
}
