#if UNITY_EDITOR
using Core.Audio;
using UnityEngine;
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AudioCoreSettingsComponent))]
public class AudioCoreSettingsComponentComponentInspector : UnityEditor.Editor
{
    //public bool isForbiden = false;

    private static bool s_usePicker;
    private static string s_loadType;
    private static string s_wiseInstallPath;
    private static string s_wiseProjectPath;


    //private UnityEditor.SerializedProperty audioLoadTypeWhenStarup;
    //private UnityEditor.SerializedProperty wiseInstallationPath;
    //private UnityEditor.SerializedProperty wiseProjectPath;
    private AudioCoreSettingsComponent targetComponent;
    private bool flagFirstEnable = false;
    public void OnEnable()
    {
        flagFirstEnable = true;
        targetComponent = target as AudioCoreSettingsComponent;

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
            s_usePicker = targetComponent.coreSettingData.usePicker;
            s_wiseInstallPath = targetComponent.coreSettingData.wiseInstallationPath;
            s_wiseProjectPath = targetComponent.coreSettingData.wiseProjectPath;
            s_loadType = targetComponent.coreSettingData.audioLoadTypeWhenStarup;
        }
        else
        {
            flagFirstEnable = false;
        }
    

        UnityEngine.GUILayout.BeginVertical();
        //    s_IsForbiden = targetComponent.isForbiden;
  
        targetComponent.coreSettingData.isForbiden = UnityEditor.EditorGUILayout.Toggle("ForbidenWise", targetComponent.coreSettingData.isForbiden);
        //useWisePicker
        targetComponent.coreSettingData.usePicker = UnityEditor.EditorGUILayout.Toggle("useWisePicker", targetComponent.coreSettingData.usePicker);

        //loadTypeWhenStarup
        GUILayout.BeginHorizontal();
        GUILayout.Label("loadTypeWhenStarup:", GUILayout.ExpandWidth(false));
        targetComponent.coreSettingData.audioLoadTypeWhenStarup = UnityEditor.EditorGUILayout.TextField(targetComponent.coreSettingData.audioLoadTypeWhenStarup, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        //loadTypeWhenStarup
        GUILayout.BeginHorizontal();
        GUILayout.Label("wiseInstallationPath", GUILayout.ExpandWidth(false));
        targetComponent.coreSettingData.wiseInstallationPath = UnityEditor.EditorGUILayout.TextField(targetComponent.coreSettingData.wiseInstallationPath, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        //wiseProjectPath
        GUILayout.BeginHorizontal();
        GUILayout.Label("wiseProjectPath", GUILayout.ExpandWidth(false));
        targetComponent.coreSettingData.wiseProjectPath = UnityEditor.EditorGUILayout.TextField(targetComponent.coreSettingData.wiseProjectPath, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        UnityEngine.GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        OnGUIVary();
    }

    private void OnGUIVary()
    {
        if (s_usePicker != targetComponent.coreSettingData.usePicker)
        {
            AudioPluginSettingAgent.SetCreatePacker(targetComponent.coreSettingData.usePicker);
        }
        if (s_wiseInstallPath != targetComponent.coreSettingData.wiseInstallationPath)
        {
            AudioPluginSettingAgent.DeveloperWwiseInstallationPath = targetComponent.coreSettingData.wiseInstallationPath;
        }
        if (s_wiseProjectPath != targetComponent.coreSettingData.wiseProjectPath)
        {
            AudioPluginSettingAgent.DeveloperWwiseProjectPath = targetComponent.coreSettingData.wiseProjectPath;
        }



    }
}

#endif