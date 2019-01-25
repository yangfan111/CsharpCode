using Sharpen;
using com.graphbuilder.struc;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree that represents a function.</summary>
	/// <remarks>A node of an expression tree that represents a function.  A FuncNode can have 0 or more children.</remarks>
	public class FuncNode : TermNode
	{
		private Bag bag = new Bag(1);

		private double[] of = new double[1];

		public FuncNode(string name, bool negate)
			: base(name, negate)
		{
		}

		/// <summary>Adds the expression as a child.</summary>
		public virtual void Add(Expression x)
		{
			Insert(x, bag.Size());
		}

		/// <summary>Adds the expression as a child at the specified index.</summary>
		public virtual void Insert(Expression x, int i)
		{
			CheckBeforeAccept(x);
			int oldCap = bag.GetCapacity();
			bag.Insert(x, i);
			int newCap = bag.GetCapacity();
			if (oldCap != newCap)
			{
				of = new double[newCap];
			}
			x.parent = this;
		}

		/// <summary>Removes the specified expression as a child.</summary>
		/// <remarks>Removes the specified expression as a child.  Does nothing if the expression was not a child.</remarks>
		public virtual void Remove(Expression x)
		{
			int size = bag.Size();
			bag.Remove(x);
			if (size != bag.Size())
			{
				x.parent = null;
			}
		}

		/// <summary>Returns the number of child expressions.</summary>
		public virtual int NumChildren()
		{
			return bag.Size();
		}

		/// <summary>Returns the child expression at the specified index.</summary>
		public virtual Expression Child(int i)
		{
			return (Expression)bag.Get(i);
		}

		/// <summary>Evaluates each of the children, storing the result in an internal double array.</summary>
		/// <remarks>
		/// Evaluates each of the children, storing the result in an internal double array.  The FuncMap is
		/// used to obtain a Function object based on the name of this FuncNode.  The function is passed
		/// the double array and returns a result.  If negate is true, the result is negated.  The result
		/// is then returned.  The numParam passed to the function is the number of children of this FuncNode.
		/// </remarks>
		public override double Eval(VarMap v, FuncMap f)
		{
			int numParam = bag.Size();
			for (int i = 0; i < numParam; i++)
			{
				of[i] = Child(i).Eval(v, f);
			}
			double result = f.GetFunction(name, numParam).Of(of, numParam);
			if (negate)
			{
				result = -result;
			}
			return result;
		}
	}
}
