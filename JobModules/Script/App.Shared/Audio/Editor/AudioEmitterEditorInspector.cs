#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;





[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AudioEmitterEditor))]
public class AudioEmitterEditorInspector : AkBaseInspector
{
    private UnityEditor.SerializedProperty EmitType { get; set; }

    private SerializedProperty LookAtListener { get; set; }

    private UnityEditor.SerializedProperty TriggerType { get; set; }

    private UnityEditor.SerializedProperty SourceData { get; set; }

    private UnityEditor.SerializedProperty LogEnabled { get; set; }

    private UnityEditor.SerializedProperty EmitTimeInterval { get; set; }

    private UnityEditor.SerializedProperty EventListProperty { get; set; }

    private SerializedProperty EventActionData { get; set; }

    private FieldInfo[] sourceDataFields;

    private FieldInfo[] SourceDataFields
    {
        get
        {
            if (sourceDataFields == null)
            {
                sourceDataFields = typeof(AudioSourceEditorData).GetFields();
            }

            return sourceDataFields;
        }
    }

    public void OnGUI()
    {
    }

    public override void OnChildInspectorGUI()
    {
        instance = (AudioEmitterEditor) target;
        //  InspectorBuildHelper.BuildEnum(TriggerType, EAudioTriggerType.None);
        BuildSourceDataBox();
        BuildEmitterBox();
        BuildEventTemplate();
        InspectorBuildHelper.BuildSimple(LogEnabled, "LogEnabled");
        InspectorBuildHelper.BuildSimple(LookAtListener, "LookAtListener");

        //  BuildEmitterBox();
    }

    void BuildSourceDataBox()
    {
        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
        using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
        {
            InspectorBuildHelper.BuildTip("SourceData");
            var nowClassify = instance.SourceData.EventClassify;
            InspectorBuildHelper.BuildEnum(SourceData.FindPropertyRelative("EventClassify"), EAudioEventClassify.Other);
            var sourceDataEventClassifyArr = instance.SourceData.EventClassifyArr;
            var nowIndex                   = instance.SourceData.eventArrIndex;
            //用做混合列表选择
            //    var nowMask = InspectorBuildConst.GetBuildMaskByArrIndex(instance.SourceData.eventArrIndex);
            var nextIndex = UnityEditor.EditorGUILayout.Popup("EventName", nowIndex, sourceDataEventClassifyArr);
            instance.SourceData.eventArrIndex = nextIndex;
            var eventSwitchGroups = instance.SourceData.EventSwitchGroups;
            if (eventSwitchGroups.Count == 0)
                return;
            foreach (var group in eventSwitchGroups)
            {
                group.selectedIndex = UnityEditor.EditorGUILayout.Popup("Group:", group.selectedIndex, group.StateArr);
            }
            // instance.SourceData.eventName = eventClassifyList[newIndex];


            //UnityEditor.EditorGUILayout.PropertyField(enableActionOnEvent, new UnityEngine.GUIContent("Action On Event: "));
        }

        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
    }

    void BuildEventTemplate()
    {
        using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
        {
            InspectorBuildHelper.BuildTip("Event Commom Action");
            InspectorBuildHelper.BuildSimple(actionOnEventType, "Event Executable");
            InspectorBuildHelper.BuildSimple(curveInterpolation, "Interpolation Curve");
            InspectorBuildHelper.BuildSlider(transitionDuration, new Vector2(0, 60f), "transitionDuration (s)");
            if (InspectorBuildHelper.BuildButton("Excute"))
            {
                instance.PlayEventTemplate();
            }
            
        }
    }

    private AudioEmitterEditor instance;

    void BuildEmitterBox()
    {
        tmpConstructActions1.Clear();
        tmpConstructActions1.Add(() => InspectorBuildHelper.BuildEnum(EmitType, EAudioEmitType.PlayOnce));
        //var list = new List<Action>();
        tmpConstructActions1.Add(() => InspectorBuildHelper.BuildSimple(EmitTimeInterval, "Emit Interval"));
        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
        using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
        {
            InspectorBuildHelper.BuildEnum(EmitType, EAudioEmitType.PlayOnce);
            if ((EAudioEmitType) EmitType.enumValueIndex == EAudioEmitType.PlayOnce)
            {
                if (InspectorBuildHelper.BuildButton("Play"))
                {
                    instance.PlayOnce();
                }
            }
            else
            {
                InspectorBuildHelper.BuildSimple(EmitTimeInterval, "Emit interval");
                if (instance.StopContinueAudio)
                {
                    if (InspectorBuildHelper.BuildButton("Resume"))
                    {
                        instance.Resume();
                    }
                }
                else
                {
                    if (InspectorBuildHelper.BuildButton("Stop"))
                    {
                        instance.Stop();
                    }
                }
            }
            if (InspectorBuildHelper.BuildButton("Stop All"))
            {
                instance.StopAll();
            }
            if (InspectorBuildHelper.BuildButton("Reload Config XML"))
            {
                instance.ReloadXML();
            }
            if (InspectorBuildHelper.BuildButton("Init Wise Bank"))
            {
                instance.ReloadWiseBank();
            }
        }

        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
    }


    public void OnEnable()
    {
//        var properties = this.GetType().GetProperties();
//        foreach (var property in properties)
//        {
//            var attrs = property.GetCustomAttributes(false);
//            if(attrs.Length==0||!(attrs[0] is EditorSerializedSimple))
//                continue;
//            Debug.Log(property.Name);
//            property.SetValue(this,FindProperty(property.Name),null);
//            
//        }
        EmitType        = FindProperty("EmitType");
        TriggerType     = FindProperty("TriggerType");
        SourceData      = FindProperty("SourceData");
        EventActionData = FindProperty("EventActionData");

        actionOnEventType  = EventActionData.FindPropertyRelative("ActionOnEventType");
        curveInterpolation = EventActionData.FindPropertyRelative("Interpolation");
        transitionDuration = EventActionData.FindPropertyRelative("Transition");


        EmitTimeInterval = FindProperty("EmitTimeInterval");
        LogEnabled       = FindProperty("EnableEmitterLog");
        LookAtListener   = FindProperty("LookAtListener");

        tmpConstructActions3 = new List<Action>();
        tmpConstructActions2 = new List<Action>();
    }


    private UnityEditor.SerializedProperty actionOnEventType;
    private UnityEditor.SerializedProperty curveInterpolation;
    private UnityEditor.SerializedProperty transitionDuration;


    
}

#endif