using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace ArtPlugins
{ 
public class LocalFileUtils {

        public static long getFileID(Component item)
        {
#if UNITY_EDITOR
      //      UnityEditor.EditorUtility.SetDirty(item);
            PropertyInfo inspectorModeInfo =
                typeof(UnityEditor.SerializedObject).GetProperty("inspectorMode",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            UnityEditor.SerializedObject serializedObject =
                new UnityEditor.SerializedObject(item);
            inspectorModeInfo.SetValue(serializedObject, UnityEditor.InspectorMode.Debug, null);
            UnityEditor.SerializedProperty localIdProp =
                serializedObject.FindProperty("m_LocalIdentfierInFile");

            long fildID = localIdProp.longValue;
            //Important set the component to dirty so it won‘t be overriden from a prefab!
            //UnityEditor.EditorUtility.SetDirty(item);
            return fildID;
#endif
        }
    }

}