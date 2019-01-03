using UnityEngine;
[UnityEngine.ExecuteInEditMode]
[UnityEngine.DisallowMultipleComponent]

public class AudioComponentMaker : MonoBehaviour
{

    public AudioComponentMaker S_Instance { get; private set; }
    private void OnEnable()
    {
        if (S_Instance == this)
            DestroyWwiseGlobalExistance();
        // AkWwisePostImportCallbackSetup.CheckWwiseGlobalExistance();

    }
    private void OnDisable()
    {
        if (S_Instance == this)
            DestroyWwiseGlobalExistance();
    }
    private void OnDestroy()
    {
        S_Instance = null;
    }
    private void Awake()
    {

        if (S_Instance && S_Instance != this)
        {
            gameObject.SetActive(false);
            return;

        }

        if (!Application.isPlaying)
        {
            DestroyWwiseGlobalExistance();
        }
        else
        {
            S_Instance = this;
            CreateWiseGlobal();
        }
    }
    public void CreateWiseGlobal()
    {

        // wiseObject = new GameObject("WwiseGlobal");
        gameObject.AddComponent<AkInitializer>();
        gameObject.AddComponent<WisePluginNotificationRoute>();
    }
    private void OnApplicationQuit()
    {
        //  DestroyWwiseGlobalExistance();
    }
    public static void DestroyWwiseGlobalExistance()
    {

#if UNITY_EDITOR
        if (Application.isPlaying) return;
        //while (true)
        //{
        //    //var oldObj = UnityEngine.GameObject.Find("AudioMaker");
        //    //if (oldObj == null)
        //    //    break;
        //    //UnityEditor.Undo.DestroyObjectImmediate(oldObj);

        //}
#endif
    }
}
