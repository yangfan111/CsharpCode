using Sharpen;
using com.wd.free.action;
using Core.Free;

namespace com.wd.free.para
{
	public class ObjectFields : IFields, IRule
	{
		private object obj;

		public ObjectFields(object obj)
			: base()
		{
			this.obj = obj;
		}

		public virtual object GetObj()
		{
			return obj;
		}

		public virtual void SetObj(object obj)
		{
			this.obj = obj;
		}

		public virtual bool HasField(string field)
		{
			return ReflectionCache.HasField(obj, field);
		}

		public virtual string[] GetFields()
		{
			return ReflectionCache.GetSimpleFieldNames(obj);
		}

		public virtual IPara Get(string field)
		{
			return new FieldPara(obj, field, ReflectionCache.GetField(obj, field));
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.ObjectFields;
        }
    }
}
