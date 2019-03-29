using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Core.Configuration.Equipment;
using Core.Utils;

namespace Core.Configuration.Utils
{
    public class XmlNodeBase : IXmlNodeParser
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ResourceItem));

        protected static void Init(Type t, Dictionary<string, Action<object, string>> handler)
        {
            foreach (var field in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.IsDefined(typeof(XmlNodeAttribute), false))
                {
                    var attr = (XmlNodeAttribute)field.GetCustomAttributes(typeof(XmlNodeAttribute), false)[0];
                    FieldInfo f = field;
                    handler.Add(attr.Id, (instance, value) =>
                    {
                        f.SetValue(instance, StringUtil.Handler[f.FieldType](value));
                    });
                }
            }
        }

        protected virtual Dictionary<string, Action<object, string>> GetHandler()
        {
            throw new NotImplementedException();
        }

        protected virtual void ParseSpecial(XmlNode node)
        {
            _logger.WarnFormat("Unknown XmlNode: {0}", node.Name);
        }

        public void Parse(XmlNode node)
        {
            var handler = GetHandler();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (handler.ContainsKey(child.Name))
                {
                    handler[child.Name](this, child.InnerText);
                }
                else
                {
                    ParseSpecial(child);
                }
            }
        }
    }
}
