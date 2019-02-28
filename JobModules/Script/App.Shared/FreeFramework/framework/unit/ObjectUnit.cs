using com.wd.free.para;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.framework.unit
{
    public class ObjectUnit : IParable
    {
        private SimpleParaList list;
        private object obj;

        public ObjectUnit(object obj)
        {
            this.obj = obj;
            list = new SimpleParaList();
            list.AddFields(new ObjectFields(obj));
        }

        public object GetObject
        {
            get { return obj; }
        }

        public void AddField(IPara para)
        {
            this.list.AddPara(para);
        }

        public ParaList GetParameters()
        {
            return list;
        }
    }
}
