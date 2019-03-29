using Sharpen;

namespace com.wd.free.action.function
{
	[System.Serializable]
	public class FuncArg
	{
		private const long serialVersionUID = -5858886919480535254L;

		private string type;

		private string name;

		private string desc;

		private string defaultValue;

		public FuncArg()
		{
		}

		public FuncArg(string type, string name)
			: base()
		{
			this.type = type;
			this.name = name;
		}

		public virtual string GetType()
		{
			return type;
		}

		public virtual void SetType(string type)
		{
			this.type = type;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual string GetDefaultValue()
		{
			if (defaultValue == null)
			{
				defaultValue = string.Empty;
			}
			return defaultValue;
		}

		public virtual void SetDefaultValue(string defaultValue)
		{
			this.defaultValue = defaultValue;
		}
	}
}
