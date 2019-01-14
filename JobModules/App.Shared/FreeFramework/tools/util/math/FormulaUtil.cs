using System.Collections.Generic;
using Sharpen;
using com.graphbuilder.math;
using com.graphbuilder.math.func;

namespace com.cpkf.yyjd.tools.util.math
{
	public class FormulaUtil
	{
		private static FuncMap fm;

		static FormulaUtil()
		{
			fm = new FuncMap();
			fm.SetFunction("sin", new SinFunction());
			fm.SetFunction("cos", new CosFunction());
            fm.SetFunction("acos", new AcosFunction());
            fm.SetFunction("asin", new AsinFunction());
            fm.SetFunction("atan", new AtanFunction());
            fm.SetFunction("tan", new TanFunction());
            fm.SetFunction("cos", new CosFunction());
            fm.SetFunction("max", new MaxFunction());
			fm.SetFunction("min", new MinFunction());
			fm.SetFunction("abs", new AbsFunction());
			fm.SetFunction("log", new LogFunction());
			fm.SetFunction("pow", new PowFunction());
			fm.SetFunction("mod", new ModFunction());
		}

		public static double GetValue(string formula, MyDictionary<string, double> map)
		{
			Expression expression = ExpressionTree.Parse(formula);
			VarMap vm = new VarMap(false);
			foreach (string key in map.Keys)
			{
				vm.SetValue(key, map[key]);
			}
			return expression.Eval(vm, fm);
		}
	}
}
