using System;
using Sharpen;

namespace com.graphbuilder.math
{
	/// <summary><p>VarMap maps a name to a value.</summary>
	/// <remarks>
	/// <p>VarMap maps a name to a value.  A VarMap is used in the eval method of an Expression object.
	/// This class can be used as the default variable-map.
	/// <p>During the evaluation of an expression, if a variable is not supported then a RuntimeException is thrown.
	/// Case sensitivity can only be specified in the constructor (for consistency).  When case sensitivity is false,
	/// the String.equalsIgnoreCase method is used.  When case sensitivity is true, the String.equals method is used.
	/// By default, case sensitivity is true.
	/// </remarks>
	public class VarMap
	{
		private bool caseSensitive = true;

		private string[] name = new string[2];

		private double[] value = new double[2];

		private int numVars = 0;

		public VarMap()
			: this(true)
		{
		}

		public VarMap(bool caseSensitive)
		{
			this.caseSensitive = caseSensitive;
		}

		/// <summary>Returns the value associated with the specified variable name.</summary>
		/// <exception cref="System.Exception">If a matching variable name cannot be found.</exception>
		public virtual double GetValue(string varName)
		{
			for (int i = 0; i < numVars; i++)
			{
				if (caseSensitive && name[i].Equals(varName) || !caseSensitive && Sharpen.Runtime.EqualsIgnoreCase(name[i], varName))
				{
					return value[i];
				}
			}
			throw new Exception("variable value has not been set: " + varName);
		}

		/// <summary>Assigns the value to the specified variable name.</summary>
		/// <exception cref="System.ArgumentException">If the variable name is null.</exception>
		public virtual void SetValue(string varName, double val)
		{
			if (varName == null)
			{
				throw new ArgumentException("varName cannot be null");
			}
			for (int i = 0; i < numVars; i++)
			{
				if (caseSensitive && name[i].Equals(varName) || !caseSensitive && Sharpen.Runtime.EqualsIgnoreCase(name[i], varName))
				{
					value[i] = val;
					return;
				}
			}
			if (numVars == name.Length)
			{
				string[] tmp1 = new string[2 * numVars];
				double[] tmp2 = new double[tmp1.Length];
				for (int i_1 = 0; i_1 < numVars; i_1++)
				{
					tmp1[i_1] = name[i_1];
					tmp2[i_1] = value[i_1];
				}
				name = tmp1;
				value = tmp2;
			}
			name[numVars] = varName;
			value[numVars] = val;
			numVars++;
		}

		/// <summary>Returns true if the case of the variable names is considered.</summary>
		public virtual bool IsCaseSensitive()
		{
			return caseSensitive;
		}

		/// <summary>Returns an array of exact length of the variable names stored in this map.</summary>
		public virtual string[] GetVariableNames()
		{
			string[] arr = new string[numVars];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = name[i];
			}
			return arr;
		}

		/// <summary>Returns an array of exact length of the values stored in this map.</summary>
		/// <remarks>
		/// Returns an array of exact length of the values stored in this map.  The returned
		/// array corresponds to the order of the names returned by getVariableNames.
		/// </remarks>
		public virtual double[] GetValues()
		{
			double[] arr = new double[numVars];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = value[i];
			}
			return arr;
		}

		/// <summary>Removes the variable-name from the map.</summary>
		/// <remarks>Removes the variable-name from the map. Does nothing if the variable-name is not found.</remarks>
		public virtual void Remove(string varName)
		{
			for (int i = 0; i < numVars; i++)
			{
				if (caseSensitive && name[i].Equals(varName) || !caseSensitive && Sharpen.Runtime.EqualsIgnoreCase(name[i], varName))
				{
					for (int j = i + 1; j < numVars; j++)
					{
						name[j - 1] = name[j];
						value[j - 1] = value[j];
					}
					numVars--;
					name[numVars] = null;
					value[numVars] = 0;
					break;
				}
			}
		}
	}
}
