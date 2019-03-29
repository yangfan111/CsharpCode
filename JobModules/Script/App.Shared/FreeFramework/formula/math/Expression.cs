using System;
using System.Text;
using Sharpen;
using com.graphbuilder.struc;

namespace com.graphbuilder.math
{
	/// <summary>The class from which all nodes of an expression tree are descendents.</summary>
	/// <remarks>
	/// The class from which all nodes of an expression tree are descendents.  Expressions can be evaluated
	/// using the eval method.  Expressions that are or have FuncNodes or VarNodes as descendents must provide
	/// a VarMap or FuncMap respectively.  Expressions that consist entirely of OpNodes and ValNodes do not
	/// require a VarMap or FuncMap.  For Expressions that support children (OpNodes, FuncNodes), a child can
	/// only be accepted provided it currently has no parent, a cyclic reference is not formed, and it is
	/// non-null.
	/// </remarks>
	public abstract class Expression
	{
		protected internal Expression parent = null;

		/// <summary>Returns the result of evaluating the expression tree rooted at this node.</summary>
		public abstract double Eval(VarMap v, FuncMap f);

		/// <summary>Returns true if this node is a descendent of the specified node, false otherwise.</summary>
		/// <remarks>
		/// Returns true if this node is a descendent of the specified node, false otherwise.  By this
		/// methods definition, a node is a descendent of itself.
		/// </remarks>
		public virtual bool IsDescendent(Expression x)
		{
			Expression y = this;
			while (y != null)
			{
				if (y == x)
				{
					return true;
				}
				y = y.parent;
			}
			return false;
		}

		/// <summary>Returns the parent of this node.</summary>
		public virtual Expression GetParent()
		{
			return parent;
		}

		/// <summary>
		/// Protected method used to verify that the specified expression can be included as a child
		/// expression of this node.
		/// </summary>
		/// <exception cref="System.ArgumentException">If the specified expression is not accepted.</exception>
		protected internal virtual void CheckBeforeAccept(Expression x)
		{
			if (x == null)
			{
				throw new ArgumentException("expression cannot be null");
			}
			if (x.parent != null)
			{
				throw new ArgumentException("expression must be removed parent");
			}
			if (IsDescendent(x))
			{
				throw new ArgumentException("cyclic reference");
			}
		}

		/// <summary>Returns an array of exact length of the variable names contained in the expression tree rooted at this node.</summary>
		public virtual string[] GetVariableNames()
		{
			return GetTermNames(true);
		}

		/// <summary>Returns an array of exact length of the function names contained in the expression tree rooted at this node.</summary>
		public virtual string[] GetFunctionNames()
		{
			return GetTermNames(false);
		}

		private string[] GetTermNames(bool varNames)
		{
			Bag b = new Bag();
			GetTermNames(this, b, varNames);
			string[] arr = new string[b.Size()];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = (string)b.Get(i);
			}
			return arr;
		}

		private static void GetTermNames(Expression x, Bag b, bool varNames)
		{
			if (x is OpNode)
			{
				OpNode o = (OpNode)x;
				GetTermNames(o.leftChild, b, varNames);
				GetTermNames(o.rightChild, b, varNames);
			}
			else
			{
				if (x is VarNode)
				{
					if (varNames)
					{
						VarNode v = (VarNode)x;
						if (!b.Contains(v.name))
						{
							b.Add(v.name);
						}
					}
				}
				else
				{
					if (x is FuncNode)
					{
						FuncNode f = (FuncNode)x;
						if (!varNames)
						{
							if (!b.Contains(f.name))
							{
								b.Add(f.name);
							}
						}
						for (int i = 0; i < f.NumChildren(); i++)
						{
							GetTermNames(f.Child(i), b, varNames);
						}
					}
				}
			}
		}

		/// <summary>Returns a string that represents the expression tree rooted at this node.</summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			ToString(this, sb);
			return sb.ToString();
		}

		private static void ToString(Expression x, StringBuilder sb)
		{
			if (x is OpNode)
			{
				OpNode o = (OpNode)x;
				sb.Append("(");
				ToString(o.leftChild, sb);
				sb.Append(o.GetSymbol());
				ToString(o.rightChild, sb);
				sb.Append(")");
			}
			else
			{
				if (x is TermNode)
				{
					TermNode t = (TermNode)x;
					if (t.GetNegate())
					{
						sb.Append("(");
						sb.Append("-");
					}
					sb.Append(t.GetName());
					if (t is FuncNode)
					{
						FuncNode f = (FuncNode)t;
						sb.Append("(");
						if (f.NumChildren() > 0)
						{
							ToString(f.Child(0), sb);
						}
						for (int i = 1; i < f.NumChildren(); i++)
						{
							sb.Append(", ");
							ToString(f.Child(i), sb);
						}
						sb.Append(")");
					}
					if (t.GetNegate())
					{
						sb.Append(")");
					}
				}
				else
				{
					if (x is ValNode)
					{
						sb.Append(((ValNode)x).val);
					}
				}
			}
		}
	}
}
