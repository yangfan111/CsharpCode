using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data
{
	/// <summary>单条数据记录封装类</summary>
	/// <author>Dave</author>
	[System.Serializable]
	public class DataRecord
	{
		private const long serialVersionUID = -1070543050853190777L;

		/// <summary>数据操作类型枚举</summary>
		/// <author>Dave</author>
		public enum DataAction
		{
			Add,
			Update,
			Delete
		}

		/// <summary>域名与域值对应关系</summary>
		private Dictionary<string, object> dataMap;

		/// <summary>数据操作类型</summary>
		private DataRecord.DataAction dataAction;

		public DataRecord()
		{
			this.dataMap = new MyDictionary<string, object>();
			this.dataAction = DataRecord.DataAction.Add;
		}

		public virtual string[] GetAllFields()
		{
			return Sharpen.Collections.ToArray(dataMap.Keys, new string[] {  });
		}

		public virtual string GetFieldValue(string field)
		{
			if (!dataMap.ContainsKey(field) || dataMap[field] == null)
			{
				return string.Empty;
			}
			return dataMap[field].ToString();
		}

		public virtual object GetFieldObject(string field)
		{
			return dataMap[field];
		}

		public virtual void DeleteField(string field)
		{
			this.dataMap.Remove(field);
		}

		public virtual bool ContainsField(string field)
		{
			return dataMap.ContainsKey(field);
		}

		public virtual void SetAction(DataRecord.DataAction action)
		{
			this.dataAction = action;
		}

		public virtual void AddField(string field, string value)
		{
			this.dataMap[field] = value;
		}

		public virtual void AddField(string field, object value)
		{
			this.dataMap[field] = value;
		}

		public override string ToString()
		{
			return dataMap.ToString();
		}

		public virtual com.cpkf.yyjd.tools.data.DataRecord Clone()
		{
			com.cpkf.yyjd.tools.data.DataRecord dr = new com.cpkf.yyjd.tools.data.DataRecord();
		    dr.dataMap = (Dictionary<string, object>) dataMap.CloneObject();
			dr.dataAction = dataAction;
			return dr;
		}
	}
}
