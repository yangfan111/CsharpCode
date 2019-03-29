using System;
using System.Collections.Generic;
using UnityEngine;
using XmlConfig;

namespace Core.CharacterController
{

    public class AirMoveCurve:MonoBehaviour
    {
#if UNITY_EDITOR
        public bool SaveFile = false;
#endif
        
        public AnimationCurve AireMoveCurve = new AnimationCurve(
            new Keyframe[3]
            {
                new Keyframe(0f, 1f),
                new Keyframe(1.0f, 0.95f),
                new Keyframe(1.5f, 0.5f)
            });
        
        public List<MovementCurveInfo> MovementCurve = new List<MovementCurveInfo>();

#if UNITY_EDITOR
        void Update()
        {
            if (SaveFile)
            {
                var path = UnityEditor.EditorUtility.SaveFilePanel(
                    "Save cure as xml",
                    "",
                    "AirMoveCurve.xml",
                    "xml");
                if (path.Length > 0)
                {
                    CurveSerializerTool.GenerateConfig(path, new SpeedCurveConfig(AireMoveCurve, MovementCurve));
                    UnityEditor.EditorUtility.DisplayDialog("success", "save file to path " + path, "ok");
                }
                SaveFile = false;
            }
        }
#endif
    }
}