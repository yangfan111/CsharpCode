using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree that has exactly 2 children, a left child and a right child.</summary>
	/// <remarks>
	/// A node of an expression tree that has exactly 2 children, a left child and a right child.  After the
	/// children are evaluated, a mathematical operation is applied and the result is returned.
	/// </remarks>
	public abstract class OpNode : Expression
	{
		protected internal Expression leftChild = null;

		protected internal Expression rightChild = null;

		public OpNode(Expression leftChild, Expression rightChild)
		{
			SetLeftChild(leftChild);
			SetRightChild(rightChild);
		}

		public virtual void SetLeftChild(Expression x)
		{
			CheckBeforeAccept(x);
			if (leftChild != null)
			{
				leftChild.parent = null;
			}
			x.parent = this;
			leftChild = x;
		}

		public virtual void SetRightChild(Expression x)
		{
			CheckBeforeAccept(x);
			if (rightChild != null)
			{
				rightChild.parent = null;
			}
			x.parent = this;
			rightChild = x;
		}

		public virtual Expression GetLeftChild()
		{
			return leftChild;
		}

		public virtual Expression GetRightChild()
		{
			return rightChild;
		}

		/// <summary>Returns the text symbol that represents the operation.</summary>
		public abstract string GetSymbol();
	}
}
