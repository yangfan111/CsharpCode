﻿using System;
using System.IO;
using System.Xml.Linq;
using UnityEditor;

public class Xml2Cs
{
    const string strTab = "    ";
    const string strConfig = "Config";
    const string strItem = "Item";

    [@MenuItem("Assets/XML2CS")]
    public static void XML2CS()
    {
        string xmlName = Selection.activeObject.name;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path.EndsWith(".xml"))
        {
            string fileName = MachiningFileName(xmlName);
            string classItemName = fileName.Replace(strConfig, strItem);
            string filePath = EditorUtility.SaveFilePanel("save config CS", "Assets/Hall.Template", fileName, "cs");
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileStream fs = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.Xml.Serialization;");

            sw.WriteLine();
            sw.WriteLine("namespace Template");
            sw.WriteLine("{");

            XElement firstElement = XElement.Load(path);   //根节点（root）
            sw.WriteLine(strTab + string.Format("[XmlRoot(\"{0}\"", firstElement.Name) + ")]");

            foreach (var secondElement in firstElement.Elements())   //第二层（Items）
            {
                sw.WriteLine(strTab + "public class " + fileName);
                sw.WriteLine(strTab + "{");
                sw.WriteLine(strTab + strTab + "public " + classItemName + "[] " + secondElement.Name + ";");
                sw.WriteLine(strTab + "}");
                sw.WriteLine();

                foreach (var thirdElement in secondElement.Elements())//第三层
                {
                    sw.WriteLine(strTab + string.Format("[XmlType(\"{0}\"", thirdElement.Name) + ")]");
                    sw.WriteLine(strTab + "public class " + classItemName);
                    sw.WriteLine(strTab + "{");

                    foreach(var fourthElement in thirdElement.Elements())//第四层
                    {
                        string attrStr = string.Empty;
                        XAttribute attribute = fourthElement.Attribute("type");
                        if (attribute != null)
                        {
                            attrStr = attribute.Value;
                        }
                        string type = "int";
                        string str = strTab + strTab;
                        if (attrStr.Equals("string"))
                        {
                            type = "string";
                        }
                        else if(attrStr.Equals("float"))
                        {
                            type = "float";
                        }

                        str += string.Format("public {0} " + fourthElement.Name + ";", type);
                        sw.WriteLine(str);
                    }
                    sw.WriteLine(strTab + "}");
                    sw.WriteLine("}");
                    break;
                }
                break;
            }

            sw.Close();
            fs.Close();
        }
    }

    static string MachiningFileName(string name)
    {
        string fileName = "";
        string[] temp = name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = temp[i].Substring(0, 1).ToUpper() + temp[i].Substring(1, temp[i].Length - 1);
            fileName += temp[i];
        }
        fileName += strConfig;
        return fileName;

    }
}
