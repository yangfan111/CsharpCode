using Sharpen;

namespace com.wd.free.para
{
	public class SimpleParable : IParable
	{
		private ParaList list;

		public SimpleParable()
		{
			this.list = new ParaList();
		}

		public SimpleParable(ParaList list)
		{
			this.list = list;
		}

        public object GetFieldObject(int index)
        {
            if(list is SimpleParaList)
            {
                SimpleParaList spl = (SimpleParaList)list;
                if(spl.GetFieldList().Count > index)
                {
                    IFields fields = (ObjectFields)spl.GetFieldList()[index];
                    if(fields is ObjectFields)
                    {
                        return ((ObjectFields)fields).GetObj();
                    }
                }
            }

            return null;
        }

		public virtual void SetList(ParaList list)
		{
			this.list = list;
		}

		public virtual ParaList GetParameters()
		{
			return list;
		}
	}
}
