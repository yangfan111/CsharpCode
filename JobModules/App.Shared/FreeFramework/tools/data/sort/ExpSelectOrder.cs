using System;
using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.condition;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class ExpSelectOrder : ISelectOrder, IExpParser<ISelectOrder>
	{
		private const long serialVersionUID = -5123723305215251206L;

		private static IList<IExpressionParser> parsers;

		private ClauseSelectOrder order;

		static ExpSelectOrder()
		{
			parsers = new List<IExpressionParser>();
			parsers.Add(new BooleanExpressionParser());
			parsers.Add(new NumberSequenceExpressionParser());
			parsers.Add(new NumberRangeExpressionParser());
			parsers.Add(new StringSequenceExpressionParser());
			parsers.Add(new StringRangeExpressionParser());
			parsers.Add(new CharOrderExpressionParser());
		}

		private const string SPLITER_TYPE = ":";

		private string expression;

		public ExpSelectOrder()
		{
		}

		public ExpSelectOrder(string expression)
			: base()
		{
			this.expression = expression;
		}

		public virtual ICollection<string> GetAllFeatures()
		{
			if (order == null)
			{
				order = (ClauseSelectOrder)new ClauseParser<ISelectOrder>().Parse(expression, this, new ClauseSelectOrder());
			}
			return order.GetAllFeatures();
		}

		public virtual DataBlocks Sort(DataBlock block)
		{
			if (order == null)
			{
				order = (ClauseSelectOrder)new ClauseParser<ISelectOrder>().Parse(expression, this, new ClauseSelectOrder());
			}
			return order.Sort(block);
		}

		public virtual com.cpkf.yyjd.tools.data.sort.ExpSelectOrder Clone()
		{
			return new com.cpkf.yyjd.tools.data.sort.ExpSelectOrder();
		}

		public virtual string GetExpression()
		{
			return expression;
		}

		public virtual void SetExpression(string expression)
		{
			this.expression = expression;
		}

		public virtual ISelectOrder Parse(string exp)
		{
			if (!StringUtil.IsNullOrEmpty(exp))
			{
				string[] ss = exp.Split(SPLITER_TYPE);
				if (ss.Length == 2)
				{
					FeatureSelectOrder featureOrder = new FeatureSelectOrder();
					featureOrder.SetFeature(ss[0].Trim());
					featureOrder.SetValue(GetOrderValue(ss[1]));
					return featureOrder;
				}
			}
			throw new ConditionParseException("错误的格式" + exp);
		}

		private IValueOrder GetOrderValue(string type)
		{
			foreach (IExpressionParser parser in parsers)
			{
				if (parser.CanParse(type))
				{
					return parser.Parse(type);
				}
			}
			throw new Exception("can not parse type:" + type);
		}
	}
}
