using System;
using com.wd.free.para;
using Sharpen;
using Core.Free;

namespace com.wd.free.action.function
{
	[System.Serializable]
	public class ArgValue : IRule
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

        public int GetRuleID()
        {
            return (int)ERuleIds.ArgValue;
        }
    }
}
