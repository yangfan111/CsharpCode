using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Entitas;
using Entitas.Utils;

namespace Core.Utils
{
    public static class DumpEntatis
    {
      

        public static string Dump(this Entity entity)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var component in entity.GetComponents())
            {
                
                sb.AppendFormat("Component:{0}:\n", component.GetType().ToString());
                var memberInfos = GetPublicMemberInfos(component.GetType());
                foreach (var info in memberInfos)
                {
                    var memberValue = info.GetValue(component);
                   
                    sb.AppendFormat("     {0}:{1}:\n", info.name, memberValue);
                }
            }


            return sb.ToString();
        }

        public static List<PublicMemberInfo> GetPublicMemberInfos(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = type.GetFields(bindingFlags);
            var propertyInfos = type.GetProperties(bindingFlags);
            var memberInfos = new List<PublicMemberInfo>(
                fieldInfos.Length + propertyInfos.Length
            );

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                memberInfos.Add(new PublicMemberInfo(fieldInfos[i]));
            }

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                {
                    memberInfos.Add(new PublicMemberInfo(propertyInfo));
                }
            }

            return memberInfos;
        }
    }
}