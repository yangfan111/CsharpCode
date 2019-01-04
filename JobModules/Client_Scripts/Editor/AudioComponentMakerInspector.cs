//#if UNITY_EDITOR
//using Core.Audio;

//using UnityEditor.Rendering.PostProcessing;
//using UnityEngine;

//[UnityEditor.CanEditMultipleObjects]
//[UnityEditor.CustomEditor(typeof(AudioComponentMaker))]
//public class AudioComponentMakerInspector : BaseEditor<AudioComponentMaker>
//{
//    private AudioComponentMaker target;
//    private static AudioConfigData localConfigData;
//    private static bool s_openOptions = false;
//    private static readonly string audioConfigPath = "projAudioSettings.xml";
//    private SerializedProperty dataProperty;

//    SerializedProperty s_isForbidden;
//    private SerializedProperty s_usePicker;
//    private SerializedProperty s_wiseInstallPath;
//    private SerializedProperty s_wiseProjPath;
//    private SerializedProperty s_audioLoadType;

//    private static void LoadSettings()
//    {
//        if (localConfigData != null)
//        {
//            return;
//        }
//        localConfigData = new AudioConfigData();
//        try
//        {
//            if (System.IO.File.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath, audioConfigPath)))
//            {
//                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(AudioConfigData));
//                var xmlFileStream = new System.IO.FileStream(UnityEngine.Application.dataPath + "/" + audioConfigPath,
//                    System.IO.FileMode.Open, System.IO.FileAccess.Read);
//                localConfigData = (AudioConfigData)xmlSerializer.Deserialize(xmlFileStream);
//                xmlFileStream.Close();
//            }
//            else
//            {
//                localConfigData = AudioConfigData.Output;

//            }

//        }
//        catch (System.Exception)
//        {
//        }

//    }
//    private static void SaveSettings()
//    {
//        try
//        {
//            var xmlDoc = new System.Xml.XmlDocument();
//            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(AudioConfigData));
//            using (var xmlStream = new System.IO.MemoryStream())
//            {
//                var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
//                xmlSerializer.Serialize(streamWriter, localConfigData);
//                xmlStream.Position = 0;
//                xmlDoc.Load(xmlStream);
//                xmlDoc.Save(System.IO.Path.Combine(UnityEngine.Application.dataPath, audioConfigPath));
//            }
//        }
//        catch (System.Exception)
//        {
//        }
//    }
//    public void OnEnable()
//    {
//        LoadSettings();
    
//        target = base.target as AudioComponentMaker;
//        if (!target) return;
//        target.configData = localConfigData;
//        dataProperty = serializedObject.FindProperty("configData");
//        s_isForbidden = FindProperty(x=>x.configData.isForbiden);
//        s_usePicker = FindProperty(x=>x.configData.usePicker);
//        s_wiseInstallPath= FindProperty(x => x.configData.wiseInstallationPath);
//        s_wiseProjPath = FindProperty(x => x.configData.wiseProjectPath);
//        s_audioLoadType = FindProperty(x => x.configData.audioLoadTypeWhenStarup);







//    }
//    public override void OnInspectorGUI()
//    {
//        if (!target) return;
//        serializedObject.Update();
//        s_openOptions = target.openEditorOptions;
//        target.openEditorOptions = UnityEditor.EditorGUILayout.Toggle("openEditorOptions", target.openEditorOptions);
//        DisplaySettingComp(target.openEditorOptions);

//        serializedObject.ApplyModifiedProperties();
//    }
//    static string[] arr = new string[2] { "1", "2" };
//    void DisplaySettingComp(bool display)
//    {
//        if (display)
//        {

//            EditorGUILayout.BeginVertical();
//            EditorGUILayout.PropertyField(s_isForbidden);
//            EditorGUILayout.PropertyField(s_wiseProjPath);
//            EditorGUILayout.PropertyField(s_wiseInstallPath);
//            EditorGUILayout.PropertyField(s_usePicker);
//            EditorGUILayout.PropertyField(s_audioLoadType);


//            if (GUILayout.Button("SaveSettings", GUILayout.MaxWidth(200f)))
//            {
//                SaveSettings();
//            }

//            EditorGUILayout.EndVertical();
//        }
//        //var cmp = target.gameObject.GetComponent(typeof(AudioCoreSettingsComponent));
//        //if (display)
//        //{
//        //    if (!cmp)
//        //        target.settingComponent = UnityEditor.Undo.AddComponent<AudioCoreSettingsComponent>(target.gameObject);
//        //}
//        //else if (cmp)
//        //{
//        //    //TODO:component做序列化保存
//        //    UnityEditor.Undo.DestroyObjectImmediate(cmp);
//        //}

//    }
//}


//#endif