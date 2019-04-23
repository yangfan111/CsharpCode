using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace Assets.Utils.Utils
{
    public static class CopyUtil
    {
        /// <summary>
        /// 只赋值第一级对象，且要求是值类型
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void SimpleMap<T1, T2>(T1 from, T2 to)
        {
            var fromFields = from.GetType().GetFields();
            var toFields = to.GetType().GetFields();
            for(var i = 0; i < fromFields.Length; i++)
            {
                for(var j = 0; j < toFields.Length; j++)
                {
                    if(fromFields[i].Name == toFields[j].Name && fromFields[i].FieldType == toFields[j].FieldType)
                    {
                        toFields[j].SetValue(to, fromFields[i].GetValue(from));
                    }
                }
            }
        } 
    }
}
