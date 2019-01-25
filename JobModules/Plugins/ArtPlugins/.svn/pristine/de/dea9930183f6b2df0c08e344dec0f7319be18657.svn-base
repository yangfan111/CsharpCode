using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ArtPlugins
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MultiTag))]
    public class MultiTagEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MultiTag mt = target as MultiTag;
            base.OnInspectorGUI();
            // int oldValue = mt.tagMask;
            //mt.tagMask = EditorGUILayout.MaskField(mt.tagMask, testType);
            // int addValue = mt.tagMask-oldValue;
            EditorGUILayout.LabelField("总数量:" + targets.Length);
            int[] checkedCount = new int[(int)MultiTag.TagEnum.Max];
            for (int i = 0; i < (int)MultiTag.TagEnum.Max; i++)
            {
                foreach (var item in targets)
                {
                    checkedCount[i] += (item as MultiTag).btags[i] ? 1 : 0;
                }
            }
            for (int i = 0; i < (int)MultiTag.TagEnum.Max; i++)
            {
                bool oldVlaue = mt.btags[i];
                EditorGUILayout.BeginHorizontal();
                mt.btags[i] = EditorGUILayout.Toggle(MultiTag.TagEnumName[i], mt.btags[i]);
                if (checkedCount[i] != targets.Length && checkedCount[i] != 0)
                {

                    if (GUILayout.Button("已勾选" + checkedCount[i]))
                    {

                    }
                    if (GUILayout.Button("未勾选" + (targets.Length - checkedCount[i])))
                    {

                    }
                    // EditorGUILayout.LabelField(mt.btags[i] ?(targets.Length - checkedCount[i])+"": checkedCount[i]+"");

                }
                EditorGUILayout.EndHorizontal();
                if (oldVlaue != mt.btags[i])
                {
                    foreach (var item in targets)
                    {
                        (item as MultiTag).btags[i] = mt.btags[i];
                    }
                }
            }
            //if (addValue != 0)
            //{
            //    foreach (var item in targets)
            //    {
            //        (item as MultiTag).tagMask = mt.tagMask;
            //    }
            //}
        }
    }
}