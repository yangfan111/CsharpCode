using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.wd.free.debug
{
    public class FreeDebugConfig
    {
        public bool Enable;

        private string fields;

        private List<string> fieldList;

        public FreeDebugConfig()
        {
            this.fieldList = new List<string>();
        }

        public string Fields
        {
            get { return fields; }
            set
            {
                fields = value;
                fieldList.Clear();
                foreach (string f in value.Split(','))
                {
                    fieldList.Add(f.Trim());
                }
            }
        }
    }
}
