using System;
using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class SimpleSelectOrder : ISelectOrder
	{
		private const long serialVersionUID = -5787950387292672718L;

		private const string SPLITER_FEATURE = ",";

		private const string SPLITER_TYPE = ":";

		private string andFeatures;

		private string orFeatures;

		private static IList<IExpressionParser> parsers;

		static SimpleSelectOrder()
		{
			parsers = new List<IExpressionParser>();
			parsers.Add(new BooleanExpressionParser());
			parsers.Add(new NumberSequenceExpressionParser());
			parsers.Add(new NumberRangeExpressionParser());
			parsers.Add(new StringSequenceExpressionParser());
			parsers.Add(new StringRangeExpressionParser());
		}

		public SimpleSelectOrder()
		{
		}

		public SimpleSelectOrder(string andFeatures, string orFeatures)
		{
			this.andFeatures = andFeatures;
			this.orFeatures = orFeatures;
		}

		public virtual DataBlocks Sort(DataBlock block)
		{
			ClauseSelectOrder clause = new ClauseSelectOrder();
			IList<FeatureSelectOrder> andOrders = BuildOrder(andFeatures);
			foreach (FeatureSelectOrder order in andOrders)
			{
				clause.AddAndSelectOrder(order);
			}
			IList<FeatureSelectOrder> orOrders = BuildOrder(orFeatures);
			foreach (FeatureSelectOrder order_1 in orOrders)
			{
				clause.AddOrSelectOrder(order_1);
			}
			return clause.Sort(block);
		}

		public virtual string GetAndFeatures()
		{
			return andFeatures;
		}

		public virtual void SetAndFeatures(string andFeatures)
		{
			this.andFeatures = andFeatures;
		}

		public virtual string GetOrFeatures()
		{
			return orFeatures;
		}

		public virtual void SetOrFeatures(string orFeatures)
		{
			this.orFeatures = orFeatures;
		}

		private IList<FeatureSelectOrder> BuildOrder(string features)
		{
			IList<FeatureSelectOrder> orders = new List<FeatureSelectOrder>();
			if (!StringUtil.IsNullOrEmpty(features))
			{
				foreach (string feature in features.Split(SPLITER_FEATURE))
				{
					string[] ss = feature.Split(SPLITER_TYPE);
					if (ss.Length == 2)
					{
						FeatureSelectOrder featureOrder = new FeatureSelectOrder();
						featureOrder.SetFeature(ss[0].Trim());
						featureOrder.SetValue(GetOrderValue(ss[1]));
						orders.Add(featureOrder);
					}
				}
			}
			return orders;
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

		public virtual ICollection<string> GetAllFeatures()
		{
			HashSet<string> set = new HashSet<string>();
			IList<FeatureSelectOrder> andOrders = BuildOrder(andFeatures);
			IList<FeatureSelectOrder> orOrders = BuildOrder(orFeatures);
			foreach (ISelectOrder order in orOrders)
			{
				Sharpen.Collections.AddAll(set, order.GetAllFeatures());
			}
			foreach (ISelectOrder order_1 in andOrders)
			{
				Sharpen.Collections.AddAll(set, order_1.GetAllFeatures());
			}
			return set;
		}
	}
}
