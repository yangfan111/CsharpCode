#if UNITY_EDITOR
using Core.Audio;
using UnityEngine;
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AudioComponentMaker))]
public class AudioComponentMakerInspector : UnityEditor.Editor
{
    private AudioComponentMaker target;
    private static bool s_openOptions = false;
    public void OnEnable()
    {
        target = base.target as AudioComponentMaker;
        DisplaySettingComp(target.openEditorOptions);

    }
    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        s_openOptions = target.openEditorOptions;
        target.openEditorOptions = UnityEditor.EditorGUILayout.Toggle("openEditorOptions", target.openEditorOptions);
        if (s_openOptions != target.openEditorOptions)
        {
            DisplaySettingComp(target.openEditorOptions);
        }

    }
    void DisplaySettingComp(bool display)
    {
        var cmp = target.gameObject.GetComponent(typeof(AudioCoreSettingsComponent));
        if (display)
        {
            if (!cmp)
                target.settingComponent = UnityEditor.Undo.AddComponent<AudioCoreSettingsComponent>(target.gameObject);
        }
        else if (cmp)
        {
            //TODO:component做序列化保存
            UnityEditor.Undo.DestroyObjectImmediate(cmp);
        }

    }
}


#endif