using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace com.wd.free.xml
{
    public class XmlParser
    {
        public static object FromXml(string xml, XmlAlias alias)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNode node = doc.DocumentElement;
            return handleOneNode(alias, node, null, null);
        }

        private static object handleOneNode(XmlAlias alias, XmlNode node, object obj, string parentName)
        {
            if (node != null)
            {
                string name = GetClassName(node);
                object childObj = alias.GetObject(name);

                if(childObj == null)
                {
                    return obj;
                }

                if (obj == null)
                {
                    obj = childObj;
                }
                else
                {
                    alias.SetField(parentName, obj, childObj, node.Name);
                }

                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name != "class")
                        {
                            alias.SetFieldValue(name, childObj, attr.Name, attr.Value);
                        }
                    }
                }
                
                foreach (XmlNode child in node.ChildNodes)
                {
                    handleOneNode(alias, child, childObj, name);
                }
            }

            return obj;
        }

        private static string GetFieldName(object parent, XmlNode node)
        {
            return null;
        }

        private static string GetClassName(XmlNode node)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == "class")
                    {
                        return attr.Value;
                    }
                }
            }

            return node.Name;
        }
    }
}
