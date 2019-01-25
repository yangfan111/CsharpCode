using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public abstract class IPara : IPoolable
	{
		public const string CON_EQUAL = "==";

		public const string CON_GREATER = ">";

		public const string CON_LESS = "<";

		public const string CON_NOT_EQUAL = "<>";

		public const string CON_GE = ">=";

		public const string CON_LE = "<=";

		public const string OP_ASSIGN = "=";

		public const string OP_MINUS = "-";

		public const string OP_ADD = "+";

		public const string OP_DIVIDE = "/";

		public const string OP_MULTI = "*";

		public const string OP_MOD = "%";

		public abstract string GetName();

		public abstract void SetName(string name);

		public abstract bool IsPublic();

		public abstract void SetPublic(bool p);

		public abstract object GetValue();

		public abstract string GetDesc();

		public abstract IPara Initial(string con, string v);

		public abstract string[] GetConditions();

		public abstract bool Meet(string con, IPara v);

		public abstract string[] GetOps();

		public abstract void SetValue(string op, IPara v);

	    public abstract IPoolable Copy();

	    public abstract IPoolable Borrow();

	    public abstract void Recycle();

	    public abstract bool IsTemp();
    }

	public static class IParaConstants
	{
	}
}
