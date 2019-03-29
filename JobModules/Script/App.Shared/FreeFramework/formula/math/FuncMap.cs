using System;
using Sharpen;
using com.graphbuilder.math.func;

namespace com.graphbuilder.math
{
	/// <summary><p>FuncMap maps a name to a function.</summary>
	/// <remarks>
	/// <p>FuncMap maps a name to a function.  A FuncMap is used in the eval method of an Expression object.
	/// This class can be used as the default function-map.  The loadDefaultFunctions() method can be used
	/// to take advantage of the many already implemented functions (see below).
	/// <p>During the evaluation of an expression, if a function is not supported then a RuntimeException is thrown.
	/// <p>Default functions:
	/// <p>
	/// <dl>
	/// <dt>No Parameters</dt>
	/// <dd>
	/// <ul>
	/// <li>e() &rarr; Math.E</li>
	/// <li>pi() &rarr; Math.PI</li>
	/// <li>rand() &rarr; Math.random()</li>
	/// <li>min() &rarr; Double.MIN_VALUE</li>
	/// <li>max() &rarr; Double.MAX_VALUE</li>
	/// </ul>
	/// </dd>
	/// <dt>1 Parameter</dt>
	/// <dd>
	/// <ul>
	/// <li>sin(x) &rarr; Math.sin(double)</li>
	/// <li>cos(x) &rarr; Math.cos(double)</li>
	/// <li>tan(x) &rarr; Math.tan(double)</li>
	/// <li>asin(x) &rarr; Math.asin(double)</li>
	/// <li>acos(x) &rarr; Math.acos(double)</li>
	/// <li>atan(x) &rarr; Math.atan(double)</li>
	/// <li>asinh(x) &rarr; 2 * ln(sqrt((x+1)/2) + sqrt((x-1)/2))</li>
	/// <li>acosh(x) &rarr; ln(x + sqrt(1 + x<sup>2</sup>))</li>
	/// <li>atanh(x) &rarr; (ln(1+x) - ln(1-x)) / 2</li>
	/// <li>sinh(x) &rarr; (<i>e<sup>x</sup>&nbsp;-&nbsp;e<sup>-x</sup></i>)/2</li>
	/// <li>cosh(x) &rarr; (<i>e<sup>x</sup>&nbsp;+&nbsp;e<sup>-x</sup></i>)/2</li>
	/// <li>tanh(x) &rarr; (<i>e<sup>x</sup>&nbsp;-&nbsp;e<sup>-x</sup></i>)/(<i>e<sup>x</sup>&nbsp;+&nbsp;e<sup>-x</sup></i>)</li>
	/// <li>sqrt(x) &rarr; Math.sqrt(double)</li>
	/// <li>abs(x) &rarr; Math.abs(double)</li>
	/// <li>ceil(x) &rarr; Math.ceil(double)</li>
	/// <li>floor(x) &rarr; Math.floor(double)</li>
	/// <li>exp(x) &rarr; <i>e</i><sup>x</sup></li>
	/// <li>ln(x) &rarr; log<sub><i>e</i></sub>x</li>
	/// <li>lg(x) &rarr; log<sub>2</sub>x</li>
	/// <li>log(x) &rarr; log<sub>10</sub>x</li>
	/// <li>sign(x) &rarr; x &gt; 0 = 1, x &lt; 0 = -1, else 0</li>
	/// <li>fact(n) &rarr; n! = 1 * 2 * ... * (n - 1) * n</li>
	/// <li>round(x) &rarr; Math.round(double)</li>
	/// </ul>
	/// </dd>
	/// <dt>2 Parameters</dt>
	/// <dd>
	/// <ul>
	/// <li>log(x,y) &rarr; log<sub>y</sub>x</li>
	/// <li>combin(n, r) &rarr; PascalsTriangle.nCr(n, r)</li>
	/// <li>mod(x, y) &rarr; x % y</li>
	/// <li>pow(x, y) &rarr; x<sup>y</sup></li>
	/// </ul>
	/// </dd>
	/// <dt>n Parameters</dt>
	/// <dd>
	/// <ul>
	/// <li>min(x1,x2,...,xn)</li>
	/// <li>max(x1,x2,...,xn)</li>
	/// <li>sum(x1,x2,...,xn) &rarr; x1 + x2 + ... + xn</li>
	/// <li>avg(x1,x2,...,xn) &rarr; (x1 + x2 + ... + xn) / n</li>
	/// </ul>
	/// </dd>
	/// </dl>
	/// </p>
	/// <p>Note: Case sensitivity can only be specified in the constructor (for consistency).  When case sensitivity is false,
	/// the String.equalsIgnoreCase method is used.  When case sensitivity is true, the String.equals method is used.  The
	/// matching does not include the parenthesis.  For example, when case sensitivity is false and the default functions have been
	/// loaded, then "RaNd", "rand", and "RAND" all map to the RandFunction().  By default, case sensitivity is false.
	/// </remarks>
	public class FuncMap
	{
		private string[] name = new string[50];

		private Function[] func = new Function[50];

		private int numFunc = 0;

		private bool caseSensitive = false;

		public FuncMap()
		{
		}

		public FuncMap(bool caseSensitive)
		{
			this.caseSensitive = caseSensitive;
		}

