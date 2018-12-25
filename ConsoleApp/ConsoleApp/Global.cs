using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleApp.Test;
namespace ConsoleApp
{
    public enum SerializableType
    {

        Bin =1,
        Soap =2,
    }
    public enum EnumType
    {
        e1,
        e2
    }
    public static class ReadOnlyAss
    {
        public static SerializeObject DefualtSerializeObject= new CustomSerialize() { num = 99 ,n1 = 1000};
    }
    public enum CustomCallbackType
    {
        Default,
    }
}
