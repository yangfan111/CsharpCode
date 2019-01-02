#if UNITY_EDITOR
using UnityEngine;
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(VoyagerSettingsComponent))]
public class VoyagerSettingComponentInspector : UnityEditor.Editor
{
    //public bool isForbiden = false;

    private static bool s_usePicker;
    private static string s_loadType;
    private static string s_wiseInstallPath;
    private static string s_wiseProjectPath;


    private UnityEditor.SerializedProperty audioLoadTypeWhenStarup;
    private UnityEditor.SerializedProperty wiseInstallationPath;
    private UnityEditor.SerializedProperty wiseProjectPath;
    private VoyagerSettingsComponent targetComponent;
    private bool flagFirstEnable = false;
    public void OnEnable()
    {
        flagFirstEnable = true;
        targetComponent = target as VoyagerSettingsComponent;
        audioLoadTypeWhenStarup = serializedObject.FindProperty("audioLoadTypeWhenStarup");
        wiseInstallationPath = serializedObject.FindProperty("wiseInstallationPath");
        wiseProjectPath = serializedObject.FindProperty("wiseProjectPath");
    }
    public void OnDisable()
    {
        flagFirstEnable = false;
    }
    public override void OnInspectorGUI()

    {
        serializedObject.Update();
        if(!flagFirstEnable)
        {
            s_usePicker = targetComponent.usePicker;
            s_wiseInstallPath = targetComponent.wiseInstallationPath;
            s_wiseProjectPath = targetComponent.wiseProjectPath;
            s_loadType = targetComponent.audioLoadTypeWhenStarup;
        }
        else
        {
            flagFirstEnable = false;
        }
    

        UnityEngine.GUILayout.BeginVertical();

        //    s_IsForbiden = targetComponent.isForbiden;
        targetComponent.isForbiden = UnityEditor.EditorGUILayout.Toggle("ForbidenWise", targetComponent.isForbiden);

        targetComponent.usePicker = UnityEditor.EditorGUILayout.Toggle("useWisePicker", targetComponent.usePicker);
        UnityEditor.EditorGUILayout.PropertyField(audioLoadTypeWhenStarup, new UnityEngine.GUIContent("audioLoadTypeWhenStarup"));
        UnityEditor.EditorGUILayout.PropertyField(wiseInstallationPath, new UnityEngine.GUIContent("wiseInstallationPath"));
        UnityEditor.EditorGUILayout.PropertyField(wiseProjectPath, new UnityEngine.GUIContent("wiseProjectPath"));
        
        UnityEngine.GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        OnGUIVary();
    }

    private void OnGUIVary()
    {
        if (s_usePicker != targetComponent.usePicker)
        {
            VoyagerCustomizeSettings.SetCreatePacker(targetComponent.usePicker);
        }
        if (s_wiseInstallPath != targetComponent.wiseInstallationPath)
        {
            VoyagerCustomizeSettings.DeveloperWwiseInstallationPath = targetComponent.wiseInstallationPath;
        }
        if (s_wiseProjectPath != targetComponent.wiseProjectPath)
        {
            VoyagerCustomizeSettings.DeveloperWwiseProjectPath = targetComponent.wiseProjectPath;
        }
     
    }
}

#endif