		/// <summary>Adds the mappings for many common functions.</summary>
		/// <remarks>Adds the mappings for many common functions.  The names are specified in all lowercase letters.</remarks>
		public virtual void LoadDefaultFunctions()
		{
			// >= 0 parameters
			SetFunction("min", new MinFunction());
			SetFunction("max", new MaxFunction());
			// > 0 parameters
			SetFunction("sum", new SumFunction());
			SetFunction("avg", new AvgFunction());
			// 0 parameters
			SetFunction("pi", new PiFunction());
			SetFunction("e", new EFunction());
			SetFunction("rand", new RandFunction());
			// 1 parameter
			SetFunction("sin", new SinFunction());
			SetFunction("cos", new CosFunction());
			SetFunction("tan", new TanFunction());
			SetFunction("sqrt", new SqrtFunction());
			SetFunction("abs", new AbsFunction());
			SetFunction("ceil", new CeilFunction());
			SetFunction("floor", new FloorFunction());
			SetFunction("exp", new ExpFunction());
			SetFunction("lg", new LgFunction());
			SetFunction("ln", new LnFunction());
			SetFunction("sign", new SignFunction());
			SetFunction("round", new RoundFunction());
			SetFunction("fact", new FactFunction());
			SetFunction("cosh", new CoshFunction());
			SetFunction("sinh", new SinhFunction());
			SetFunction("tanh", new TanhFunction());
			SetFunction("acos", new AcosFunction());
			SetFunction("asin", new AsinFunction());
			SetFunction("atan", new AtanFunction());
			SetFunction("acosh", new AcoshFunction());
			SetFunction("asinh", new AsinhFunction());
			SetFunction("atanh", new AtanhFunction());
			// 2 parameters
			SetFunction("pow", new PowFunction());
			SetFunction("mod", new ModFunction());
			SetFunction("combin", new CombinFunction());
			// 1 or 2 parameters
			SetFunction("log", new LogFunction());
		}

		/// <summary>Returns a function based on the name and the specified number of parameters.</summary>
		/// <exception cref="System.Exception">If no supporting function can be found.</exception>
		public virtual Function GetFunction(string funcName, int numParam)
		{
			for (int i = 0; i < numFunc; i++)
			{
				if (func[i].AcceptNumParam(numParam) && (caseSensitive && name[i].Equals(funcName) || !caseSensitive && Sharpen.Runtime.EqualsIgnoreCase(name[i], funcName)))
				{
					return func[i];
				}
			}
			throw new Exception("function not found: " + funcName + " " + numParam);
		}

		/// <summary>Assigns the name to map to the specified function.</summary>
		/// <exception cref="System.ArgumentException">If any of the parameters are null.</exception>
		public virtual void SetFunction(string funcName, Function f)
		{
			if (funcName == null)
			{
				throw new ArgumentException("function name cannot be null");
			}
			if (f == null)
			{
				throw new ArgumentException("function cannot be null");
			}
			for (int i = 0; i < numFunc; i++)
			{
				if (caseSensitive && name[i].Equals(funcName) || !caseSensitive && Sharpen.Runtime.EqualsIgnoreCase(name[i], funcName))
				{
					func[i] = f;
					return;
				}
			}
			if (numFunc == name.Length)
			{
				string[] tmp1 = new string[2 * numFunc];
				Function[] tmp2 = new Function[tmp1.Length];
				for (int i_1 = 0; i_1 < numFunc; i_1++)
				{
					tmp1[i_1] = name[i_1];
					tmp2[i_1] = func[i_1];
				}
				name = tmp1;
				func = tmp2;
			}
			name[numFunc] = funcName;
			func[numFunc] = f;
			numFunc++;
		}

		/// <summary>Returns true if the case of the function names is considered.</summary>
		public virtual bool IsCaseSensitive()
		{
			return caseSensitive;
		}

		/// <summary>Returns an array of exact length of the function names stored in this map.</summary>
		public virtual string[] GetFunctionNames()
		{
			string[] arr = new string[numFunc];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = name[i];
			}
			return arr;
		}

		/// <summary>Returns an array of exact length of the functions stored in this map.</summary>
		/// <remarks>
		/// Returns an array of exact length of the functions stored in this map.  The returned
		/// array corresponds to the order of the names returned by getFunctionNames.
		/// </remarks>
		public virtual Function[] GetFunctions()
		{
			Function[] arr = new Function[numFunc];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = func[i];
			}
			return arr;
		}

		/// <summary>Removes the function-name and the associated function from the map.</summary>
		/// <remarks>
		/// Removes the function-name and the associated function from the map.  Does nothing if the function-name
		/// is not found.
		/// </remarks>
		public virtual void Remove(string funcName)
		{
			for (int i = 0; i < numFunc; i++)
			{
				if (caseSensitive && name[i].Equals(funcName) || !caseSensitive && Sharpen.Runtime.EqualsIgnoreCase(name[i], funcName))
				{
					for (int j = i + 1; j < numFunc; j++)
					{
						name[j - 1] = name[j];
						func[j - 1] = func[j];
					}
					numFunc--;
					name[numFunc] = null;
					func[numFunc] = null;
					break;
				}
			}
		}
	}
}
