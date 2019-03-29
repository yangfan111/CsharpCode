using System;
using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary>A node of an expression tree that represents a variable or a function.</summary>
	public abstract class TermNode : Expression
	{
		protected internal string name = null;

		protected internal bool negate = false;

		public TermNode(string name, bool negate)
		{
			SetName(name);
			SetNegate(negate);
		}

		/// <summary>Returns true if the term should negate the result before returning it in the eval method.</summary>
		public virtual bool GetNegate()
		{
			return negate;
		}

		public virtual void SetNegate(bool b)
		{
			negate = b;
		}

		/// <summary>Returns the name of the term.</summary>
		public virtual string GetName()
		{
			return name;
		}

		/// <summary>Sets the name of the term.</summary>
		/// <remarks>
		/// Sets the name of the term.  Valid names must not begin with a digit or a decimal, and must not contain
		/// round brackets, operators, commas or whitespace.
		/// </remarks>
		/// <exception cref="System.ArgumentException">If the name is null or invalid.</exception>
		public virtual void SetName(string s)
		{
			if (s == null)
			{
				throw new ArgumentException("name cannot be null");
			}
			if (!IsValidName(s))
			{
				throw new ArgumentException("invalid name: " + s);
			}
			name = s;
		}

		private static bool IsValidName(string s)
		{
			if (s.Length == 0)
			{
				return false;
			}
			char c = s[0];
			if (c >= '0' && c <= '9' || c == '.' || c == ',' || c == '(' || c == ')' || c == '^' || c == '*' || c == '/' || c == '+' || c == '-' || c == ' ' || c == '\t' || c == '\n')
			{
				return false;
			}
			for (int i = 1; i < s.Length; i++)
			{
				c = s[i];
				if (c == ',' || c == '(' || c == ')' || c == '^' || c == '*' || c == '/' || c == '+' || c == '-' || c == ' ' || c == '\t' || c == '\n')
				{
					return false;
				}
			}
			return true;
		}
	}
}
