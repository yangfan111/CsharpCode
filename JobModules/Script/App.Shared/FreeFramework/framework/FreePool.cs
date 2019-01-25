using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Free.framework
{
    public class FreePool
    {
        public static SimpleProto Allocate()
        {
            return new SimpleProto();
        }
    }
}
