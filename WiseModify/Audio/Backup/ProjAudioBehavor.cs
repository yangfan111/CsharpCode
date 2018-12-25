//using UnityEngine;
//using System.Collections;
//using   App.Shared.Audio;

//public class ProjAudioBehavor : MonoBehaviour
//{
//    GameObject go;
    
//    private void Awake()
//    {
      
//    }


  
//    private void OnMouseDown()
//    {
//        Debug.Log("Mouse Down");
//        AudioStatic.Dispatcher.PostTriggerSignal(AudioTriggerArgsFactory.CreateSceneArgs("fight"));


//    }
//    private void OnEnable()
//    {
//        Debug.Log("Mouse up");
//        AudioStatic.Dispatcher.PostTriggerSignal(AudioTriggerArgsFactory.CreateSceneArgs("fight"));
//    }
//    IEnumerator TryAudioContinue()
//    {
//        while(true)
//        {
          
//            // AudioStatic.Driver.PostEvent(EVENTS.GUN_56_SHOT,gameObject);
//            yield return new WaitForSeconds(5.0f);
//            AudioStatic.Dispatcher.PostTriggerSignal(AudioTriggerArgsFactory.CreateSceneArgs("fight"));
//        }
      
//    }
//    string keyDownInput = null;
//    void Update()
//    {
//       if(Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            AudioStatic.Driver.PostEvent(EVENTS.CAR_START,this.gameObject);
//        }
//        if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
         
//            AudioStatic.Driver.PostEvent(EVENTS.CAR_STOP, this.gameObject);
//        }


//        //StartCoroutine(TryAudioContinue());
//    }
//    private void OnGUI()
//    {
//        GUILayout.BeginHorizontal();
//        if(GUILayout.Button("car start", GUILayout.Width(100), GUILayout.Height(50)))
//        {
//            AudioStatic.Driver.PostEvent(EVENTS.CAR_START, this.gameObject);
//        }
//        if(GUILayout.Button("car stop", GUILayout.Width(100), GUILayout.Height(50)))
//        {
//            AudioStatic.Driver.PostEvent(EVENTS.CAR_STOP, this.gameObject);
//        }
//        if (GUILayout.Button("56 shot", GUILayout.Width(100), GUILayout.Height(50)))
//        {
//            AudioStatic.Driver.PostEvent(EVENTS.GUN_56_SHOT, this.gameObject);
//        }
//        if (GUILayout.Button("56 stop", GUILayout.Width(100), GUILayout.Height(50)))
//        {
          
//            AudioStatic.Driver.PostEvent(EVENTS.GUN_56_SHOT_CONTINUE_STOP, this.gameObject);
//        }
//        GUILayout.EndHorizontal();
//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("Switch shot continue", GUILayout.Width(200), GUILayout.Height(50)))
//        {
//            AudioStatic.Driver.ChangeTargetGroupState(SWITCHES.GUN_SHOT_MODE_TYPE.GROUP, SWITCHES.GUN_SHOT_MODE_TYPE.SWITCH.GUN_SHOT_MODE_TYPE_CONTINUE, this.gameObject);
//        }
//        if (GUILayout.Button("Switch shot single", GUILayout.Width(200), GUILayout.Height(50)))
//        {
//            AudioStatic.Driver.ChangeTargetGroupState(SWITCHES.GUN_SHOT_MODE_TYPE.GROUP, SWITCHES.GUN_SHOT_MODE_TYPE.SWITCH.GUN_SHOT_MODE_TYPE_SINGLE, this.gameObject);
//        }
   
//        //if (GUILayout.Button("cam water", GUILayout.Width(200), GUILayout.Height(50)))
//        //{
//        //    AudioStatic.Driver.SwitchGlobalState(STATES.GLOBAL_CAMERA.GROUP, STATES.GLOBAL_CAMERA.STATE.GLOBAL_CAMERA_OVERWATER);
//        //}
//        //if (GUILayout.Button("cam under water", GUILayout.Width(200), GUILayout.Height(50)))
//        //{
//        //    AudioStatic.Driver.SwitchGlobalState(SWITCHES.GUN_SHOT_MODE_TYPE.GROUP, STATES.GLOBAL_CAMERA.STATE.GLOBAL_CAMERA_UNDERWATER);
//        //}
//        GUILayout.EndHorizontal();
//    }
//}

