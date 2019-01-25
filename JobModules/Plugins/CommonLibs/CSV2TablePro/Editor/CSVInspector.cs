using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace C2TPro
{
    [CustomEditor(typeof(TextAsset))]
    public class CSVInspector : Editor
    {
        bool foldout = true;
        Vector2 scrollPos = Vector2.zero;

        public static bool IsCsvAsset(UnityEngine.Object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is TextAsset))
                return false;

            string path = AssetDatabase.GetAssetPath(obj);
            string ext = Path.GetExtension(path).ToLower();
            if (ext != ".csv")
                return false;

            return true;
        }

        static void SaveCSVMetaData(CSVMetaData meta, TextAsset csv)
        {
            AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(csv));
            importer.userData = meta != null ? meta.ToXml() : "";
            importer.SaveAndReimport();
        }

        void DrawText()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(500));
            string text = ((TextAsset)target).text;
            int max = Mathf.Clamp(text.Length, 0, 2048);
            text = text.Substring(0, max);
            EditorGUILayout.TextArea(text);
            EditorGUILayout.EndScrollView();
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;

            if (!IsCsvAsset(target))
            {
                DrawText();
                return;
            }

            TextAsset csv = target as TextAsset;

            // Retrieve metaData from *.csv.meta file's userData
            AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(csv));
            CSVMetaData metaData = CSVMetaData.CreateFromXml(importer.userData);

            // Prevent UnityEditor Serialization...
            //if (metaData != null && metaData.columns.Count == 0)
            //  metaData = null;

            // Is Table?
            bool isTable = EditorGUILayout.Toggle("Is Table", metaData != null);
            if (isTable && metaData == null)
            {
                metaData = new CSVMetaData();
                metaData.UpdateColumns(csv);
                SaveCSVMetaData(metaData, csv);
                return;
            }
            if (!isTable && metaData != null)
            {
                SaveCSVMetaData(null, csv);
                return;
            }

            // if csv asset is not a table, early return.
            if (metaData == null)
            {
                if (!string.IsNullOrEmpty(importer.userData))
                {
                    string warn = string.Format("AssetImporter.userData is not empty: {0}\nMark 'Is Table' then reset AssetImporter.userData.",
                                                importer.userData);
                    EditorGUILayout.HelpBox(warn, MessageType.Warning);
                }
                DrawText();
                return;
            }

            // Is static?
            metaData.isStatic = EditorGUILayout.Toggle("Is Static", metaData.isStatic);
            if (metaData.isStatic)
            {
                string path = AssetDatabase.GetAssetPath(csv);
                if (!path.Contains("Resources/"))
                {
                    EditorGUILayout.HelpBox("'static table csv asset' must exists in Resources folder", MessageType.Warning);
                }
                metaData.staticCsvPath = AssetDatabase.GetAssetPath(csv).Replace("Assets/Resources/", "").Replace(".csv", "");
            }

            // csv asset
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("CSV", csv, typeof(TextAsset), false);
            EditorGUI.EndDisabledGroup();

            // Generated script
            MonoScript script = null;
            if (!string.IsNullOrEmpty(metaData.generatedScriptPath))
            {
                script = AssetDatabase.LoadAssetAtPath<MonoScript>(metaData.generatedScriptPath);
            }
            script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
            metaData.generatedScriptPath = script == null ? "" : AssetDatabase.GetAssetPath(script);

            // buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update Columns"))
            {
                metaData.UpdateColumns(csv);
            }
            if (GUILayout.Button("Generate Script"))
            {
                if (string.IsNullOrEmpty(metaData.generatedScriptPath))
                    metaData.generatedScriptPath = EditorUtility.SaveFilePanel("Save Script", "Assets", csv.name + "Table.cs", "cs");
                if (!string.IsNullOrEmpty(metaData.generatedScriptPath))
                {
                    script = CreateScript(csv, metaData.generatedScriptPath, metaData);
                    metaData.generatedScriptPath = AssetDatabase.GetAssetPath(script);
                    EditorGUIUtility.PingObject(script);
                }
            }
            EditorGUILayout.EndHorizontal();

            // columns
            foldout = EditorGUILayout.Foldout(foldout, "Columns");
            if (foldout)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < metaData.columns.Count; i++)
                {
                    string column = metaData.columns[i];
                    string type = metaData.columnTypes[i];
                    int typeIndex = GetColumnTypeIndex(type);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(column);
                    int newTypeIndex = EditorGUILayout.Popup(typeIndex, CSVMetaData.ColumnType);
                    if (newTypeIndex != typeIndex)
                    {
                        metaData.columnTypes[i] = CSVMetaData.ColumnType[newTypeIndex];
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            if (GUI.changed)
                SaveCSVMetaData(metaData, csv);

            DrawText();
        }

        int GetColumnTypeIndex(string type)
        {
            for (int i = 0; i < CSVMetaData.ColumnType.Length; i++)
                if (CSVMetaData.ColumnType[i] == type)
                    return i;
            return 0;
        }

        MonoScript CreateScript(TextAsset csv, string path, CSVMetaData metaData)
        {
            if (csv == null || string.IsNullOrEmpty(csv.text))
                return null;

            string className = Path.GetFileNameWithoutExtension(path);
            string code = TableCodeGen.Generate(csv.text, className, metaData);

            File.WriteAllText(path, code);
            Debug.Log("Table script generated: " + path);

            AssetDatabase.Refresh();

            // absolute path to relative
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            return AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
        }
    }
}