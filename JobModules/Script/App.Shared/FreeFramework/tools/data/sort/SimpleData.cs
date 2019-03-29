using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public class SimpleData : IFeaturable
	{
		private const long serialVersionUID = -6084685037469937167L;

		private Dictionary<string, object> map;

		public SimpleData()
		{
			this.map = new MyDictionary<string, object>();
		}

		public virtual void AddFeatureValue(string feature, object value)
		{
			this.map[feature] = value;
		}

		public virtual object GetFeatureValue(string feature)
		{
			if (map.ContainsKey(feature))
			{
				return map[feature];
			}
			else
			{
				return string.Empty;
			}
		}

		public virtual bool HasFeature(string feature)
		{
			return map.ContainsKey(feature);
		}

		public override string ToString()
		{
			return map.ToString();
		}
	}
}
