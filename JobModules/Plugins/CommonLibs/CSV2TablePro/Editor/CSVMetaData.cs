using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace C2TPro
{
	[Serializable]
	public class CSVMetaData
	{
		public static string[] ColumnType = {"string", "int", "float", "bool"};
		public List<string> columns = new List<string>();
		public List<string> columnTypes = new List<string>();
        public bool isStatic = true;
        public string staticCsvPath = "";
        public string generatedScriptPath = "";

		public string GetColumnType(string column)
		{
			int i = columns.IndexOf(column);
			return (i >= 0) ? columnTypes[i] : "string";
		}

		public void UpdateColumns(TextAsset csv)
		{
			string[][] grid = CsvParser.Parse(csv.text);
			Dictionary<string, string> dict = new Dictionary<string, string>();
			for(int i = 0 ; i < grid[0].Length ; i++)
			{
				string column = grid[0][i];
				string type = GetColumnType(column);
				dict.Add(column, type);
			}
			columns = new List<string>(dict.Keys);
			columnTypes = new List<string>(dict.Values);
		}

		public static CSVMetaData CreateFromXml(string xml)
		{
            if (string.IsNullOrEmpty(xml))
                return null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CSVMetaData));
                CSVMetaData data = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as CSVMetaData;
                return data;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                return null;
            }
		}

		public string ToXml()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CSVMetaData));
			StringWriter swriter = new StringWriter();
			XmlTextWriter xwriter = new XmlTextWriter(swriter);
			serializer.Serialize(xwriter, this);
			return swriter.ToString();
		}
	}
}