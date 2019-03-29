using Sharpen;

namespace com.wd.free.action.function
{
	[System.Serializable]
	public class ArgValue
	{
		private const long serialVersionUID = 2425028715886833864L;

		private string name;

		private string value;

		public ArgValue()
		{
		}

		public ArgValue(string name, string value)
			: base()
		{
			this.name = name;
			this.value = value;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetValue()
		{
			return value;
		}

		public virtual void SetValue(string value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			return name + "=" + value;
		}
	}
}
